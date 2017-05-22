#region Header

/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on :
* Author     : Vinoth, Moorthy, Manikandan, Rajkumar and Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

#endregion Header

namespace Pointel.Interactions.Email.Forms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

    using Microsoft.Win32;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Core;
    using Pointel.Interactions.Email.DataContext;
    using Pointel.Interactions.Email.Helper;
    using Pointel.Interactions.Email.Helpers;
    using Pointel.Interactions.Email.Listener;
    using Pointel.Interactions.Email.UserControls;
    using Pointel.Interactions.IPlugins;
    using Pointel.Tools;
    using System.Threading;

    /// <summary>
    /// Interaction logic for EmailMainWindow.xaml
    /// </summary>
    public partial class EmailMainWindow : Window, IEmailAttribute
    {
        #region Fields

        public const Int32 MF_BYPOSITION = 0x400;

        //Team Communicator
        public System.Windows.Controls.ContextMenu contextMenuTransfer = new System.Windows.Controls.ContextMenu();
        public long totalfilelength = 0;

        private const int CU_Close = 1002;
        private const int CU_Maximize = 1004;
        private const int CU_Minimize = 1000;
        private const int CU_Normal = 1001;
        private const int CU_Restore = 1003;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int MF_DISABLED = 0x2;
        private const int MF_ENABLED = 0x0;
        private const int MF_GRAYED = 0x1;
        private const int SC_Close = 0x0000f060;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Move = 0x0000f010;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Size = 0x0000f000;
        private const Int32 WM_SYSCOMMAND = 0x112;

        Dictionary<string, string> CustomerDetails = new Dictionary<string, string>();
        DataUserControl dataUserControl = null;
        private DialPad dialpad;

        // Server Events
        private EventGetInteractionContent eventGetInteractionContent;
        private string firstName;

        //User Controls
        InboundUserControl inboundUserControl = null;
        InteractionDataList interactionDataList = null;
        InteractionTypes interactionTypes;

        //Contact Server Notification
        bool isContactDatatobeUpdated = false;
        private bool isNeedToNotifyRefresh;
        private string lastName;

        // Logger
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        //Attachment
        OpenFileDialog openFileDialog = null;
        OutboundUserControl outboundUserControl = null;
        private IntPtr SystemMenu;
        double tempHeight;
        double tempLeft;
        double tempTop;
        double tempWidth;

        // string
        private string _contactID;
        private EmailDetails _emailDetails = new EmailDetails();
        private EmailWindowState _emailWindowState;
        private Importer _importClass;
        private string _interactionID;
        bool _isSaveMailToWorkbin = true;
        bool _isWindowSizeChanged = false;
        private string _parentIxnID;

        // For UI
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private DispatcherTimer _timerforcloseError;
        private DispatcherTimer _timerforexecuteTC;
        private double _windowFullWidth = 0;
        private double _windowMainGridWidth = 400;
        private double _windowRightGridWidth = 400;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMainWindow"/> class.
        /// </summary>
        /// <param name="eventInvite">The event invite.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        public EmailMainWindow(EventInvite eventInvite, string firstName, string lastName)
        {
            KeyValueCollection kvpUserData = new KeyValueCollection();
            try
            {
                isNeedToNotifyRefresh = true;
                logger.Info("EmailMainWindow constructor Event Invite");
                SetWindowUi();
                interactionTypes = InteractionTypes.EventInvite;
                this.firstName = firstName;
                this.lastName = lastName;
                // Get InteractionID
                _interactionID = eventInvite.Interaction.InteractionId;
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                    ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).NotifyWorkbinContentChanged(_interactionID, false);
                // Get ContactID
                if (eventInvite.Interaction.InteractionUserData.Contains("ContactId"))
                    _contactID = eventInvite.Interaction.InteractionUserData["ContactId"].ToString();
                //Display Contact name
                lblTitleStatus.Text = firstName + " " + lastName + " - " + "Agent Interaction Desktop";
                lbltabCustomerName.Text = firstName + " " + lastName;
                lblCustomerName.Text = firstName + " " + lastName;

                if (eventInvite.Interaction.InteractionUserData != null)
                    kvpUserData = eventInvite.Interaction.InteractionUserData;
                switch (eventInvite.Interaction.InteractionType)
                {
                    case "Inbound": lblEmailIxnType.Text = "Inbound";
                        InboundSaveContent.Text = "Save to " + (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress") ? (string)ConfigContainer.Instance().GetValue("workbin.email.in-progress") : "workbin");
                        panelInbound.Visibility = Visibility.Visible;
                        panelOutbound.Visibility = Visibility.Collapsed;
                        inboundUserControl = new InboundUserControl(eventInvite, GetContentFromUCS(eventInvite.Interaction.InteractionId));
                        if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text) || !string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                            btnReplyAll.Visibility = Visibility.Visible;
                        dockEmailContent.Children.Clear();
                        dockEmailContent.Children.Add(inboundUserControl);
                        break;
                    case "Outbound": lblEmailIxnType.Text = "Outbound";
                        OutboundSaveContent.Text = "Save to " + (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft") ? (string)ConfigContainer.Instance().GetValue("workbin.email.draft") : "workbin");
                        panelInbound.Visibility = Visibility.Collapsed;
                        panelOutbound.Visibility = Visibility.Visible;
                        outboundUserControl = new OutboundUserControl(this, GetContentFromUCS(_interactionID), true, firstName + " " + lastName, false);
                        if (kvpUserData.AllKeys.Contains("InteractionSubtype") &&
                            string.Compare(kvpUserData["InteractionSubtype"].ToString(), "OutboundReply") == 0)
                        {
                            _parentIxnID = eventGetInteractionContent.InteractionAttributes.ParentId;
                        }
                        dockEmailContent.Children.Clear();
                        dockEmailContent.Children.Add(outboundUserControl);
                        break;
                    default: break;
                }
                dataUserControl = new DataUserControl(kvpUserData, eventGetInteractionContent != null ?
               (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.TheComment) ?
                           eventGetInteractionContent.InteractionAttributes.TheComment : string.Empty) : string.Empty, _interactionID);
                if (dataUserControl != null)
                {
                    gridDataResHistory.Children.Clear();
                    gridDataResHistory.Children.Add(dataUserControl);
                    btnData.IsChecked = true;
                    Width = _windowMainGridWidth + _windowRightGridWidth + 16 + colEmailOptions.Width.Value;
                    MinWidth = Width;
                }
                if (!EmailDataContext.GetInstance().IsContactServerActive)
                {
                    panelInbound.Visibility = panelOutbound.Visibility = Visibility.Collapsed;
                    isContactDatatobeUpdated = true;
                    btnPrint.Visibility = Visibility.Collapsed;
                    if (dataUserControl != null)
                        dataUserControl.tbItemNotes.Visibility = Visibility.Collapsed;
                }
                this.Activate();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at EmailMainWindow constructor" + ex.ToString());
                grdrow_error.Height = new GridLength(0);
                this.Close();
            }
            finally
            {
                kvpUserData = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMainWindow"/> class.
        /// </summary>
        /// <param name="eventPulledInteractions">The event pulled interactions.</param>
        public EmailMainWindow(EventPulledInteractions eventPulledInteractions)
        {
            KeyValueCollection kvpUserData = new KeyValueCollection();
            string contactName;
            try
            {
                SetWindowUi();
                InboundSaveContent.Text = "Put back in original location";
                OutboundSaveContent.Text = "Put back in original location";
                interactionTypes = InteractionTypes.EventPull;
                if (eventPulledInteractions.Interactions != null && eventPulledInteractions.Interactions.Count > 0)
                {
                    string[] keys = eventPulledInteractions.Interactions.AllKeys;
                    _interactionID = keys[0];
                }
                kvpUserData = eventPulledInteractions.Interactions[_interactionID] as KeyValueCollection;
                if (kvpUserData.Contains("ContactId"))
                    _contactID = kvpUserData["ContactId"].ToString();
                contactName = GetContactName();

                switch (kvpUserData["InteractionType"].ToString())
                {
                    case "Inbound":
                        lblEmailIxnType.Text = "Inbound";
                        if (string.IsNullOrEmpty(contactName) && kvpUserData.Contains("FromAddress") && !string.IsNullOrEmpty(kvpUserData["FromAddress"].ToString()))
                            contactName = kvpUserData["FromAddress"].ToString();
                        panelInbound.Visibility = Visibility.Visible;
                        panelOutbound.Visibility = Visibility.Collapsed;
                        inboundUserControl = new InboundUserControl(kvpUserData, GetContentFromUCS(_interactionID));
                        if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text) || !string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                            btnReplyAll.Visibility = Visibility.Visible;
                        dockEmailContent.Children.Clear();
                        dockEmailContent.Children.Add(inboundUserControl);
                        break;
                    case "Outbound":
                        lblEmailIxnType.Text = "Outbound";
                        if (string.IsNullOrEmpty(contactName) && kvpUserData.Contains("To") && !string.IsNullOrEmpty(kvpUserData["To"].ToString()))
                            contactName = kvpUserData["To"].ToString();
                        panelInbound.Visibility = Visibility.Collapsed;
                        panelOutbound.Visibility = Visibility.Visible;
                        outboundUserControl = new OutboundUserControl(this, GetContentFromUCS(_interactionID), true, contactName, false);
                        if (kvpUserData["InteractionSubtype"].ToString() == "OutboundReply")
                        {
                            _parentIxnID = eventGetInteractionContent.InteractionAttributes.ParentId;
                        }
                        dockEmailContent.Children.Clear();
                        dockEmailContent.Children.Add(outboundUserControl);
                        break;
                }

                lblTitleStatus.Text = contactName + " - " + "Agent Interaction Desktop";
                lbltabCustomerName.Text = contactName;
                lblCustomerName.Text = contactName;

                //User Data
                //_eventPulledInteractions.Interactions
                _isWindowSizeChanged = false;
                dataUserControl = new DataUserControl(kvpUserData, eventGetInteractionContent != null ?
               (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.TheComment) ?
                           eventGetInteractionContent.InteractionAttributes.TheComment : string.Empty) : string.Empty, _interactionID);
                if (dataUserControl != null)
                {
                    gridDataResHistory.Children.Clear();
                    gridDataResHistory.Children.Add(dataUserControl);
                    btnData.IsChecked = true;
                    Width = _windowMainGridWidth + _windowRightGridWidth + 16 + colEmailOptions.Width.Value;
                    MinWidth = Width;
                }
                this.Activate();
                UpdatePendingStatus(true);
            }
            catch (Exception ex)
            {
                //logger.Error("Error occurred at EmailMainWindow constructor" + ex.ToString());
                //this.Close();
                throw ex;
            }
            finally
            {
                contactName = null;
                kvpUserData = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMainWindow"/> class.
        /// </summary>
        /// <param name="parentixnID">The parentixn identifier.</param>
        /// <param name="isReplyALL">if set to <c>true</c> [is reply all].</param>
        public EmailMainWindow(string parentixnID, bool isReplyALL)
        {
            InteractionService interactionService = new InteractionService();
            EventInteractionProperties eventInteractionProperties;
            try
            {
                SetWindowUi();
                interactionTypes = InteractionTypes.CreateReply;
                _parentIxnID = parentixnID;
                Pointel.Interactions.Core.Common.OutputValues output = interactionService.GetInteractionProperties(EmailDataContext.GetInstance().ProxyClientID, _parentIxnID);
                if (output.MessageCode == "200" && output.IMessage != null)
                {
                    eventInteractionProperties = (EventInteractionProperties)output.IMessage;
                    ReplyorReplyALL(_parentIxnID, GetUserData(eventInteractionProperties.Interaction.InteractionUserData), isReplyALL);
                }
                else
                {
                    GetContentFromUCS(parentixnID);
                    ReplyorReplyALL(_parentIxnID, GetUserData(eventGetInteractionContent.InteractionAttributes.AllAttributes), isReplyALL);
                }
                this.Activate();
                UpdatePendingStatus(true);
                isNeedToNotifyRefresh = true;

                if (dataUserControl != null)
                {
                    Width = _windowMainGridWidth + _windowRightGridWidth + 16 + colEmailOptions.Width.Value;
                    MinWidth = Width;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at EmailMainWindow constructor" + ex.ToString());
                grdrow_error.Height = new GridLength(0);
                this.Close();
            }
            finally
            {
                interactionService = null;
                eventInteractionProperties = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMainWindow"/> class.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="contactID">The contact identifier.</param>
        public EmailMainWindow(string emailAddress, string contactID, string outboundInteractionID = null)
        {
            InteractionService interactionService = new InteractionService();
            KeyValueCollection kvpUserData = new KeyValueCollection();
            try
            {
                SetWindowUi();
                interactionTypes = InteractionTypes.Compose;
                if (!string.IsNullOrEmpty(contactID))
                    _contactID = contactID;
                else
                {
                    KeyValueCollection otherFields = new KeyValueCollection();
                    otherFields.Add("To", emailAddress);
                    otherFields.Add("EmailAddress", emailAddress);
                    Dictionary<ContactDetails, string> _contactdetails = null;
                    _contactdetails = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(EmailDataContext.GetInstance().TenantDbId.ToString(), MediaTypes.Email, otherFields);
                    if (_contactdetails != null)
                    {
                        _contactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                    }

                }
                string contactName = GetContactName();
                if (string.IsNullOrEmpty(contactName))
                    contactName = emailAddress;
                lblTitleStatus.Text = contactName + " - " + "Agent Interaction Desktop";
                lbltabCustomerName.Text = contactName;
                lblCustomerName.Text = contactName;

                if (EmailDataContext.GetInstance().MailBox != null && EmailDataContext.GetInstance().MailBox.Count > 0)
                    kvpUserData.Add("FromAddress", EmailDataContext.GetInstance().MailBox[0].Tag.ToString());
                else
                    ShowError("Action Aborted: From Address is missing");
                kvpUserData.Add("To", emailAddress);
                kvpUserData.Add("ContactId", _contactID);
                if (outboundInteractionID != null)
                {
                    KeyValueCollection kvpOutboundUserData = new KeyValueCollection();
                    Pointel.Interactions.Core.Common.OutputValues output = interactionService.GetInteractionProperties(EmailDataContext.GetInstance().ProxyClientID, _parentIxnID);
                    if (output.MessageCode == "200" && output.IMessage != null)
                    {
                        EventInteractionProperties eventInteractionProperties = (EventInteractionProperties)output.IMessage;
                        if (eventInteractionProperties.Interaction.InteractionUserData != null)
                        {
                            kvpOutboundUserData = eventInteractionProperties.Interaction.InteractionUserData;
                        }
                    }
                    GetContentFromUCS(outboundInteractionID);
                    if (kvpOutboundUserData.Count == 0 && eventGetInteractionContent.InteractionAttributes.AllAttributes != null)
                        kvpOutboundUserData = eventGetInteractionContent.InteractionAttributes.AllAttributes;

                    if (kvpOutboundUserData.Count > 0)
                    {
                        foreach (var key in kvpOutboundUserData.AllKeys)
                        {
                            if (!(kvpUserData.Contains(key)))
                                kvpUserData.Add(key, kvpOutboundUserData[key]);
                        }
                    }
                }

                _interactionID = interactionService.SubmitNewInteraction(EmailDataContext.GetInstance().TenantDbId, EmailDataContext.GetInstance().ProxyClientID, (ConfigContainer.Instance().AllKeys.Contains("email.default-queue") ? (string)ConfigContainer.Instance().GetValue("email.default-queue") : string.Empty), kvpUserData);
                if (!string.IsNullOrEmpty(_interactionID))
                {
                    lblEmailIxnType.Text = "Outbound";
                    OutboundSaveContent.Text = "Save to " + (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft") ? (string)ConfigContainer.Instance().GetValue("workbin.email.draft") : "workbin");
                    panelInbound.Visibility = Visibility.Collapsed;
                    panelOutbound.Visibility = Visibility.Visible;
                    if (!string.IsNullOrEmpty(outboundInteractionID))
                        outboundUserControl = new OutboundUserControl(this, emailAddress, contactName, GetSignature(), eventGetInteractionContent);
                    else
                        outboundUserControl = new OutboundUserControl(this, emailAddress, contactName, GetSignature());
                    dockEmailContent.Children.Clear();
                    dockEmailContent.Children.Add(outboundUserControl);

                    dataUserControl = new DataUserControl(kvpUserData, string.Empty, _interactionID);
                    if (dataUserControl != null)
                    {
                        gridDataResHistory.Children.Clear();
                        gridDataResHistory.Children.Add(dataUserControl);
                        btnData.IsChecked = true;
                        Width = _windowMainGridWidth + _windowRightGridWidth + 16 + colEmailOptions.Width.Value;
                        MinWidth = Width;
                    }
                    InsertOutboundInteraction(false);
                }
                _isWindowSizeChanged = false;
                this.Activate();
                UpdatePendingStatus(true);
                isNeedToNotifyRefresh = true;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at EmailMainWindow constructor" + ex.ToString());
                grdrow_error.Height = new GridLength(0);
                this.Close();
            }
        }

        #endregion Constructors

        #region Enumerations

        enum EmailWindowState
        {
            Normal, Minimized, Maximized
        }

        //Interaction Type
        enum InteractionTypes
        {
            EventInvite,
            EventPull,
            CreateReply,
            Compose
        }

        #endregion Enumerations

        #region Properties

        public Importer ImportClass
        {
            get
            {
                _importClass = _importClass ?? new Importer();
                return _importClass;
            }
        }

        public string InteractionID
        {
            get
            {
                return _interactionID;
            }
        }

        public string ParentInteractionID
        {
            get
            {
                return _parentIxnID;
            }
        }

        #endregion Properties

        #region Methods

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        /// <summary>
        /// Closes the error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void CloseError(object sender, EventArgs e)
        {
            try
            {
                grdrow_error.Height = new GridLength(0);
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CloseSignatureError() : " + ex.Message);
            }
        }

        /// <summary>
        /// Contacts the updation.
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="contactId">The contact identifier.</param>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns>System.String.</returns>
        public string ContactUpdation(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList attributeList)
        {
            string contactName = string.Empty;
            try
            {
                if (operationType == "Update")
                {
                    if (contactId.Equals(_contactID))
                    {
                        AttributesHeader firstNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                        if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                        {
                            contactName += firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                        }
                        AttributesHeader LastNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                        if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                        {
                            contactName += LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                        }

                        lblTitleStatus.Text = contactName + " - " + "Agent Interaction Desktop";
                        lbltabCustomerName.Text = contactName;
                        lblCustomerName.Text = contactName;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at  ContactUpdation" + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets the content from ucs.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <returns>EventGetInteractionContent.</returns>
        public EventGetInteractionContent GetContentFromUCS(string interactionID)
        {
            eventGetInteractionContent = null;
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
            {
                IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetInteractionContent(interactionID, true);

                if (response != null)
                {
                    switch (response.Id)
                    {
                        case EventGetInteractionContent.MessageId:
                            eventGetInteractionContent = (EventGetInteractionContent)response;
                            break;

                        case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                            Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError eventError = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)response;
                            if (eventError != null)
                            {
                                eventGetInteractionContent = null;
                                logger.Trace(eventError.ToString());
                            }
                            else
                            {
                                logger.Warn("EventError Occurred while doing GetInteractionContent()");
                            }
                            break;
                    }
                }
            }
            return eventGetInteractionContent;
        }

        /// <summary>
        /// Gets the user data.
        /// </summary>
        /// <param name="inboundUserData">The inbound user data.</param>
        /// <returns>KeyValueCollection.</returns>
        public KeyValueCollection GetUserData(KeyValueCollection inboundUserData)
        {
            KeyValueCollection userdata = new KeyValueCollection();
            foreach (var key in inboundUserData.AllKeys)
            {
                switch (key)
                {
                    case "To":
                        if (EmailDataContext.GetInstance().MailBox != null && EmailDataContext.GetInstance().MailBox.Count > 0)
                            userdata.Add("FromAddress", EmailDataContext.GetInstance().MailBox[0].Tag.ToString());
                        else
                            userdata.Add("FromAddress", string.Empty);
                        break;
                    case "FromAddress":
                        userdata.Add("To", inboundUserData["FromAddress"]);
                        break;
                    case "EmailAddress":
                        userdata.Add("EmailAddress", inboundUserData["To"]);
                        break;
                    case "Email":
                        userdata.Add("Email", inboundUserData["To"]);
                        break;
                    case "Subject":
                        string replyPefix = "Re:<SPACE>";
                        if (ConfigContainer.Instance().AllKeys.Contains("email.reply-prefix") && !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("email.reply-prefix").Trim()))
                            replyPefix = ConfigContainer.Instance().GetAsString("email.reply-prefix").Trim();

                        replyPefix = replyPefix.Replace("<SPACE>", " ");

                        if (!(inboundUserData["Subject"].ToString().StartsWith(replyPefix)))
                            userdata.Add("Subject", replyPefix + inboundUserData["Subject"]);
                        else
                            userdata.Add("Subject", inboundUserData["Subject"]);
                        break;
                    case "FromPersonal": break;
                    default: userdata.Add(key, inboundUserData[key]);
                        break;
                }
            }
            return userdata;
        }

        /// <summary>
        /// Inserts the outbound interaction.
        /// </summary>
        /// <param name="isReply">if set to <c>true</c> [is reply].</param>
        /// <param name="isSend">if set to <c>true</c> [is send].</param>
        public void InsertOutboundInteraction(bool isReply, bool isSend = false)
        {
            try
            {
                InteractionAttributes interactionAttributes = new InteractionAttributes()
                {
                    TenantId = new NullableInt(EmailDataContext.GetInstance().TenantDbId),
                    Id = _interactionID,
                    EntityTypeId = new NullableEntityTypes(EntityTypes.EmailOut),
                    CreatorAppId = new NullableInt(ConfigContainer.Instance().ApplicationDbId),
                    MediaTypeId = "email",
                    TypeId = "Outbound",
                    Status = new NullableStatuses(Statuses.InProcess),
                    Subject = outboundUserControl.txtOutboundSubject.Text,
                    StartDate = new NullableDateTime(DateTime.UtcNow),
                    AllAttributes = dataUserControl.CurrentData,
                    ContactId = _contactID,
                    TheComment = dataUserControl.notes
                };

                // Owner ID functionality
                if (!isSend || (isSend && !(ConfigContainer.Instance().AllKeys.Contains("email.enable.set-ownerid-on-send") && ((string)ConfigContainer.Instance().GetValue("email.enable.set-ownerid-on-send")).ToLower().Equals("false"))))
                    interactionAttributes.OwnerId = eventGetInteractionContent != null ? (eventGetInteractionContent.InteractionAttributes.OwnerId == null ? new NullableInt(ConfigContainer.Instance().PersonDbId) : eventGetInteractionContent.InteractionAttributes.OwnerId) : new NullableInt(ConfigContainer.Instance().PersonDbId);
                else if (isSend && (ConfigContainer.Instance().AllKeys.Contains("email.enable.set-ownerid-on-send") && ((string)ConfigContainer.Instance().GetValue("email.enable.set-ownerid-on-send")).ToLower().Equals("true")))
                    interactionAttributes.OwnerId = new NullableInt(ConfigContainer.Instance().PersonDbId);
                else if (eventGetInteractionContent.InteractionAttributes.OwnerId == null)
                    interactionAttributes.OwnerId = new NullableInt(ConfigContainer.Instance().PersonDbId);

                // Timeshift functionality
                //example timeshift(minutes) = -330 if DateTime.Now.ToLocalTime().ToString("zzz") returns -5:30
                var hor = Convert.ToInt16(DateTime.Now.ToLocalTime().ToString("zzz").Split(':')[0]) * 60;
                var min = hor >= 0 ? hor + Convert.ToInt16(DateTime.Now.ToLocalTime().ToString("zzz").Split(':')[1]) : hor - Convert.ToInt16(DateTime.Now.ToLocalTime().ToString("zzz").Split(':')[1]);
                interactionAttributes.Timeshift = min;

                EmailOutEntityAttributes emailOutEntityAttributes = new EmailOutEntityAttributes()
                {
                    FromAddress = outboundUserControl.txtOutboundFrom.Text,
                    ToAddresses = outboundUserControl.txtOutboundTo.Text.Replace(";", ","),
                    CcAddresses = outboundUserControl.txtOutboundCc.Text.Replace(";", ","),
                    BccAddresses = outboundUserControl.txtOutboundBcc.Text.Replace(";", ",")
                };

                if (isReply)
                {
                    interactionAttributes.ParentId = _parentIxnID;
                    interactionAttributes.CanBeParent = new NullableBool(true);
                    interactionAttributes.SubtypeId = "OutboundReply";
                }
                else
                {
                    interactionAttributes.SubtypeId = "OutboundNew";
                }
                outboundUserControl.htmlEditor.GetContent();
                InteractionContent interactionContent = new InteractionContent()
                {
                    Text = outboundUserControl.htmlEditor.RTBTextContent,
                    StructuredText = outboundUserControl.htmlEditor.RTBContent,
                    StructTextMimeType = "text/html; charset=utf-8"
                };
                ContactServerHelper.InsertInteraction(interactionContent, interactionAttributes, (BaseEntityAttributes)emailOutEntityAttributes);

            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at InsertOutboundInteraction method " + exception.ToString());
            }
        }

        public void NotifyVoiceChennalState(bool chennalAvailable)
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                btnCustomerDetails.IsEnabled = chennalAvailable;
            }));
        }

        /// <summary>
        /// Opens the inbound mail.
        /// </summary>
        /// <param name="eventpullixnIds">The eventpullixn ids.</param>
        public void OpenInboundMail(EventPulledInteractions eventpullixnIds)
        {
            try
            {
                totalfilelength = 0;
                interactionTypes = InteractionTypes.EventPull;
                KeyValueCollection kvpUserData = new KeyValueCollection();

                string[] keys = eventpullixnIds.Interactions.AllKeys;
                _interactionID = keys[0];

                kvpUserData = eventpullixnIds.Interactions[_interactionID] as KeyValueCollection;
                if (kvpUserData.Contains("ContactId"))
                    _contactID = kvpUserData["ContactId"].ToString();

                string contactName = GetContactName();
                lblTitleStatus.Text = contactName + " - " + "Agent Interaction Desktop";
                lbltabCustomerName.Text = contactName;
                lblCustomerName.Text = contactName;
                InboundSaveContent.Text = "Put back in original location";
                lblEmailIxnType.Text = "Inbound";
                lblTabItemShowTimer.Text = "[00:00:00]";
                panelInbound.Visibility = Visibility.Visible;
                panelOutbound.Visibility = Visibility.Collapsed;
                outboundUserControl = null;
                inboundUserControl = new InboundUserControl(kvpUserData, GetContentFromUCS(_interactionID));
                btnReplyAll.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text) || !string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                    btnReplyAll.Visibility = Visibility.Visible;
                dockEmailContent.Children.Clear();
                dockEmailContent.Children.Add(inboundUserControl);

                dataUserControl = null;
                dataUserControl = new DataUserControl(kvpUserData, eventGetInteractionContent != null ?
                                 (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.TheComment) ?
                                    eventGetInteractionContent.InteractionAttributes.TheComment : string.Empty) : string.Empty,
                                    _interactionID);
                if (dataUserControl != null)
                {
                    gridDataResHistory.Children.Clear();
                    gridDataResHistory.Children.Add(dataUserControl);
                    btnData.IsChecked = true;
                    btnContacts.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnResponses.IsChecked = false;
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));

                }
                CloseError(null, null);
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at OpenInboundMail method " + exception.ToString());
            }
        }

        /// <summary>
        /// Replyors the reply all.
        /// </summary>
        /// <param name="parentixnID">The parentixn identifier.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="isReplyAll">if set to <c>true</c> [is reply all].</param>
        public void ReplyorReplyALL(string parentixnID, KeyValueCollection userData, bool isReplyAll)
        {
            try
            {
                OutboundSaveContent.Text = "Save to " + (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft") ? (string)ConfigContainer.Instance().GetValue("workbin.email.draft") : "workbin");
                InteractionService interactionService = new InteractionService();
                string createNewIxnID = interactionService.SubmitReplyInteraction(EmailDataContext.GetInstance().TenantDbId, EmailDataContext.GetInstance().ProxyClientID, (ConfigContainer.Instance().AllKeys.Contains("email.default-queue") ? (string)ConfigContainer.Instance().GetValue("email.default-queue") : string.Empty), parentixnID, userData);
                if (!string.IsNullOrEmpty(createNewIxnID) && !(createNewIxnID.StartsWith("Error:")))
                {
                    _interactionID = createNewIxnID;
                    interactionTypes = InteractionTypes.CreateReply;
                    if (userData.Contains("ContactId"))
                    {
                        _contactID = userData["ContactId"].ToString();
                        string contactName = GetContactName();
                        lblTitleStatus.Text = contactName + " - " + "Agent Interaction Desktop";
                        lbltabCustomerName.Text = contactName;
                        lblCustomerName.Text = contactName;
                    }
                    _parentIxnID = parentixnID;
                    lblEmailIxnType.Text = "Outbound";
                    panelInbound.Visibility = Visibility.Collapsed;
                    panelOutbound.Visibility = Visibility.Visible;
                    outboundUserControl = new OutboundUserControl(this, GetContentFromUCS(_parentIxnID), isReplyAll, firstName + " " + lastName, true, GetSignature());
                    dockEmailContent.Children.Clear();
                    dockEmailContent.Children.Add(outboundUserControl);
                    inboundUserControl = null;
                    dataUserControl = null;
                    dataUserControl = new DataUserControl(userData, string.Empty, _interactionID);
                    gridDataResHistory.Children.Clear();
                    gridDataResHistory.Children.Add(dataUserControl);

                    btnData.IsChecked = true;
                    btnContacts.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnResponses.IsChecked = false;
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                    outboundUserControl.htmlEditor.Focus();
                    InsertOutboundInteraction(true);
                }
                else
                {
                    grdrow_error.Height = GridLength.Auto;
                    ShowError(createNewIxnID.Substring(6));
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at ReplyorReplyALL method " + exception.ToString());
            }
        }

        /// <summary>
        /// Saves the inbound mail.
        /// </summary>
        /// <param name="isClose">if set to <c>true</c> [is close].</param>
        public void SaveInboundMail(bool isClose)
        {
            try
            {
                AddorUpdateCaseDatatoIxn();
                Pointel.Interactions.Core.Common.OutputValues OutputValues;
                InteractionService interactionService = new InteractionService();
                if (interactionTypes == InteractionTypes.EventInvite || isClose)
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress")))
                        OutputValues = interactionService.PlaceInWorkbin(_interactionID, EmailDataContext.GetInstance().AgentID,
                                                                        (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress") ?
                                                                        (string)ConfigContainer.Instance().GetValue("workbin.email.in-progress") : string.Empty),
                                                                        EmailDataContext.GetInstance().ProxyClientID);
                    else
                    {
                        ShowError("Action Aborted : \"workbin.email.in-progress\" option is not defined");
                        return;
                    }

                }
                else
                    OutputValues = interactionService.PlaceInQueue(_interactionID, EmailDataContext.GetInstance().ProxyClientID, "__BACK__");
                if (OutputValues.MessageCode == "200")
                {
                    ContactServerHelper.UpdateInteraction(_interactionID, eventGetInteractionContent != null ?
                                                                                (eventGetInteractionContent.InteractionAttributes.OwnerId == null ?
                                                                                ConfigContainer.Instance().PersonDbId : 0)
                                                                                : 0,
                                                                                dataUserControl.notes, dataUserControl.CurrentData, 2);
                    //if (EmailDataContext.GetInstance().MessageToClientEmail != null)
                    //{
                    //    UpdatePendingStatus(false);
                    //}
                    _isSaveMailToWorkbin = false;
                    grdrow_error.Height = new GridLength(0);
                    this.Close();
                }
                else
                {
                    grdrow_error.Height = GridLength.Auto;
                    ShowError(OutputValues.Message.ToString());
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at SaveInboundMail method " + exception.ToString());
            }
        }

        public void SaveOutboundEmail(bool isClose, bool isSaveinWorkbin)
        {
            try
            {
                if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundFrom.Text))
                {
                    if (ValidateEmailAddress())
                    {
                        if (outboundUserControl.colAttachError.Width == new GridLength(0))
                        {
                            AddorUpdateCaseDatatoIxn();
                            if (isSaveinWorkbin)
                                SaveInWorkbin(isClose);
                            else
                            {
                                UpdateOutboundInteraction(2);
                                AddorDeleteAttachments();
                            }
                        }
                        else
                            ShowError("Maximum upload limit is restricted to " + EmailServerDetails.EmailMaxAttachmentSize() + " MB.");
                    }
                }
                else
                    ShowError("Action Aborted: From Address is missing");

            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at SaveOutboundEmail method " + exception.ToString());
            }
        }

        /// <summary>
        /// Saves the in workbin.
        /// </summary>
        /// <param name="isclose">if set to <c>true</c> [isclose].</param>
        private void SaveInWorkbin(bool isClose)
        {
            try
            {
                if (interactionTypes == InteractionTypes.EventPull && !isClose)
                {
                    InteractionService interactionService = new InteractionService();
                    Pointel.Interactions.Core.Common.OutputValues OutputValues = interactionService.PlaceInQueue(_interactionID, EmailDataContext.GetInstance().ProxyClientID, "__BACK__");
                    if (OutputValues.MessageCode == "200")
                    {
                        UpdateOutboundInteraction(2);
                        AddorDeleteAttachments();
                        //UpdatePendingStatus(false);
                        _isSaveMailToWorkbin = false;
                        grdrow_error.Height = new GridLength(0);
                        this.Close();
                    }
                    else
                    {
                        grdrow_error.Height = GridLength.Auto;
                        ShowError(OutputValues.Message.ToString());
                    }
                }
                else
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("workbin.email.draft")))
                    {
                        InteractionService interactionService = new InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues OutputValues = interactionService.PlaceInWorkbin(_interactionID, EmailDataContext.GetInstance().AgentID,
                            // EmailDataContext.GetInstance().PlaceName,
                            (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft") ? (string)ConfigContainer.Instance().GetValue("workbin.email.draft") : string.Empty), EmailDataContext.GetInstance().ProxyClientID);
                        if (OutputValues.MessageCode == "200")
                        {
                            //if (interactionTypes == InteractionTypes.CreateReply)
                            //    InsertOutboundInteraction(true);
                            //else
                            UpdateOutboundInteraction(2);
                            AddorDeleteAttachments();
                            //UpdatePendingStatus(false);
                            _isSaveMailToWorkbin = false;
                            grdrow_error.Height = new GridLength(0);
                            this.Close();
                        }
                        else
                        {
                            grdrow_error.Height = GridLength.Auto;
                            ShowError(OutputValues.Message.ToString());
                        }
                    }
                    else
                        ShowError("Action Aborted : \"workbin.email.draft\" option is not defined");
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at SaveOutboundEmail method " + exception.ToString());
            }
        }


        /// <summary>
        /// Sets the window UI.
        /// </summary>
        public void SetWindowUi()
        {
            InitializeComponent();
            this.DataContext = _emailDetails;
            WindowResizer winResize = new WindowResizer(this);
            winResize.addResizerDown(BottomSideRect);
            winResize.addResizerRight(RightSideRect);
            winResize.addResizerRightDown(RightbottomSideRect);
            winResize = null;
            _isWindowSizeChanged = false;
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="errMsg">The error MSG.</param>
        public void ShowError(string errMsg)
        {
            if (string.IsNullOrEmpty(errMsg)) return;
            lblErrMsg.Text = errMsg;
            grdrow_error.Height = GridLength.Auto;
            starttimerforerror();
        }

        /// <summary>
        /// Updates the outbound interaction.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="isSend">if set to <c>true</c> [is send].</param>
        public void UpdateOutboundInteraction(int status, bool isSend = false)
        {
            try
            {
                if (outboundUserControl != null)
                {
                    InteractionAttributes interactionAttributes = new InteractionAttributes()
                    {
                        TenantId = new NullableInt(EmailDataContext.GetInstance().TenantDbId),
                        EntityTypeId = new NullableEntityTypes(EntityTypes.EmailOut),
                        Id = _interactionID,
                        CreatorAppId = new NullableInt(ConfigContainer.Instance().ApplicationDbId),
                        Status = status == 2 ? new NullableStatuses(Statuses.InProcess) : new NullableStatuses(Statuses.Stopped),
                        Subject = outboundUserControl.txtOutboundSubject.Text,
                        AllAttributes = dataUserControl.CurrentData,
                        TheComment = dataUserControl.notes,
                        ContactId = _contactID,

                        //StartDate = outboundUserControl.startDate
                    };

                    if (isSend && (ConfigContainer.Instance().AllKeys.Contains("email.enable.set-ownerid-on-send") && ((string)ConfigContainer.Instance().GetValue("email.enable.set-ownerid-on-send")).ToLower().Equals("true")))
                        interactionAttributes.OwnerId = new NullableInt(ConfigContainer.Instance().PersonDbId);

                    EmailOutEntityAttributes emailOutEntityAttributes = new EmailOutEntityAttributes()
                    {
                        FromAddress = outboundUserControl.txtOutboundFrom.Text.Replace(";", ","),
                        ToAddresses = outboundUserControl.txtOutboundTo.Text.Replace(";", ","),
                        CcAddresses = outboundUserControl.txtOutboundCc.Text.Replace(";", ","),
                        BccAddresses = outboundUserControl.txtOutboundBcc.Text.Replace(";", ",")
                    };
                    outboundUserControl.htmlEditor.GetContent();
                    InteractionContent interactionContent = new InteractionContent()
                    {
                        Text = outboundUserControl.htmlEditor.RTBTextContent,
                        StructuredText = outboundUserControl.htmlEditor.RTBContent,
                        StructTextMimeType = "text/html; charset=utf-8"
                    };
                    ContactServerHelper.UpdateInteraction(interactionContent, interactionAttributes,
                        (BaseEntityAttributes)emailOutEntityAttributes);
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at UpdateOutboundInteraction method " + exception.ToString());
            }
        }

        /// <summary>
        /// Validates the email address.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ValidateEmailAddress()
        {
            bool isValidate = false;
            if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundTo.Text)) // Check To is empty or not
            {
                if (ValidateEmailAddress(outboundUserControl.txtOutboundTo.Text)) // Check to address is valid or not
                {
                    if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundSubject.Text)) // Check subject is empty or not
                    {
                        if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundCc.Text)) // Check CC is null or empty
                        {
                            if (!ValidateEmailAddress(outboundUserControl.txtOutboundCc.Text)) // Check CC address is valid or not
                            {
                                ShowError("Invalid Email ID " + outboundUserControl.txtOutboundCc.Text + " found at Cc field");
                                grdrow_error.Height = GridLength.Auto;
                                return false;
                            }
                            else
                                isValidate = true;
                        }
                        if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundBcc.Text)) // Check BCC is null or empty
                        {
                            if (!ValidateEmailAddress(outboundUserControl.txtOutboundBcc.Text)) // Check BCC address is valid or not
                            {
                                ShowError("Invalid Email ID " + outboundUserControl.txtOutboundBcc.Text + " found at Bcc field");
                                grdrow_error.Height = GridLength.Auto;
                                return false;
                            }
                            else
                                isValidate = true;
                        }
                        else
                            return true;

                    }
                    else
                    {
                        ShowError("Subject field cannot be empty.");
                        grdrow_error.Height = GridLength.Auto;
                        return false;
                    }
                }
                else
                {
                    ShowError("Invalid Email ID " + outboundUserControl.txtOutboundTo.Text + " found at To field");
                    grdrow_error.Height = GridLength.Auto;
                    return false;
                }
            }
            else
            {
                ShowError("To address field cannot be empty");
                grdrow_error.Height = GridLength.Auto;
                return false;
            }
            return isValidate;
        }

        /// <summary>
        /// Validates the email address.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ValidateEmailAddress(string email)
        {
            string regex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9][\-a-zA-Z0-9]{0,22}[a-zA-Z0-9]))$";

            if (ConfigContainer.Instance().AllKeys.Contains("email.validate-expression") && !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("email.validate-expression")))
                regex = ConfigContainer.Instance().GetAsString("email.validate-expression");

            email = email.Replace(";", ",");
            string[] emailAddress = (email.EndsWith(",") ? email.Remove(email.Length - 1) : email).Split(',');
            bool status = true;
            foreach (string address in emailAddress)
            {
                status = System.Text.RegularExpressions.Regex.IsMatch(address.TrimStart().TrimEnd(), regex);
                if (!status)
                    break;
                status = (address.Length - (address.IndexOf("@") + 1) <= 64);
                if (!status)
                    break;
            }
            return status;
        }

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, Int32 uIDEnableItem, Int32 uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        void AddAttachment(string[] filenames)
        {
            var invalidFileError = string.Empty;
            var maxsizeError = string.Empty;
            int emailMaxAttachmentSize = EmailServerDetails.EmailMaxAttachmentSize();
            foreach (var fileName in filenames)
            {
                FileInfo f = new FileInfo(fileName);
                string[] RestrictAttachFileType = ConfigContainer.Instance().AllKeys.Contains("email.restricted-attachment-file-types") ? ((string)ConfigContainer.Instance().GetValue("email.restricted-attachment-file-types")).Replace(" ", "").Split(',') : null;
                if (RestrictAttachFileType != null &&
                    RestrictAttachFileType.Contains(f.Extension.Replace(".", "")) && string.IsNullOrEmpty(invalidFileError))
                    invalidFileError = "File with extensions (" + String.Join(", ", RestrictAttachFileType.Select(p => p.ToString()).ToArray()) + ") is restricted to attach with this mail.";

                if (RestrictAttachFileType != null && !RestrictAttachFileType.Contains(f.Extension.Replace(".", "")))
                {
                    outboundUserControl.AddAttachments(fileName, f.Length);
                    totalfilelength += f.Length;
                    if (emailMaxAttachmentSize > 0 && outboundUserControl.CheckAttachmentSize(false))
                        maxsizeError = "Maximum upload limit is restricted to " + emailMaxAttachmentSize + " MB.";
                }
            }
            var finalError = string.Empty;
            if (!string.IsNullOrEmpty(invalidFileError) && !string.IsNullOrEmpty(maxsizeError))
                finalError = invalidFileError + Environment.NewLine + maxsizeError;
            else
                finalError = invalidFileError + maxsizeError;
            ShowError(finalError);
        }

        /// <summary>
        /// Addors the delete attachments.
        /// </summary>
        private void AddorDeleteAttachments()
        {
            if (outboundUserControl.AddedAttachDocPath != null)
            {
                foreach (var item in outboundUserControl.AddedAttachDocPath)
                {
                    if (!item.Contains("\\"))
                        ContactServerHelper.AddAttachDocument(_interactionID, item, true);
                    else
                        ContactServerHelper.AddAttachDocument(_interactionID, item);
                }
            }
            if (outboundUserControl.DeletedAttachDocId != null)
            {
                foreach (var item in outboundUserControl.DeletedAttachDocId)
                {
                    ContactServerHelper.RemoveAttachDocument(_interactionID, item);
                }
            }
        }

        /// <summary>
        /// Addors the update case datato ixn.
        /// </summary>
        private void AddorUpdateCaseDatatoIxn()
        {
            try
            {
                KeyValueCollection kvpAddCaseData = new KeyValueCollection();
                KeyValueCollection kvpUpdateCaseData = new KeyValueCollection();
                KeyValueCollection kvpDeleteCaseData = new KeyValueCollection();

                switch (lblEmailIxnType.Text.ToString())
                {
                    case "Inbound":
                        if (dataUserControl.CurrentData.ContainsKey("Cc"))
                        {
                            if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text))
                            {
                                kvpUpdateCaseData.Add("Cc", inboundUserControl.txtInboundCc.Text);
                                dataUserControl.CurrentData["Cc"] = inboundUserControl.txtInboundCc.Text;
                            }
                            else
                                kvpDeleteCaseData.Add("Cc", dataUserControl.CurrentData["Cc"]);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text))
                            {
                                kvpAddCaseData.Add("Cc", inboundUserControl.txtInboundCc.Text);
                                dataUserControl.CurrentData.Add("Cc", inboundUserControl.txtInboundCc.Text);
                            }
                        }

                        if (dataUserControl.CurrentData.ContainsKey("Bcc"))
                        {
                            if (!string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                            {
                                kvpUpdateCaseData.Add("Bcc", inboundUserControl.txtInboundBcc.Text);
                                dataUserControl.CurrentData["Bcc"] = inboundUserControl.txtInboundBcc.Text;
                            }
                            else
                                kvpDeleteCaseData.Add("Cc", dataUserControl.CurrentData["Cc"]);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                            {
                                kvpAddCaseData.Add("Bcc", inboundUserControl.txtInboundBcc.Text);
                                dataUserControl.CurrentData.Add("Bcc", inboundUserControl.txtInboundBcc.Text);
                            }
                        }

                        if (dataUserControl.CurrentData.ContainsKey("StartDate"))
                        {
                            kvpUpdateCaseData.Add("StartDate", inboundUserControl.lblInboundDateTime.Text);
                            dataUserControl.CurrentData["StartDate"] = inboundUserControl.lblInboundDateTime.Text;
                        }
                        else
                        {
                            kvpAddCaseData.Add("StartDate", inboundUserControl.lblInboundDateTime.Text);
                            dataUserControl.CurrentData.Add("StartDate", inboundUserControl.lblInboundDateTime.Text);
                        }
                        break;

                    case "Outbound":
                        if (dataUserControl.CurrentData.ContainsKey("FromAddress"))
                        {
                            kvpUpdateCaseData.Add("FromAddress", outboundUserControl.txtOutboundFrom.Text);
                            dataUserControl.CurrentData["FromAddress"] = outboundUserControl.txtOutboundFrom.Text;
                        }
                        else
                        {
                            kvpAddCaseData.Add("FromAddress", outboundUserControl.txtOutboundFrom.Text);
                            dataUserControl.CurrentData.Add("FromAddress", outboundUserControl.txtOutboundFrom.Text);
                        }

                        if (dataUserControl.CurrentData.ContainsKey("Subject"))
                        {
                            kvpUpdateCaseData.Add("Subject", outboundUserControl.txtOutboundSubject.Text);
                            dataUserControl.CurrentData["Subject"] = outboundUserControl.txtOutboundSubject.Text;
                        }
                        else
                        {
                            kvpAddCaseData.Add("Subject", outboundUserControl.txtOutboundSubject.Text);
                            dataUserControl.CurrentData.Add("Subject", outboundUserControl.txtOutboundSubject.Text);
                        }

                        if (dataUserControl.CurrentData.ContainsKey("To"))
                        {
                            kvpUpdateCaseData.Add("To", outboundUserControl.txtOutboundTo.Text);
                            dataUserControl.CurrentData["To"] = outboundUserControl.txtOutboundTo.Text;
                        }
                        else
                        {
                            kvpAddCaseData.Add("To", outboundUserControl.txtOutboundTo.Text.Replace(";", ","));
                            dataUserControl.CurrentData.Add("To", outboundUserControl.txtOutboundTo.Text.Replace(";", ","));
                        }

                        if (dataUserControl.CurrentData.ContainsKey("Cc"))
                        {
                            if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundCc.Text))
                            {
                                kvpUpdateCaseData.Add("Cc", outboundUserControl.txtOutboundCc.Text.Replace(";", ","));
                                dataUserControl.CurrentData["Cc"] = outboundUserControl.txtOutboundCc.Text.Replace(";", ",");
                            }
                            else
                                kvpDeleteCaseData.Add("Cc", dataUserControl.CurrentData["Cc"]);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundCc.Text))
                            {
                                kvpAddCaseData.Add("Cc", outboundUserControl.txtOutboundCc.Text.Replace(";", ","));
                                dataUserControl.CurrentData.Add("Cc", outboundUserControl.txtOutboundCc.Text.Replace(";", ","));
                            }
                        }

                        if (dataUserControl.CurrentData.ContainsKey("Bcc"))
                        {
                            if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundBcc.Text))
                            {
                                kvpUpdateCaseData.Add("Bcc", outboundUserControl.txtOutboundBcc.Text.Replace(";", ","));
                                dataUserControl.CurrentData["Bcc"] = outboundUserControl.txtOutboundBcc.Text.Replace(";", ",");
                            }
                            else
                                kvpDeleteCaseData.Add("Bcc", dataUserControl.CurrentData["Bcc"]);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundBcc.Text))
                            {
                                kvpAddCaseData.Add("Bcc", outboundUserControl.txtOutboundBcc.Text.Replace(";", ","));
                                dataUserControl.CurrentData.Add("Bcc", outboundUserControl.txtOutboundBcc.Text.Replace(";", ","));
                            }
                        }

                        if (dataUserControl.CurrentData.ContainsKey("StartDate"))
                        {
                            kvpUpdateCaseData.Add("StartDate", outboundUserControl.startDate);
                            dataUserControl.CurrentData["StartDate"] = outboundUserControl.startDate;
                        }
                        else
                        {
                            kvpAddCaseData.Add("StartDate", outboundUserControl.startDate);
                            dataUserControl.CurrentData.Add("StartDate", outboundUserControl.startDate);
                        }
                        break;
                }

                InteractionService interactionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = new Core.Common.OutputValues();
                if (kvpAddCaseData.Count > 0)
                {
                    result = interactionService.AddCaseDataProperties(_interactionID, EmailDataContext.GetInstance().ProxyClientID, kvpAddCaseData);
                    if (result.MessageCode == "200")
                    {
                        logger.Info("Case Data added successfully for " + _interactionID);
                    }
                }
                if (kvpUpdateCaseData.Count > 0)
                {
                    result = interactionService.UpdateCaseDataProperties(_interactionID, EmailDataContext.GetInstance().ProxyClientID, kvpUpdateCaseData);
                    if (result.MessageCode == "200")
                    {
                        logger.Info("Case Data updated successfully for " + _interactionID);
                    }
                }
                if (kvpDeleteCaseData.Count > 0)
                {
                    result = interactionService.DeleteCaseDataProperties(_interactionID, EmailDataContext.GetInstance().ProxyClientID, kvpDeleteCaseData);
                    if (result.MessageCode == "200")
                    {
                        logger.Info("Case Data Deleted successfully for " + _interactionID);
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at AddorUpdateCaseDatatoIxn method " + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the borderRecentIXN control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void borderRecentIXN_MouseEnter(object sender, MouseEventArgs e)
        {
            popupRecentIXNDetails.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            popupRecentIXNDetails.PlacementTarget = borderRecentIXN;
            popupRecentIXNDetails.StaysOpen = true;
            popupRecentIXNDetails.Focusable = false;
            popupRecentIXNDetails.IsOpen = true;
        }

        /// <summary>
        /// Handles the MouseLeave event of the borderRecentIXN control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void borderRecentIXN_MouseLeave(object sender, MouseEventArgs e)
        {
            popupRecentIXNDetails.IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the btnAttachment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (outboundUserControl != null)
            {
                if (openFileDialog == null)
                    ConfigOpenFileDialog();
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    AddAttachment(openFileDialog.FileNames);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCloseError control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            CloseError(null, null);
        }

        /// <summary>
        /// Handles the Click event of the btnConsultCall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnConsultCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dialpad != null)
                {
                    dialpad = null;
                }
                dialpad = new DialPad();
                var grid = new Grid();
                grid.Background = System.Windows.Media.Brushes.White;
                grid.Children.Add(dialpad);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = System.Windows.Media.Brushes.Transparent;
                menuConsultItem.Header = grid;
                menuConsultItem.Margin = new Thickness(-12, -1, -18, -3);
                menuConsultItem.Width = Double.NaN;
                EmailDataContext.GetInstance().cmshow.Items.Clear();
                EmailDataContext.GetInstance().cmshow.Items.Add(menuConsultItem);
                EmailDataContext.GetInstance().cmshow.PlacementTarget = btnConsultCall;
                EmailDataContext.GetInstance().cmshow.Placement = PlacementMode.Bottom;
                EmailDataContext.GetInstance().cmshow.IsOpen = true;
                EmailDataContext.GetInstance().cmshow.StaysOpen = true;
                EmailDataContext.GetInstance().cmshow.Focus();
            }
            catch (Exception exception)
            {
                logger.Error(" Error occurred while btnConsultCall_Click() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnContacts control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnContacts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= EmailWindow_SizeChanged;
                if (btnContacts.IsChecked == null)
                {

                }
                else if (btnContacts.IsChecked == true)
                {
                    if (imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                        btnContacts.IsEnabled = false;
                    if (btnContacts.IsEnabled == false || btnData.IsEnabled == false || btnResponses.IsEnabled == false)
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                            colDataResHistory.Width = new GridLength(this.Width - (16 + colEmailOptions.Width.Value));
                        else if (_emailWindowState == EmailWindowState.Maximized)
                            colDataResHistory.Width = new GridLength(this.Width - colEmailOptions.Width.Value);
                    }
                    else
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                        {
                            colMain.MinWidth = colDataResHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                colMain.Width = new GridLength(400);
                                colDataResHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                colMain.Width = new GridLength(mathSize);
                                colDataResHistory.Width = new GridLength(mathSize);
                            }
                            if (!(ConfigContainer.Instance().AllKeys.Contains("allow.system-draggable") &&
                            ((string)ConfigContainer.Instance().GetValue("allow.system-draggable")).ToLower().Equals("true")))
                            {
                                if (Left < 0)
                                    Left = 0;
                                if (Top < 0)
                                    Top = 0;
                                if (Left > SystemParameters.WorkArea.Right - Width)
                                    Left = SystemParameters.WorkArea.Right - Width;
                                if (Top > SystemParameters.WorkArea.Bottom - Height)
                                    Top = SystemParameters.WorkArea.Bottom - Height;
                            }
                            Width = colMain.Width.Value + colDataResHistory.Width.Value + colEmailOptions.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + colEmailOptions.Width.Value;
                        }
                        else if (_emailWindowState == EmailWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            colMain.Width = new GridLength(mathSize);
                            colDataResHistory.Width = new GridLength(mathSize);
                        }
                    }

                    gridDataResHistory.Children.Clear();
                    btnShowHideIXNPanel.IsEnabled = true;

                    if (string.IsNullOrEmpty(_contactID))
                    {
                        Dictionary<ContactDetails, string> _contactdetails = null;
                        _contactdetails = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(EmailDataContext.GetInstance().TenantDbId.ToString(), MediaTypes.Email, dataUserControl.CurrentData);
                        if (_contactdetails != null)
                        {
                            _contactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                        }
                    }
                    UserControl contact = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactUserControl(_contactID, MediaTypes.Email);
                    if (contact != null)
                    {
                        contact.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        Grid.SetColumn(contact, 1);
                        gridDataResHistory.Children.Add(contact);
                    }

                    btnResponses.IsEnabled = true;
                    btnData.IsEnabled = true;
                    btnResponses.IsChecked = false;
                    btnData.IsChecked = false;
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                }
                else
                {
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _windowRightGridWidth = colDataResHistory.Width.Value;
                    gridDataResHistory.Children.Clear();
                    colDataResHistory.Width = new GridLength(0);
                    colDataResHistory.MinWidth = 0;

                    if (_emailWindowState == EmailWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + colEmailOptions.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + colEmailOptions.Width.Value;
                        colMain.Width = new GridLength(_windowMainGridWidth);
                        colMain.MinWidth = 400;
                    }
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        colMain.Width = new GridLength(this.Width - (colEmailOptions.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(EmailWindow_SizeChanged);

                #region Old Code
                //if (_emailDetails.ContactImageSource != null && _emailDetails.ContactImageSource.ToString().Contains("leftArrow.png"))
                //{
                //    if (imgShowHideIXNPanel.Source != null && !imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //    {
                //        logger.Debug("Clearing contact user control");
                //        _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //        gridDataResHistory.Children.Clear();
                //        colDataResHistory.Width = new GridLength(0);
                //        colDataResHistory.MinWidth = 0;
                //        colMain.Width = new GridLength(3, GridUnitType.Star);
                //        colMain.MinWidth = 400;
                //        btnShowHideIXNPanel.IsEnabled = false;
                //    }
                //    else
                //    {
                //        btnContacts.IsChecked = true;
                //    }

                //}
                //else
                //{
                //    //Load Contacts
                //    if (EmailDataContext.GetInstance().HtPlugin.ContainsKey(Plugins.Contact))
                //    {
                //        logger.Debug("Loading contact user control for the contact ID : " + _contactID);
                //        gridDataResHistory.Children.Clear();
                //        if (_emailWindowState == EmailWindowState.Normal)
                //        {
                //            colDataResHistory.Width = new GridLength(400);
                //            colDataResHistory.MinWidth = 400;
                //        }
                //        else if (_emailWindowState == EmailWindowState.Maximized)
                //        {
                //            if (this.Width >= 1280)
                //                colDataResHistory.Width = new GridLength((System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2) - 25);

                //            else
                //                colDataResHistory.Width = new GridLength(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2);
                //        }
                //        if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //        {
                //            colDataResHistory.Width = new GridLength(3, GridUnitType.Star);
                //            colDataResHistory.MinWidth = 400;
                //        }
                //        if (string.IsNullOrEmpty(_contactID))
                //        {
                //            Dictionary<ContactDetails, string> _contactdetails = null;
                //            _contactdetails = ((IContactPlugin)EmailDataContext.GetInstance().HtPlugin[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(EmailDataContext.GetInstance().TenantDbId.ToString(), MediaTypes.Email, dataUserControl.CurrentData);
                //            if (_contactdetails != null)
                //            {
                //                _contactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                //            }
                //        }
                //        UserControl contact = ((IContactPlugin)EmailDataContext.GetInstance().HtPlugin[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactUserControl(_contactID, _interactionID, MediaTypes.Email);
                //        if (contact != null)
                //        {
                //            contact.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //            Grid.SetColumn(contact, 1);
                //            gridDataResHistory.Children.Add(contact);
                //        }
                //    }
                //    btnShowHideIXNPanel.IsEnabled = true;
                //    btnData.IsChecked = false;
                //    btnResponses.IsChecked = false;
                //    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                //}
                #endregion Old Code
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while btnContacts_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        /// <summary>
        /// Handles the Click event of the btnCustomerDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCustomerDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_contactID))
                {
                    List<string> attribList = new List<string>();
                    attribList.Add("PhoneNumber");
                    System.Windows.Controls.ContextMenu _menuContext = new System.Windows.Controls.ContextMenu();
                    EventGetAttributes eventGetAttribute = ContactServerHelper.RequestGetContactAttribute(_contactID, attribList);

                    if (eventGetAttribute.Attributes != null)
                    {
                        List<AttributesHeader> attributeHeader = eventGetAttribute.Attributes.Cast<AttributesHeader>().ToList();
                        if (attributeHeader.Count > 0)
                        {
                            AttributesInfoList attributeList = attributeHeader[0].AttributesInfoList;
                            for (int i = 0; i < attributeList.Count; i++)
                            {
                                System.Windows.Controls.MenuItem _itemPhoneNumber = new System.Windows.Controls.MenuItem();
                                if (!String.IsNullOrEmpty(attributeList[i].AttrValue.ToString()))
                                {
                                    _itemPhoneNumber.Header = "Call " + (string.IsNullOrEmpty(attributeList[i].Description) ?
                                        (" " + attributeList[i].AttrValue.ToString()) :
                                        (new ContactOperation().AdjustDescription(attributeList[i].Description) + " (" + attributeList[i].AttrValue.ToString() + ")"));
                                    _itemPhoneNumber.Tag = attributeList[i].AttrValue.ToString();
                                    Image icon = new Image();
                                    icon.Source = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Email\\Phone.png", UriKind.Relative));
                                    icon.Stretch = Stretch.Fill;
                                    _itemPhoneNumber.Icon = icon;
                                    _itemPhoneNumber.Click += new RoutedEventHandler(_itemPhoneNumber_Click);
                                    _itemPhoneNumber.FontWeight = FontWeights.Bold;
                                    _menuContext.Items.Add(_itemPhoneNumber);
                                }
                            }
                        }
                    }

                    if (_menuContext.Items.Count == 0)
                    {
                        System.Windows.Controls.MenuItem _itemPhoneNumber = new System.Windows.Controls.MenuItem();
                        _itemPhoneNumber.Header = "No Number exists";
                        _itemPhoneNumber.FontWeight = FontWeights.Bold;
                        _menuContext.Items.Add(_itemPhoneNumber);
                    }
                    btnCustomerDetails.ContextMenu = _menuContext;
                    btnCustomerDetails.ContextMenu.PlacementTarget = btnCustomerDetails;
                    btnCustomerDetails.ContextMenu.IsOpen = true;
                }
                else
                {
                    logger.Warn("Contact id is null or empty");
                }

            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnExit_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= EmailWindow_SizeChanged;
                if (btnData.IsChecked == null)
                {

                }
                else if (btnData.IsChecked == true)
                {
                    if (imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                        btnData.IsEnabled = false;
                    if (btnData.IsEnabled == false || btnContacts.IsEnabled == false || btnResponses.IsEnabled == false)
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                            colDataResHistory.Width = new GridLength(this.Width - (16 + colEmailOptions.Width.Value));
                        else if (_emailWindowState == EmailWindowState.Maximized)
                            colDataResHistory.Width = new GridLength(this.Width - colEmailOptions.Width.Value);
                    }
                    else
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                        {
                            colMain.MinWidth = colDataResHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                colMain.Width = new GridLength(400);
                                colDataResHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                colMain.Width = new GridLength(mathSize);
                                colDataResHistory.Width = new GridLength(mathSize);
                            }
                            if (!(ConfigContainer.Instance().AllKeys.Contains("allow.system-draggable") &&
                            ((string)ConfigContainer.Instance().GetValue("allow.system-draggable")).ToLower().Equals("true")))
                            {
                                if (Left < 0)
                                    Left = 0;
                                if (Top < 0)
                                    Top = 0;
                                if (Left > SystemParameters.WorkArea.Right - Width)
                                    Left = SystemParameters.WorkArea.Right - Width;
                                if (Top > SystemParameters.WorkArea.Bottom - Height)
                                    Top = SystemParameters.WorkArea.Bottom - Height;
                            }
                            Width = colMain.Width.Value + colDataResHistory.Width.Value + colEmailOptions.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + colEmailOptions.Width.Value;
                        }
                        else if (_emailWindowState == EmailWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            colMain.Width = new GridLength(mathSize);
                            colDataResHistory.Width = new GridLength(mathSize);
                        }

                    }
                    gridDataResHistory.Children.Clear();
                    btnShowHideIXNPanel.IsEnabled = true;

                    if (dataUserControl != null)
                    {
                        dataUserControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        dataUserControl.MinWidth = 395;
                        gridDataResHistory.Children.Add(dataUserControl);
                    }

                    btnContacts.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnResponses.IsChecked = false;
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                }
                else
                {
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _windowRightGridWidth = colDataResHistory.Width.Value;
                    gridDataResHistory.Children.Clear();
                    colDataResHistory.Width = new GridLength(0);
                    colDataResHistory.MinWidth = 0;

                    if (_emailWindowState == EmailWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + colEmailOptions.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + colEmailOptions.Width.Value;
                        colMain.Width = new GridLength(_windowMainGridWidth);
                        colMain.MinWidth = 400;
                    }
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        colMain.Width = new GridLength(this.Width - (colEmailOptions.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(EmailWindow_SizeChanged);
                #region Old Code
                //if (_emailDetails.CasedataImageSource != null && _emailDetails.CasedataImageSource.ToString().Contains("leftArrow.png"))
                //{
                //    if (imgShowHideIXNPanel.Source != null && !imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //    {
                //        _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //        gridDataResHistory.Children.Clear();
                //        colDataResHistory.Width = new GridLength(0);
                //        colDataResHistory.MinWidth = 0;
                //        colMain.Width = new GridLength(3, GridUnitType.Star);
                //        colMain.MinWidth = 400;
                //        btnShowHideIXNPanel.IsEnabled = false;
                //    }
                //    else
                //    {
                //        btnData.IsChecked = true;
                //    }
                //}
                //else
                //{
                //    gridDataResHistory.Children.Clear();
                //    btnShowHideIXNPanel.IsEnabled = true;
                //    if (_emailWindowState == EmailWindowState.Normal)
                //    {
                //        colDataResHistory.Width = new GridLength(400);
                //        colDataResHistory.MinWidth = 400;
                //    }
                //    else if (_emailWindowState == EmailWindowState.Maximized)
                //    {
                //        if (this.Width >= 1280)
                //            colDataResHistory.Width = new GridLength((System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2) - 25);

                //        else
                //            colDataResHistory.Width = new GridLength(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2);
                //    }
                //    if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //    {
                //        colDataResHistory.Width = new GridLength(3, GridUnitType.Star);
                //        colDataResHistory.MinWidth = 400;
                //    }
                //    if (dataUserControl != null)
                //    {
                //        dataUserControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //        Grid.SetColumn(dataUserControl, 1);
                //        gridDataResHistory.Children.Add(dataUserControl);
                //    }
                //    btnContacts.IsChecked = false;
                //    btnResponses.IsChecked = false;
                //    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));

                //}
                #endregion
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while btnData_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var showMessageBox = new MessageBox("Information",
                                         "Are you sure want to delete the interaction?", "Yes", "No", false);
                showMessageBox.Owner = this;
                showMessageBox.ShowDialog();
                if (showMessageBox.DialogResult == true)
                {
                    if (!string.IsNullOrEmpty(_parentIxnID))
                    {
                        KeyValueCollection kvpixnIds = new KeyValueCollection();
                        kvpixnIds.Add("id", _parentIxnID);
                        InteractionService interactionService = new InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues output = interactionService.PullInteraction(EmailDataContext.GetInstance().TenantDbId, EmailDataContext.GetInstance().ProxyClientID, kvpixnIds);
                        if (output.MessageCode == "200")
                        {
                            EventPulledInteractions eventpullixnIds = (EventPulledInteractions)output.IMessage;
                            if (eventpullixnIds.Interactions != null && eventpullixnIds.Interactions.Count > 0)
                            {
                                DeleteInteraction(false);
                                OpenInboundMail(eventpullixnIds);
                            }
                            else
                            {
                                output = interactionService.GetInteractionProperties(EmailDataContext.GetInstance().ProxyClientID, _parentIxnID);
                                if (output.MessageCode == "200" && output.IMessage != null)
                                {
                                    EventInteractionProperties eventInteractionProperties = (EventInteractionProperties)output.IMessage;
                                    if (eventInteractionProperties.Interaction.InteractionState == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.InteractionState.Handling)
                                    {
                                        showMessageBox = new MessageBox("Information",
                                      "Currently Parent Inbound Email is handled by the Agent( " + GetFullNameOfAgent(eventInteractionProperties.Interaction.InteractionAssignedTo) + " ), Do you still want to delete the Email?", "Yes", "No", false);
                                        showMessageBox.Owner = this;
                                        showMessageBox.ShowDialog();
                                        if (showMessageBox.DialogResult == true)
                                            DeleteInteraction(true);
                                    }
                                    else
                                        ShowDefaultWarningPopup();
                                }
                                else
                                    ShowDefaultWarningPopup();
                            }
                        }
                    }
                    else
                    {
                        DeleteInteraction(true);
                    }

                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnDelete_Click method " + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmailDataContext.GetInstance().IsContactServerActive)
                    if (lblEmailIxnType.Text.ToString() == "Inbound")
                    {
                        SaveInboundMail(true);
                    }
                    else
                    {
                        SaveOutboundEmail(true, true);
                    }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnExit_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnForward control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.UserControl teamCommunicatorForward = LoadTeamCommunicatorForward();
                if (teamCommunicatorForward != null)
                {
                    logger.Info("Team Communicator window : " + teamCommunicatorForward);
                    teamCommunicatorForward.Unloaded += new RoutedEventHandler(userControl_Unloaded);
                    var parent = FindAncestor<Grid>(teamCommunicatorForward);
                    if (parent != null)
                        parent.Children.Clear();
                    Grid grid1 = new Grid();
                    grid1.Background = Brushes.White;
                    grid1.Children.Add(teamCommunicatorForward);
                    var menuConsultItem = new System.Windows.Controls.MenuItem();
                    menuConsultItem.StaysOpenOnClick = true;
                    menuConsultItem.Background = Brushes.Transparent;
                    menuConsultItem.Header = grid1;
                    menuConsultItem.Margin = new Thickness(-18, -7, -20, -7);
                    menuConsultItem.Width = Double.NaN;
                    if (contextMenuTransfer == null)
                        contextMenuTransfer = new System.Windows.Controls.ContextMenu();
                    contextMenuTransfer.Items.Clear();
                    contextMenuTransfer.Items.Add(menuConsultItem);
                    contextMenuTransfer.PlacementTarget = btnForward;
                    contextMenuTransfer.Placement = PlacementMode.Bottom;
                    contextMenuTransfer.IsOpen = true;
                    contextMenuTransfer.StaysOpen = true;
                    contextMenuTransfer.Focus();
                    logger.Info("Team Communicator window loaded");
                }
                else
                {
                    logger.Warn("Team Communicator not loaded");
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnForward_Click" + exception.ToString());
            }
        }

        //End
        //Save Inbound Mail
        //Start
        /// <summary>
        /// Handles the Click event of the btnInboundSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnInboundSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveInboundMail(false);
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnInboundSave_Click" + exception.ToString());
            }
        }

        //Mark Done
        //Start
        /// <summary>
        /// Handles the Click event of the btnMarkDone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMarkDone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory") && ((string)ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")).ToLower().Equals("true"))
                {
                    if (dataUserControl.CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)) &&
                        dataUserControl.CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)) &&
                              !string.IsNullOrEmpty(dataUserControl.CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString()) &&
                        dataUserControl.CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString().ToLower().Trim() != "none")
                    {
                        ShoworHidePrompt();
                    }
                    else
                    {
                        var showMessageBox = new MessageBox("Warning",
                                          "Disposition code is mandatory.", "", "_Ok", false);
                        showMessageBox.Owner = this;
                        showMessageBox.ShowDialog();
                        if (showMessageBox.DialogResult != true)
                        {
                            showMessageBox.Dispose();
                            return;
                        }
                        else
                        {
                            if (_emailDetails.CasedataImageSource != null && _emailDetails.CasedataImageSource.ToString().Contains("rightArrow.png"))
                            {
                                btnData.IsChecked = true;
                                btnData_Click(null, null);
                            }
                            IEnumerable<Pointel.Interactions.Email.UserControls.DataUserControl> collection = gridDataResHistory.Children.OfType<Pointel.Interactions.Email.UserControls.DataUserControl>();
                            if (collection != null)
                                foreach (var data in collection)
                                {
                                    var obj = (Pointel.Interactions.Email.UserControls.DataUserControl)data;
                                    if (obj != null)
                                        obj.tabitemDisposition.IsSelected = true;
                                }
                        }
                    }
                }
                else
                    ShoworHidePrompt();
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnMarkDone_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnMaximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= EmailWindow_SizeChanged;
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    WindowState = System.Windows.WindowState.Normal;
                }
                if (_emailWindowState == EmailWindowState.Normal)
                {
                    btnMaximize.Style = FindResource("RestoreButton") as Style;
                    tempWidth = this.ActualWidth;
                    tempHeight = this.ActualHeight;
                    tempLeft = this.Left;
                    tempTop = this.Top;
                    _emailWindowState = EmailWindowState.Maximized;
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
                    MainBorder.Margin = new Thickness(0);
                    this.Width = System.Windows.SystemParameters.WorkArea.Width;
                    this.Height = System.Windows.SystemParameters.WorkArea.Height;
                    this.Left = 0;
                    this.Top = 0;
                    if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                    {
                        colMain.Width = new GridLength(0);
                        colMain.MinWidth = 0;
                        colDataResHistory.Width = new GridLength(this.Width - 20);
                    }
                    else if (btnResponses.IsChecked == false && btnContacts.IsChecked == false && btnData.IsChecked == false)
                    {
                        colDataResHistory.Width = new GridLength(0);
                        colDataResHistory.MinWidth = 0;
                        colMain.Width = new GridLength(this.Width - 20);
                    }
                    else
                    {
                        var mathSize = (this.Width - 20) / 2;
                        colMain.Width = new GridLength(mathSize);
                        colDataResHistory.Width = new GridLength(mathSize);
                    }

                    RightSideRect.Visibility = Visibility.Hidden;
                    RightbottomSideRect.Visibility = Visibility.Hidden;
                    BottomSideRect.Visibility = Visibility.Hidden;
                }
                else if (_emailWindowState == EmailWindowState.Maximized)
                {
                    _emailWindowState = EmailWindowState.Normal;
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                    WindowState = System.Windows.WindowState.Normal;
                    MainBorder.Margin = new Thickness(8);
                    btnMaximize.Style = FindResource("maximizeButton") as Style;
                    this.Width = tempWidth;
                    this.Height = tempHeight;
                    this.Left = tempLeft;
                    this.Top = tempTop;

                    if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                    {
                        colMain.Width = new GridLength(0);
                        colMain.MinWidth = 0;
                        colDataResHistory.MinWidth = 400;
                        colDataResHistory.Width = new GridLength(this.Width - (20 + 16));
                    }
                    else if (btnResponses.IsChecked == false && btnContacts.IsChecked == false && btnData.IsChecked == false)
                    {
                        colDataResHistory.Width = new GridLength(0);
                        colDataResHistory.MinWidth = 0;
                        colMain.MinWidth = 400;
                        colMain.Width = new GridLength(this.Width - (20 + 16));
                    }
                    else
                    {
                        var mathSize = (this.Width - (20 + 16)) / 2;
                        colMain.MinWidth = 400;
                        colDataResHistory.MinWidth = 400;
                        colMain.Width = new GridLength(mathSize);
                        colDataResHistory.Width = new GridLength(mathSize);
                    }

                    RightSideRect.Visibility = Visibility.Visible;
                    RightbottomSideRect.Visibility = Visibility.Visible;
                    BottomSideRect.Visibility = Visibility.Visible;
                }
                SizeChanged += EmailWindow_SizeChanged;
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnMaximize_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WindowState != System.Windows.WindowState.Minimized)
                {
                    WindowState = System.Windows.WindowState.Minimized;
                }
                Topmost = false;
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnMinimize_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOutboundSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOutboundSave_Click(object sender, RoutedEventArgs e)
        {
            SaveOutboundEmail(false, true);
        }

        /// <summary>
        /// Handles the Click event of the btnSaveMail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSaveMail_Click(object sender, RoutedEventArgs e)
        {
            SaveOutboundEmail(false, false);
        }

        /// <summary>
        /// Handles the Click event of the btnPrint control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (lblEmailIxnType.Text.ToString())
                {
                    case "Inbound": inboundUserControl.PrintInboundMail();
                        break;
                    case "Outbound": outboundUserControl.PrintOutboundMail();
                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do btnPrint_Click() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnReplyAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnReplyAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReplyorReplyALLClick(true);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        //Reply and Reply All
        //Start
        /// <summary>
        /// Handles the Click event of the btnReply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnReply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReplyorReplyALLClick(false);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnResponses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnResponses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= EmailWindow_SizeChanged;
                if (btnResponses.IsChecked == null)
                {

                }
                else if (btnResponses.IsChecked == true)
                {
                    if (imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                        btnResponses.IsEnabled = false;
                    if (btnResponses.IsEnabled == false || btnData.IsEnabled == false || btnContacts.IsEnabled == false)
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                            colDataResHistory.Width = new GridLength(this.Width - (16 + colEmailOptions.Width.Value));
                        else if (_emailWindowState == EmailWindowState.Maximized)
                            colDataResHistory.Width = new GridLength(this.Width - colEmailOptions.Width.Value);
                    }
                    else
                    {
                        if (_emailWindowState == EmailWindowState.Normal)
                        {
                            colMain.MinWidth = colDataResHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                colMain.Width = new GridLength(400);
                                colDataResHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                colMain.Width = new GridLength(mathSize);
                                colDataResHistory.Width = new GridLength(mathSize);
                            }
                            if (!(ConfigContainer.Instance().AllKeys.Contains("allow.system-draggable") &&
                            ((string)ConfigContainer.Instance().GetValue("allow.system-draggable")).ToLower().Equals("true")))
                            {
                                if (Left < 0)
                                    Left = 0;
                                if (Top < 0)
                                    Top = 0;
                                if (Left > SystemParameters.WorkArea.Right - Width)
                                    Left = SystemParameters.WorkArea.Right - Width;
                                if (Top > SystemParameters.WorkArea.Bottom - Height)
                                    Top = SystemParameters.WorkArea.Bottom - Height;
                            }
                            Width = colMain.Width.Value + colDataResHistory.Width.Value + colEmailOptions.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + colEmailOptions.Width.Value;
                        }
                        else if (_emailWindowState == EmailWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            colMain.Width = new GridLength(mathSize);
                            colDataResHistory.Width = new GridLength(mathSize);
                        }
                    }

                    gridDataResHistory.Children.Clear();
                    btnShowHideIXNPanel.IsEnabled = true;

                    System.Windows.Controls.UserControl contactDirectory = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetResponseUserControl(false, EventNotifyResponseComposeClick);
                    contactDirectory.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    contactDirectory.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    Grid.SetColumn(contactDirectory, 1);
                    gridDataResHistory.Children.Add(contactDirectory);

                    btnContacts.IsEnabled = true;
                    btnData.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnData.IsChecked = false;
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                    _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                }
                else
                {
                    _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                    _windowRightGridWidth = colDataResHistory.Width.Value;
                    gridDataResHistory.Children.Clear();
                    colDataResHistory.Width = new GridLength(0);
                    colDataResHistory.MinWidth = 0;

                    if (_emailWindowState == EmailWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + colEmailOptions.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + colEmailOptions.Width.Value;
                        colMain.Width = new GridLength(_windowMainGridWidth);
                        colMain.MinWidth = 400;
                    }
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        colMain.Width = new GridLength(this.Width - (colEmailOptions.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(EmailWindow_SizeChanged);

                #region Old Code
                //if (_emailDetails.ResponseImageSource != null && _emailDetails.ResponseImageSource.ToString().Contains("leftArrow.png"))
                //{
                //    if (imgShowHideIXNPanel.Source != null && !imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //    {
                //        _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //        gridDataResHistory.Children.Clear();
                //        colDataResHistory.Width = new GridLength(0);
                //        colDataResHistory.MinWidth = 0;
                //        colMain.Width = new GridLength(3, GridUnitType.Star);
                //        colMain.MinWidth = 400;
                //        btnShowHideIXNPanel.IsEnabled = false;
                //    }
                //    else
                //    {
                //        btnResponses.IsChecked = true;
                //    }
                //}
                //else
                //{
                //    if (EmailDataContext.GetInstance().HtPlugin.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                //    {
                //        btnShowHideIXNPanel.IsEnabled = true;
                //        gridDataResHistory.Children.Clear();
                //        if (_emailWindowState == EmailWindowState.Normal)
                //        {
                //            colDataResHistory.Width = new GridLength(400);
                //            colDataResHistory.MinWidth = 400;
                //        }
                //        else if (_emailWindowState == EmailWindowState.Maximized)
                //        {
                //            if (this.Width >= 1280)
                //                colDataResHistory.Width = new GridLength((System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2) - 25);

                //            else
                //                colDataResHistory.Width = new GridLength(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2);
                //        }
                //        if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Show_Left.png"))
                //        {
                //            colDataResHistory.Width = new GridLength(3, GridUnitType.Star);
                //            colDataResHistory.MinWidth = 400;
                //        }
                //        System.Windows.Controls.UserControl contactDirectory = ((IContactPlugin)EmailDataContext.GetInstance().HtPlugin[Pointel.Interactions.IPlugins.Plugins.Contact]).GetResponseUserControl(false, EventNotifyResponseComposeClick);
                //        contactDirectory.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //        contactDirectory.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //        Grid.SetColumn(contactDirectory, 1);
                //        gridDataResHistory.Children.Add(contactDirectory);
                //        btnContacts.IsChecked = false;
                //        btnData.IsChecked = false;
                //        _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                //        _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //        _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                //    }
                //}
                #endregion Old Code
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while btnResponses_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        /// <summary>
        /// Handles the Click event of the btnSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check From address
                if (!string.IsNullOrEmpty(outboundUserControl.txtOutboundFrom.Text))
                {
                    // Check Validation for To,Cc,Bcc and Subject
                    if (ValidateEmailAddress())
                    {
                        // Check Attachments Max Size
                        if (outboundUserControl.colAttachError.Width == new GridLength(0))
                        {
                            //Check Spell Check
                            if ((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                                       ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                                outboundUserControl.htmlEditor.SpellcheckMandatory();

                            if (outboundUserControl.htmlEditor._spellChecker != null && outboundUserControl.htmlEditor._spellChecker.MisspelledWords.Count > 0 && (ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                                       ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                            {
                                var showMessageBox = new MessageBox("Information",
                                                  "Please check the misspelled words", "", "_Ok", false);
                                showMessageBox.Owner = this;
                                showMessageBox.ShowDialog();
                                if (showMessageBox.DialogResult == true || showMessageBox.DialogResult == false)
                                {
                                    showMessageBox.Dispose();
                                    return;
                                }
                            }
                            else
                            {
                                // Check Disposition code
                                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory") && ((string)ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")).ToLower().Equals("true"))
                                {
                                    if (dataUserControl.CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)) &&
                                        dataUserControl.CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)) &&
                                       !string.IsNullOrEmpty(dataUserControl.CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString()) &&
                                       dataUserControl.CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString().ToLower().Trim() != "none")
                                    {
                                        SendInteraction();
                                    }
                                    else
                                    {
                                        var showMessageBox = new MessageBox("Warning",
                                                          "Disposition code is mandatory.", "", "_Ok", false);
                                        showMessageBox.Owner = this;
                                        showMessageBox.ShowDialog();
                                        if (showMessageBox.DialogResult != true)
                                        {
                                            showMessageBox.Dispose();
                                            return;
                                        }
                                        else
                                        {
                                            if (_emailDetails.CasedataImageSource != null && _emailDetails.CasedataImageSource.ToString().Contains("rightArrow.png"))
                                            {
                                                btnData.IsChecked = true;
                                                btnData_Click(null, null);
                                            }
                                            IEnumerable<Pointel.Interactions.Email.UserControls.DataUserControl> collection = gridDataResHistory.Children.OfType<Pointel.Interactions.Email.UserControls.DataUserControl>();
                                            if (collection != null)
                                                foreach (var data in collection)
                                                {
                                                    var obj = (Pointel.Interactions.Email.UserControls.DataUserControl)data;
                                                    if (obj != null)
                                                        obj.tabitemDisposition.IsSelected = true;
                                                }
                                        }
                                    }

                                }
                                else
                                    SendInteraction();
                            }
                        }
                        else
                            ShowError("Maximum upload limit is restricted to " + EmailServerDetails.EmailMaxAttachmentSize() + " MB.");
                    }
                }
                else
                    ShowError("Action Aborted: From Address is missing");
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnSend_Click method " + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnShowHideIXNPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnShowHideIXNPanel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= EmailWindow_SizeChanged;
                if (imgShowHideIXNPanel.Source != null && imgShowHideIXNPanel.Source.ToString().Contains("Hide_Left.png"))
                {
                    _windowMainGridWidth = colMain.ActualWidth;
                    imgShowHideIXNPanel.Source = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Show_Left.png", UriKind.Relative));

                    colMain.Width = new GridLength(0);
                    colMain.MinWidth = 0;
                    if (_emailWindowState == EmailWindowState.Normal)
                    {
                        colDataResHistory.Width = new GridLength(this.Width - (16 + colEmailOptions.Width.Value));
                        colDataResHistory.MinWidth = 400;
                        this.MinWidth = 400 + 16 + colEmailOptions.Width.Value;
                    }
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        colDataResHistory.Width = new GridLength(this.Width - colEmailOptions.Width.Value);
                        colDataResHistory.MinWidth = 400;
                        this.MinWidth = 400 + colEmailOptions.Width.Value;
                    }
                    ToolHeading.Text = "Show";
                    ToolContent.Text = "Agent can show the interaction panel";
                    if (btnData.IsChecked == true)
                        btnData.IsEnabled = false;
                    else if (btnResponses.IsChecked == true)
                        btnResponses.IsEnabled = false;
                    else if (btnContacts.IsChecked == true)
                        btnContacts.IsEnabled = false;
                }
                else
                {
                    ToolHeading.Text = "Hide";
                    ToolContent.Text = "Agent can hide the interaction panel";
                    imgShowHideIXNPanel.Source = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Hide_Left.png", UriKind.Relative));
                    if (_emailWindowState == EmailWindowState.Normal)
                    {
                        double size = (this.Width - (16 + colEmailOptions.Width.Value)) / 2;
                        colMain.Width = new GridLength(size);
                        colDataResHistory.Width = new GridLength(size);
                        colMain.MinWidth = colDataResHistory.MinWidth = 400;
                        this.MinWidth = 400 + 400 + 16 + colEmailOptions.Width.Value;
                    }
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        var _Width = this.Width - colEmailOptions.Width.Value;
                        colMain.Width = new GridLength(_Width / 2);
                        colDataResHistory.Width = new GridLength(_Width / 2);
                    }
                    btnData.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsEnabled = true;
                }
                SizeChanged += new SizeChangedEventHandler(EmailWindow_SizeChanged);
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while btnShowHideIXNPanel_Click() :" + generalException.ToString());
            }
        }

        //End
        /// <summary>
        /// Handles the Click event of the btnTransfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("btnTransferMail_Click Entry");
                System.Windows.Controls.UserControl teamCommunicatorTransfer = LoadTeamCommunicatorTransfer();
                if (teamCommunicatorTransfer != null)
                {
                    logger.Info("Team Communicator window : " + teamCommunicatorTransfer);
                    teamCommunicatorTransfer.Unloaded += new RoutedEventHandler(userControl_Unloaded);
                    var parent = FindAncestor<Grid>(teamCommunicatorTransfer);
                    if (parent != null)
                        parent.Children.Clear();
                    Grid grid1 = new Grid();
                    grid1.Background = Brushes.White;
                    grid1.Children.Add(teamCommunicatorTransfer);
                    var menuConsultItem = new System.Windows.Controls.MenuItem();
                    menuConsultItem.StaysOpenOnClick = true;
                    menuConsultItem.Background = Brushes.Transparent;
                    menuConsultItem.Header = grid1;
                    menuConsultItem.Width = teamCommunicatorTransfer.Width;
                    menuConsultItem.Height = teamCommunicatorTransfer.Height;
                    menuConsultItem.Margin = new Thickness(-18, -7, -20, -7);
                    if (contextMenuTransfer == null)
                        contextMenuTransfer = new System.Windows.Controls.ContextMenu();
                    contextMenuTransfer.Items.Clear();
                    contextMenuTransfer.Items.Add(menuConsultItem);
                    contextMenuTransfer.Placement = PlacementMode.Bottom;
                    contextMenuTransfer.PlacementTarget = btnTransfer;
                    contextMenuTransfer.IsOpen = true;
                    contextMenuTransfer.StaysOpen = true;
                    contextMenuTransfer.Focus();
                    logger.Info("Team Communicator window loaded");
                }
                else
                {
                    logger.Warn("Team Communicator not loaded");
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnTransfer_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Configurations the open file dialog.
        /// </summary>
        private void ConfigOpenFileDialog()
        {
            openFileDialog = new OpenFileDialog();
            string fileType = string.Empty;
            string others = string.Empty;
            string[] types = { "txt", "pdf", "zip", "rar", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "png", "gif", "bmp", "jpg", "jpeg" };
            string dilimiters = "|";
            foreach (var item in types)
            {
                string[] RestrictAttachFileType = ConfigContainer.Instance().AllKeys.Contains("email.restricted-attachment-file-types") ? ((string)ConfigContainer.Instance().GetValue("email.restricted-attachment-file-types")).Replace(" ", "").Split(',') : null;
                if (!RestrictAttachFileType.Contains(item))
                    switch (item)
                    {
                        case "txt":
                            fileType += "Text Document|*.txt" + dilimiters;
                            break;
                        case "pdf":
                            fileType += "Portable Document Format (.pdf)|*.pdf" + dilimiters;
                            break;
                        case "zip":
                            fileType += "Compressed File Format (.zip)|*.zip" + dilimiters;
                            break;
                        case "rar":
                            fileType += "Compressed File Format (.rar)|*.rar" + dilimiters;
                            break;
                        case "doc":
                        case "docx":
                            if (!fileType.Contains("Word Documents"))
                            { fileType += "Word Documents|*.doc;*.docx" + dilimiters; }
                            break;
                        case "xls":
                        case "xlsx":
                            if (!fileType.Contains("Excel Worksheets"))
                            { fileType += "Excel Worksheets|*.xls;*.xlsx" + dilimiters; }
                            break;
                        case "ppt":
                        case "pptx":
                            if (!fileType.Contains("PowerPoint Presentations"))
                            { fileType += "PowerPoint Presentations|*.ppt;*.pptx" + dilimiters; }
                            break;
                        case "png":
                            fileType += "Portable Network Graphics|*.png" + dilimiters;
                            break;
                        case "gif":
                            fileType += "Graphic Interchange Format (.gif)|*.gif" + dilimiters;
                            break;
                        case "bmp":
                            fileType += "Bitmap (.bmp)|*.bmp" + dilimiters;
                            break;
                        case "jpg":
                        case "jpeg":
                            if (!fileType.Contains("Joint Photographic Experts Group"))
                            { fileType += "Joint Photographic Experts Group (.jpg/.jpeg)|*.jpg;*.jpeg" + dilimiters; }
                            break;
                    }
            }
            fileType += "All files|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = fileType;
            openFileDialog.FilterIndex = 100;
        }

        /// <summary>
        /// Deletes the interaction.
        /// </summary>
        /// <param name="isCloseWindow">if set to <c>true</c> [is close window].</param>
        private void DeleteInteraction(bool isCloseWindow)
        {
            InteractionService interactionService = new InteractionService();
            Pointel.Interactions.Core.Common.OutputValues output = interactionService.StopProcessingInteraction(EmailDataContext.GetInstance().ProxyClientID, _interactionID);
            if (output.MessageCode == "200")
            {
                // if (interactionTypes == InteractionTypes.EventPull || interactionTypes == InteractionTypes.Compose)
                ContactServerHelper.DeleteInteraction(_interactionID);
                isNeedToNotifyRefresh = false;
                NotifyHistoryRefresh(_interactionID, true);
                if (isCloseWindow)
                {
                    //UpdatePendingStatus(false);
                    _isSaveMailToWorkbin = false;
                    grdrow_error.Height = new GridLength(0);
                    this.Close();
                }
            }
            else
            {
                grdrow_error.Height = GridLength.Auto;
                ShowError(output.Message.ToString());
            }
        }

        /// <summary>
        /// Emails the interaction listener_ contact server notification handler.
        /// </summary>
        void EmailInteractionListener_ContactServerNotificationHandler()
        {
            if (EmailDataContext.GetInstance().IsContactServerActive)
            {
                switch (lblEmailIxnType.Text.ToString())
                {
                    case "Inbound":
                        panelInbound.Visibility = Visibility.Visible;
                        if (isContactDatatobeUpdated)
                            inboundUserControl.BindContent(GetContentFromUCS(_interactionID));

                        // Changes done by sakthi to show replay button again if the mail contain CC or BCC - 02-11-2015.
                        if (!string.IsNullOrEmpty(inboundUserControl.txtInboundCc.Text) || !string.IsNullOrEmpty(inboundUserControl.txtInboundBcc.Text))
                            btnReplyAll.Visibility = Visibility.Visible;
                        break;
                    case "Outbound":
                        panelOutbound.Visibility = Visibility.Visible;
                        break;
                }
                dataUserControl.tbItemNotes.Visibility = btnPrint.Visibility = Visibility.Visible;
            }
            else
            {
                dataUserControl.tbItemNotes.Visibility = btnPrint.Visibility = panelInbound.Visibility = panelOutbound.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the Closing event of the EmailMainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void EmailMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispatcher.Invoke(
             new Action(() =>
             {
                 try
                 {
                     InteractionService.InteractionServerNotifier -= new NotifyInteractionServerState(InteractionService_InteractionServerNotifier);
                     if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                         ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).NotifyWorkbinContentChanged(_interactionID, true);
                     //if (isNeedToNotifyRefresh)
                     //    NotifyHistoryRefresh(_interactionID,false);
                     if (_isSaveMailToWorkbin)
                         btnExit_Click(null, null);
                     if (grdrow_error.Height == GridLength.Auto)
                         e.Cancel = true;
                 }
                 catch (Exception exception)
                 {
                     logger.Error("Error occurred at EmailMainWindow_Closing" + exception.ToString());
                 }
             }));
        }

        /// <summary>
        /// Handles the Loaded event of the EmailMainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EmailMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // grdrow_error.Height = new GridLength(0);
                _emailDetails.UserName = EmailDataContext.GetInstance().Username;
                _emailDetails.PlaceID = EmailDataContext.GetInstance().PlaceName;
                EmailInteractionListener.ContactServerNotificationHandler += new ContactServerNotificationHandler(EmailInteractionListener_ContactServerNotificationHandler);
                EmailInteractionListener.VoiceStatusNotification += EmailInteractionListener_VoiceStatusNotification;
                btnConsultCall.IsEnabled = EmailDataContext.GetInstance().IsVoiceMediaEnabled;
                InteractionService.InteractionServerNotifier += new NotifyInteractionServerState(InteractionService_InteractionServerNotifier);
                _shadowEffect.ShadowDepth = 0;
                _shadowEffect.Opacity = 0.5;
                _shadowEffect.Softness = 0.5;
                _shadowEffect.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#003660");
                SystemMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
                DeleteMenu(SystemMenu, SC_Move, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Size, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Maximize, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Close, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Restore, MF_BYCOMMAND);
                DeleteMenu(SystemMenu, SC_Minimize, MF_BYCOMMAND);
                InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                InsertMenu(SystemMenu, 3, MF_BYPOSITION, CU_Close, "Close");
                var source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);
                lblTabItemShowTimer.Text = "[00:00:00]";
                _emailDetails.CasedataImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\leftArrow.png", UriKind.Relative));
                _emailDetails.ContactImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                _emailDetails.ResponseImageSource = new BitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\rightArrow.png", UriKind.Relative));
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                {
                    ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).SubscribeUpdateNotification(ContactUpdation);
                    if (!string.IsNullOrEmpty(_contactID))
                    {
                        getInprogessInteractionCount();
                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                        {
                            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Email))
                            {
                                IMessage response1 = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetMediaViceInteractionCount("email", _contactID, _interactionID);
                                if (response1 != null)
                                {
                                    if (response1 != null && response1.Id == EventCountInteractions.MessageId)
                                    {
                                        EventCountInteractions eventInteractionListGet = (EventCountInteractions)response1;

                                        if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                                        {
                                            BitmapImage bi = new BitmapImage();
                                            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                            bi.BeginInit();
                                            bi.UriSource = new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Email\\Email.png", UriKind.Relative);
                                            bi.EndInit();
                                            image.Source = bi;
                                            image.Width = 15;
                                            image.Height = 15;
                                            image.Visibility = Visibility.Visible;
                                            int inprogressIXNCount = 0;
                                            inprogressIXNCount = (int)eventInteractionListGet.TotalCount.Value;
                                            if (inprogressIXNCount > 0)
                                            {
                                                StackPanel stk = new StackPanel();
                                                stk.Orientation = Orientation.Horizontal;
                                                stk.Children.Add(image);
                                                stk.Children.Add(new TextBlock() { Text = " Email  " + "(" + inprogressIXNCount + ")", Padding = new Thickness(2) });
                                                stkPanelIXNCountContent.Children.Add(stk);
                                            }
                                        }
                                    }
                                }
                            }
                            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Chat))
                            {
                                IMessage response2 = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetMediaViceInteractionCount("chat", _contactID, _interactionID);
                                if (response2 != null)
                                {
                                    if (response2 != null && response2.Id == EventCountInteractions.MessageId)
                                    {
                                        EventCountInteractions eventInteractionListGet = (EventCountInteractions)response2;

                                        if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                                        {
                                            BitmapImage bi = new BitmapImage();
                                            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                            bi.BeginInit();
                                            bi.UriSource = new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Chat\\Chat.png", UriKind.Relative);
                                            bi.EndInit();
                                            image.Source = bi;
                                            image.Width = 15;
                                            image.Height = 15;
                                            image.Visibility = Visibility.Visible;
                                            int inprogressIXNCount = 0;
                                            inprogressIXNCount = (int)eventInteractionListGet.TotalCount.Value;
                                            if (inprogressIXNCount > 0)
                                            {
                                                StackPanel stk = new StackPanel();
                                                stk.Orientation = Orientation.Horizontal;
                                                stk.Children.Add(image);
                                                stk.Children.Add(new TextBlock() { Text = " Chat  " + "(" + inprogressIXNCount + ")", Padding = new Thickness(2) });
                                                stkPanelIXNCountContent.Children.Add(stk);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        getRecentInteractionList();
                    }
                }
                if (dataUserControl != null)
                {
                    gridDataResHistory.Children.Clear();
                    gridDataResHistory.Children.Add(dataUserControl);
                }
                _isWindowSizeChanged = true;
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at EmailMainWindow Loaded" + exception.ToString());
            }
        }

        void EmailInteractionListener_VoiceStatusNotification(bool isEnabled)
        {
            btnConsultCall.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Handles the Activated event of the EmailWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EmailWindow_Activated(object sender, EventArgs e)
        {
            try
            {
                MainBorder.BorderBrush = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString("#0070C5"));
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at EmailWindow_Activated" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Deactivated event of the EmailWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EmailWindow_Deactivated(object sender, EventArgs e)
        {
            try
            {
                MainBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at EmailWindow_Deactivated" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the EmailWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void EmailWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (colMain.ActualWidth != 0 && colDataResHistory.ActualWidth != 0)
                {
                    colMain.Width = new GridLength(((this.Width - 16) / 2) - (colEmailOptions.Width.Value / 2));
                    colDataResHistory.Width = new GridLength(((this.Width - 16) / 2) - (colEmailOptions.Width.Value / 2));
                    _windowMainGridWidth = colMain.Width.Value;
                    _windowRightGridWidth = colDataResHistory.Width.Value;
                }
                else
                {
                    if (colMain.ActualWidth != 0)
                    {
                        colMain.Width = new GridLength((this.Width - (16 + colEmailOptions.Width.Value)));
                        _windowMainGridWidth = colMain.Width.Value;
                    }
                    if (colDataResHistory.ActualWidth != 0)
                    {
                        colDataResHistory.Width = new GridLength((this.Width - (16 + colEmailOptions.Width.Value)));
                        _windowRightGridWidth = colDataResHistory.Width.Value;
                    }
                }
                _windowFullWidth = this.Width;
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while EmailWindow_SizeChanged() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the StateChanged event of the EmailWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EmailWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                StateChanged -= EmailWindow_StateChanged;
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                }
                if ((WindowState == System.Windows.WindowState.Maximized))
                {
                    WindowState = System.Windows.WindowState.Normal;
                    btnMaximize_Click(null, null);
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
                }
                if (WindowState == System.Windows.WindowState.Normal)
                {
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                }
                if (_emailWindowState == EmailWindowState.Maximized)
                {
                    if (WindowState == System.Windows.WindowState.Minimized)
                    {
                        DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                        DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                        DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                        InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    }
                    else
                    {
                        DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                        DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                        DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                        InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                        InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Minimize, "Minimize");
                    }
                }
                if (_emailWindowState == EmailWindowState.Maximized && WindowState == System.Windows.WindowState.Minimized)
                {

                }

                StateChanged += EmailWindow_StateChanged;
            }
            catch (Exception generalException)
            {
                logger.Error(" Error occurred while EmailWindow_StateChanged() :" + generalException.ToString());
            }
        }

        private void EmailWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _emailDetails = null;
            _timerforcloseError = null;
            interactionDataList = null;
            _shadowEffect = null;
            tempWidth = 0;
            tempHeight = 0;
            tempLeft = 0;
            tempTop = 0;
            _contactID = null;
            _interactionID = null;
            _parentIxnID = null;
            inboundUserControl = null;
            dataUserControl = null;
            outboundUserControl = null;
            eventGetInteractionContent = null;
            contextMenuTransfer = null;
            _importClass = null;
            firstName = null;
            lastName = null;
            dialpad = null;
            openFileDialog = null;
            _windowMainGridWidth = 0;
            _windowRightGridWidth = 0;
            _windowFullWidth = 0;
            EmailInteractionListener.ContactServerNotificationHandler -= EmailInteractionListener_ContactServerNotificationHandler;
            EmailInteractionListener.VoiceStatusNotification -= EmailInteractionListener_VoiceStatusNotification;
            InteractionService.InteractionServerNotifier -= InteractionService_InteractionServerNotifier;
            ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).UnSubscribeUpdateNotification(ContactUpdation);
        }

        private void EmailWindow_Closed(object sender, EventArgs e)
        {
            UpdatePendingStatus(false);
        }

        /// <summary>
        /// Events the notify response compose click.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="name">The name.</param>
        /// <param name="selectedAttachments">The selected attachments.</param>
        /// <returns>System.String.</returns>
        private string EventNotifyResponseComposeClick(string plaintext, string response, string subject, string name, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttachmentList selectedAttachments)
        {
            try
            {
                plaintext = string.Empty;
                if (!string.IsNullOrEmpty(response))
                {
                    response = response.Replace("&lt;", "<");
                    response = response.Replace("&gt;", ">");
                    if (string.IsNullOrEmpty(outboundUserControl.txtOutboundSubject.Text.Trim()) && !string.IsNullOrEmpty(subject)
                        && ConfigContainer.Instance().AllKeys.Contains("email.enable.include-standard-response-subject-on-insert")
                        && ((string)ConfigContainer.Instance().GetValue("email.enable.include-standard-response-subject-on-insert")).ToLower().Equals("true"))
                        outboundUserControl.txtOutboundSubject.Text = subject;

                    if (getContacts())
                    {
                        string data = GetReponseFieldCodeData(response);
                        if (!string.IsNullOrEmpty(data))
                            response = data;
                    }

                    outboundUserControl.AddStandardResponse(response, selectedAttachments);
                }
            }
            catch (Exception exception)
            {
                logger.Error("EventNotifyResponseComposeClick() " + exception.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetContactName()
        {
            string contactName = string.Empty;
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                {
                    List<string> attributeList = new List<string>();
                    attributeList.Add("FirstName");
                    attributeList.Add("LastName");

                    EventGetAttributes eventGetAttribute = ContactServerHelper.RequestGetContactAttribute(_contactID, attributeList);
                    if (eventGetAttribute != null)
                    {
                        List<AttributesHeader> attributeHeader = eventGetAttribute.Attributes.Cast<AttributesHeader>().ToList();
                        if (attributeHeader.Where(x => x.AttrName.Equals("FirstName")).ToList().Count > 0)
                        {
                            AttributesHeader firstNameHeader = attributeHeader.Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                            if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                            {
                                this.firstName = firstNameHeader.AttributesInfoList[0].AttrValue.ToString();
                                contactName += firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                            }
                        }
                        if (attributeHeader.Where(x => x.AttrName.Equals("LastName")).ToList().Count > 0)
                        {
                            AttributesHeader LastNameHeader = attributeHeader.Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                            if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                            {
                                this.lastName = LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                                contactName += LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                contactName = string.Empty;
                logger.Error("Error while getting contact name :" + ex.Message);
            }
            return contactName;
        }

        private bool getContacts()
        {
            bool _isSuccess = false;
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    List<string> attributeList = new List<string>();
                    attributeList.Add("PhoneNumber");
                    attributeList.Add("EmailAddress");
                    attributeList.Add("FirstName");
                    attributeList.Add("LastName");
                    attributeList.Add("Title");
                    EventGetAttributes eventGetAttributes = ContactServerHelper.RequestGetContactAttribute(_contactID, attributeList);
                    if (eventGetAttributes != null)
                    {
                        CustomerDetails.Clear();
                        if (eventGetAttributes.Attributes != null && eventGetAttributes.Attributes.Count > 0)
                            for (int attributesCount = 0; attributesCount < eventGetAttributes.Attributes.Count; attributesCount++)
                            {
                                if (eventGetAttributes.Attributes[attributesCount].AttrName == "FirstName")
                                {
                                    CustomerDetails.Add("FirstName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                }
                                if (eventGetAttributes.Attributes[attributesCount].AttrName == "LastName")
                                {
                                    CustomerDetails.Add("LastName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                }
                                if (eventGetAttributes.Attributes[attributesCount].AttrName == "Title")
                                {
                                    CustomerDetails.Add("Title", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                }
                                if (eventGetAttributes.Attributes[attributesCount].AttrName == "EmailAddress" && eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count > 0)
                                {
                                    for (int listCount = 0; listCount < eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count; listCount++)
                                    {
                                        if (eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Primary.AttrId == eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrId)
                                        {
                                            CustomerDetails.Add("PrimaryEmailAddress", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString());
                                            break;
                                        }
                                    }
                                }
                                if (eventGetAttributes.Attributes[attributesCount].AttrName == "PhoneNumber" && eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count > 0)
                                {
                                    for (int listCount = 0; listCount < eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count; listCount++)
                                    {
                                        if (eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Primary.AttrId == eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrId)
                                        {
                                            CustomerDetails.Add("PrimaryPhoneNumber", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                        CustomerDetails.Add("FullName", string.Empty);
                        if (CustomerDetails.ContainsKey("FirstName"))
                            CustomerDetails["FullName"] += CustomerDetails["FirstName"].ToString() + " ";
                        if (CustomerDetails.ContainsKey("LastName"))
                            CustomerDetails["FullName"] += CustomerDetails["LastName"].ToString();
                        _isSuccess = true;
                    }
                }
            }
            catch
            {
                _isSuccess = false;
            }
            return _isSuccess;
        }

        /// <summary>
        /// Gets the full name of agent.
        /// </summary>
        /// <param name="empId">The emp identifier.</param>
        /// <returns>System.String.</returns>
        private string GetFullNameOfAgent(string empId)
        {
            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPersonQuery personQuery = new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPersonQuery();
            personQuery.EmployeeId = empId;
            personQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPerson person = (Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPerson)ConfigContainer.Instance().ConfServiceObject.RetrieveObject(personQuery);
            string name = string.Empty;
            if (person == null) return string.Empty;
            if (!string.IsNullOrEmpty(person.FirstName))
                name = person.FirstName;
            if (!string.IsNullOrEmpty(person.LastName))
                name = (string.IsNullOrEmpty(person.LastName) ? "" : name + " ") + person.LastName;
            return name;
        }

        /// <summary>
        /// Gets the inprogess interaction count.
        /// </summary>
        private void getInprogessInteractionCount()
        {
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    _emailDetails.InProgressIXNCountVisibility = Visibility.Collapsed;
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetTotalInteractionCount(_contactID, _interactionID);
                    if (response != null)
                    {
                        if (response != null && response.Id == EventCountInteractions.MessageId)
                        {
                            EventCountInteractions eventInteractionListGet = (EventCountInteractions)response;
                            if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                            {
                                int inprogressIXNCount = 0;
                                inprogressIXNCount = (int)eventInteractionListGet.TotalCount.Value;
                                if (inprogressIXNCount > 0)
                                {
                                    _emailDetails.InprogressInteractionCount = "(" + inprogressIXNCount + ")";
                                    if (inprogressIXNCount > 1)
                                        _emailDetails.TotalInprogessIXNCount = inprogressIXNCount + " Interactions In Progress.";
                                    else
                                        _emailDetails.TotalInprogessIXNCount = inprogressIXNCount + " Interaction In Progress.";
                                    _emailDetails.InProgressIXNCountVisibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do getInprogessInteractionCount() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the recent interaction list.
        /// </summary>
        private void getRecentInteractionList()
        {
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    List<string> attributes = new List<string>();
                    attributes.Add("MediaTypeId");
                    attributes.Add("StartDate");
                    attributes.Add("Subject");
                    _emailDetails.RecentIXNListVisibility = Visibility.Collapsed;
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionList(_contactID, EmailDataContext.GetInstance().TenantDbId, _interactionID, attributes);
                    if (response != null)
                    {
                        if (response != null && response.Id == EventInteractionListGet.MessageId)
                        {
                            EventInteractionListGet eventInteractionListGet = (EventInteractionListGet)response;

                            if (eventInteractionListGet != null && eventInteractionListGet.InteractionData != null)
                            {
                                interactionDataList = eventInteractionListGet.InteractionData;
                                if (interactionDataList.Count > 0 && interactionDataList != null)
                                {
                                    int remaining = 0;
                                    _emailDetails.RecentIXNCount = "(" + (interactionDataList.Count) + ")";
                                    _emailDetails.RecentInteractionCount = (interactionDataList.Count) + " Recent Interactions in last day:";
                                    for (int listCount = 0; listCount < interactionDataList.Count; listCount++)
                                    {
                                        if (listCount < 5)
                                        {
                                            var startDate = string.Empty;
                                            var mediaType = string.Empty;
                                            var subject = string.Empty;
                                            foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute item in interactionDataList[listCount].Attributes)
                                            {
                                                switch (item.AttributeName.ToLower())
                                                {
                                                    case "subject":
                                                        subject = Convert.ToString(item.AttributeValue);
                                                        break;
                                                    case "startdate":
                                                        startDate = Convert.ToString(Convert.ToDateTime(item.AttributeValue.ToString()));
                                                        break;
                                                    case "mediatypeid":
                                                        mediaType = Convert.ToString(item.AttributeValue);
                                                        break;
                                                }
                                            }
                                            _emailDetails.RecentInteraction.Add(new RecentInteractions(mediaType, startDate, subject));
                                        }
                                        else
                                            remaining++;
                                        if (remaining > 0)
                                            _emailDetails.RemainingDetails = remaining + " more ...";
                                    }
                                    _emailDetails.RecentIXNListVisibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                    attributes.Clear();
                    attributes = null;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while do getRecentInteractionList() :" + generalException.ToString());
            }
        }

        private string GetReponseFieldCodeData(string renderText)
        {
            string renderedText = string.Empty;
            ContactProperties contactProperties = null;
            AgentProperties agentProperties = null;
            ContactInteractionProperties contactInteractionProperties = null;
            try
            {
                Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPerson Person = ConfigContainer.Instance().GetValue("CfgPerson");
                if (Person != null)
                {
                    agentProperties = new AgentProperties();
                    agentProperties.FirstName = Person.FirstName;
                    agentProperties.LastName = Person.LastName;
                    agentProperties.FullName = Person.FirstName + " " + Person.LastName;
                    agentProperties.Signature = "No Signature";
                }
                if (CustomerDetails.Count > 0)
                {
                    contactProperties = new ContactProperties();
                    contactProperties.Id = _contactID;

                    contactProperties.FirstName = CustomerDetails.ContainsKey("FirstName") ? CustomerDetails["FirstName"] : string.Empty;
                    contactProperties.LastName = CustomerDetails.ContainsKey("LastName") ? CustomerDetails["LastName"] : string.Empty;
                    contactProperties.FullName = CustomerDetails.ContainsKey("FullName") ? CustomerDetails["FullName"] : string.Empty;
                    contactProperties.Title = CustomerDetails.ContainsKey("Title") ? CustomerDetails["Title"] : string.Empty;
                    contactProperties.PrimaryEmailAddress = CustomerDetails.ContainsKey("PrimaryEmailAddress") ? CustomerDetails["PrimaryEmailAddress"] : string.Empty;
                    contactProperties.PrimaryPhoneNumber = CustomerDetails.ContainsKey("PrimaryPhoneNumber") ? CustomerDetails["PrimaryPhoneNumber"] : string.Empty;
                }

                contactInteractionProperties = new ContactInteractionProperties();
                contactInteractionProperties.Id = _interactionID;

                // For TimeZone
                if ((interactionTypes == InteractionTypes.EventInvite || interactionTypes == InteractionTypes.EventPull) && eventGetInteractionContent.InteractionAttributes.Timeshift != null)
                {
                    try
                    {
                        var timespan = TimeSpan.FromMinutes(Convert.ToDouble(eventGetInteractionContent.InteractionAttributes.Timeshift.Value));
                        contactInteractionProperties.TimeZone = "GMT" + (timespan.Hours >= 0 ? ("+" + timespan.Hours.ToString()) : timespan.Hours.ToString()) + ":" + Math.Abs(timespan.Minutes);
                        logger.Info("Timezone updated from server");
                    }
                    catch
                    {
                        contactInteractionProperties.TimeZone = "GMT" + DateTime.Now.ToLocalTime().ToString("zzz");
                        logger.Info("Current System timezone updated");
                    }
                }
                else
                {
                    contactInteractionProperties.TimeZone = "GMT" + DateTime.Now.ToLocalTime().ToString("zzz");
                    logger.Info("Current System timezone updated");
                }

                contactInteractionProperties.AttachedData = new PropertyList();
                for (int i = 0; i < dataUserControl.CurrentData.Count; i++)
                {
                    contactInteractionProperties.AttachedData.Add(new Property() { Name = dataUserControl.CurrentData.AllKeys[i].ToString(), Value = dataUserControl.CurrentData.AllValues[i].ToString() });
                }

                contactInteractionProperties.OtherProperties = new PropertyList();

                if (lblEmailIxnType.Text == "Inbound")
                {
                    contactInteractionProperties.FromAddress = inboundUserControl.txtInboundFrom.Text;
                    contactInteractionProperties.ToAddress = inboundUserControl.txtInboundTo.Text;
                    contactInteractionProperties.Subject = inboundUserControl.txtInboundSubject.Text;
                    contactInteractionProperties.StartDate = inboundUserControl.lblInboundDateTime.Text;
                }
                else if (lblEmailIxnType.Text == "Outbound")
                {
                    contactInteractionProperties.FromAddress = outboundUserControl.txtOutboundFrom.Text;
                    contactInteractionProperties.ToAddress = outboundUserControl.txtOutboundTo.Text;
                    contactInteractionProperties.Subject = outboundUserControl.txtOutboundSubject.Text;
                    contactInteractionProperties.StartDate = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(outboundUserControl.startDate), TimeZoneInfo.Local);// Convert.ToDateTime(outboundUserControl.startDate).ToUniversalTime;
                }

                // Date Created Attribute
                string timezone = contactInteractionProperties.TimeZone.Replace("GMT", "");
                string[] time = timezone.Split(':');
                NullableDateTime datecreated = contactInteractionProperties.StartDate.Value.AddMinutes(Convert.ToInt64(time[0]) > 0 ?
                    ((Convert.ToInt64(time[0]) * 60) + Convert.ToInt64(time[1])) : ((Convert.ToInt64(time[0]) * 60) - Convert.ToInt64(time[1])));
                contactInteractionProperties.OtherProperties.Add(new Property() { Name = "DateCreated", Value = datecreated.ToString() });

                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                    renderedText = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetResponseFieldContents(agentProperties, contactProperties, contactInteractionProperties, renderText);
                else
                    renderedText = string.Empty;
            }
            catch (Exception ex)
            {
                logger.Error("GetReponseFieldCodeData : " + (ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
                renderedText = string.Empty;
            }
            finally
            {
                contactProperties = null;
                agentProperties = null;
                contactInteractionProperties = null;
            }
            return renderedText;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns>EmailSignature.</returns>
        private EmailSignature GetSignature()
        {
            //string signature = string.Empty;
            bool isHTMLResponse = false;
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(IPlugins.Plugins.Contact))
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("email.enable.signature") && ((string)ConfigContainer.Instance().GetValue("email.enable.signature")).ToLower().Equals("true"))
                    {
                        //signature = ((IPlugins.IContactPlugin)EmailDataContext.GetInstance().HtPlugin[IPlugins.Plugins.Contact]).GetEmailSignature(EmailDataContext.GetInstance().EmailSignature, ref isHTMLResponse);
                        EmailSignature emailSignature = ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).GetEmailSignature((ConfigContainer.Instance().AllKeys.Contains("email.signature") ? (string)ConfigContainer.Instance().GetValue("email.signature") : string.Empty), ref isHTMLResponse);

                        //if (string.IsNullOrEmpty(signature))
                        if (emailSignature == null)
                        {
                            //signature = string.Empty;
                            ShowError("Error inserting signature: " + (ConfigContainer.Instance().AllKeys.Contains("email.signature") ? (string)ConfigContainer.Instance().GetValue("email.signature") : string.Empty) + "\" defined in option 'email.signature' not found.");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_contactID))
                            {
                                List<string> attribList = new List<string>();
                                attribList.Add("FirstName"); attribList.Add("LastName"); attribList.Add("Title");
                                EventGetAttributes eventGetAttribute = ContactServerHelper.RequestGetContactAttribute(_contactID, attribList);
                                if (eventGetAttribute != null)
                                {
                                    List<AttributesHeader> attributeHeader = eventGetAttribute.Attributes.Cast<AttributesHeader>().ToList();
                                    int count = attributeHeader.Count;
                                    string conFirstName = string.Empty;
                                    string conFullName = string.Empty;
                                    string conLastName = string.Empty;
                                    string conTitle = string.Empty;

                                    if (attributeHeader.Where(x => x.AttrName.Equals("FirstName")).ToList().Count > 0)
                                    {
                                        AttributesHeader firstNameHeader = attributeHeader.Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                                        if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                                        {
                                            conFirstName = conFullName = firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                                        }
                                    }
                                    if (attributeHeader.Where(x => x.AttrName.Equals("LastName")).ToList().Count > 0)
                                    {
                                        AttributesHeader LastNameHeader = attributeHeader.Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                                        if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                                        {
                                            conLastName = LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                                            if (string.IsNullOrEmpty(conFullName))
                                                conFullName = conLastName;
                                            else if (!string.IsNullOrEmpty(conLastName))
                                                conFullName += " " + conLastName;
                                        }
                                    }
                                    if (attributeHeader.Where(x => x.AttrName.Equals("Title")).ToList().Count > 0)
                                    {
                                        AttributesHeader titleHeader = attributeHeader.Where(x => x.AttrName.Equals("Title")).SingleOrDefault();
                                        if (titleHeader != null && titleHeader.AttributesInfoList.Count > 0)
                                        {
                                            conTitle = titleHeader.AttributesInfoList[0].AttrValue.ToString();
                                        }
                                    }
                                    emailSignature.EmailBody = emailSignature.EmailBody.Replace("<$ Contact.LastName $>", lastName).Replace("<$ Contact.FirstName $>", firstName);
                                    emailSignature.EmailBody = emailSignature.EmailBody.Replace("<$ Contact.FullName $>", conFullName).Replace("<$ Contact.LastName $>", conTitle);
                                    emailSignature.EmailBody = emailSignature.EmailBody.Replace("<$ Interaction.Subject $>", "");
                                }
                            }
                            return emailSignature;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
                //signature = string.Empty;
            }
            return null;
        }

        /// <summary>
        /// Handles the MouseEnter event of the imgEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void imgEmail_MouseEnter(object sender, MouseEventArgs e)
        {
            popupAgentInfo.IsOpen = true;
            popupAgentInfo.Focusable = false;
            popupAgentInfo.StaysOpen = true;
            popupAgentInfo.PlacementTarget = imgEmail;
        }

        /// <summary>
        /// Handles the MouseLeave event of the imgEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void imgEmail_MouseLeave(object sender, MouseEventArgs e)
        {
            popupAgentInfo.IsOpen = false;
        }

        //It receive the interaction server state from server. if it is false means(interaction server in not active), the window will close.
        /// <summary>
        /// Interactions the service_ interaction server notifier.
        /// </summary>
        /// <param name="isOpen">if set to <c>true</c> [is open].</param>
        void InteractionService_InteractionServerNotifier(bool isOpen)
        {
            Dispatcher.Invoke(
                 new Action(() =>
                 {
                     _isSaveMailToWorkbin = false;
                     // if (!isOpen)
                     //UpdatePendingStatus(false);
                     grdrow_error.Height = new GridLength(0);
                     this.Close();
                 }));
        }

        private bool IsEmailReachMaximumCount()
        {
            int maximumEmailCount = 5;

            if (ConfigContainer.Instance().AllKeys.Contains("email.max.intstance-count"))
                int.TryParse(((string)ConfigContainer.Instance().GetValue("email.max.intstance-count")), out maximumEmailCount);
            List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
            if (emailWindows.Count == maximumEmailCount)
            {
                ShowError("Email reached maximum count. Please close opened mail and then try to open.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the team communicator forward.
        /// </summary>
        /// <returns>System.Windows.Controls.UserControl.</returns>
        private System.Windows.Controls.UserControl LoadTeamCommunicatorForward()
        {
            System.Windows.Controls.UserControl teamCommunicatorForward = null;
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.Contact);
                container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Forward);
                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                container.ComposeParts(ImportClass);

                teamCommunicatorForward = (from d in ImportClass.win
                                           where d.Name == "TeamCommunicator"
                                           select d).FirstOrDefault() as System.Windows.Controls.UserControl;
            }
            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return teamCommunicatorForward;
        }

        /// <summary>
        /// Loads the team communicator transfer.
        /// </summary>
        /// <returns>System.Windows.Controls.UserControl.</returns>
        private System.Windows.Controls.UserControl LoadTeamCommunicatorTransfer()
        {
            System.Windows.Controls.UserControl teamCommunicatorTransfer = null;
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.Email);
                container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Transfer);
                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                container.ComposeParts(ImportClass);

                teamCommunicatorTransfer = (from d in ImportClass.win
                                            where d.Name == "TeamCommunicator"
                                            select d).FirstOrDefault() as System.Windows.Controls.UserControl;
            }
            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return teamCommunicatorTransfer;
        }

        /// <summary>
        /// Marks the done interaction.
        /// </summary>
        private void MarkDoneInteraction()
        {
            try
            {
                AddorUpdateCaseDatatoIxn();
                InteractionService interactionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues output = interactionService.StopProcessingInteraction(EmailDataContext.GetInstance().ProxyClientID, _interactionID);
                if (output.MessageCode == "200")
                {
                    ContactServerHelper.UpdateInteraction(_interactionID, eventGetInteractionContent.InteractionAttributes.OwnerId == null ? ConfigContainer.Instance().PersonDbId : 0, dataUserControl.notes, dataUserControl.CurrentData, 3, DateTime.UtcNow.ToString());
                    //UpdatePendingStatus(false);
                    NotifyHistoryRefresh(_interactionID, false);
                    _isSaveMailToWorkbin = false;
                    grdrow_error.Height = new GridLength(0);
                    this.Close();
                }
                else
                {
                    grdrow_error.Height = GridLength.Auto;
                    ShowError(output.Message.ToString());
                }

            }
            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the MouseLeftButtonDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MouseLeftButtonDown_click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_emailWindowState != EmailWindowState.Maximized)
                {
                    DragMove();
                    if (!(ConfigContainer.Instance().AllKeys.Contains("allow.system-draggable") &&
                            ((string)ConfigContainer.Instance().GetValue("allow.system-draggable")).ToLower().Equals("true")))
                    {
                        if (Left < 0)
                            Left = 0;
                        if (Top < 0)
                            Top = 0;
                        if (Left > SystemParameters.WorkArea.Right - Width)
                            Left = SystemParameters.WorkArea.Right - Width;
                        if (Top > SystemParameters.WorkArea.Bottom - Height)
                            Top = SystemParameters.WorkArea.Bottom - Height; ;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Notifies the history refresh.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="isDelete">if set to <c>true</c> [is delete].</param>
        /// 
        private void NotifyHistoryRefresh(string interactionId, bool isDelete)
        {
            //Added by Sakthi to Notify the History refresh.
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).RefreshContactHistory(interactionId, isDelete);
        }

        /// <summary>
        /// Replyors the reply all click.
        /// </summary>
        /// <param name="isReplyAll">if set to <c>true</c> [is reply all].</param>
        private void ReplyorReplyALLClick(bool isReplyAll)
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("email.default-queue") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("email.default-queue")))
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress")))
                    {
                        if (ConfigContainer.Instance().AllKeys.Contains("email.enable.move-inbound-to-in-progress-workbin-on-reply") && ((string)ConfigContainer.Instance().GetValue("email.enable.move-inbound-to-in-progress-workbin-on-reply")).ToLower().Equals("true"))
                        {
                            string inboundInteractionID = _interactionID;
                            string notes = dataUserControl.notes;
                            AddorUpdateCaseDatatoIxn();
                            KeyValueCollection kvpUserData = dataUserControl.CurrentData;
                            lblTabItemShowTimer.Text = "[00:00:00]";
                            ReplyorReplyALL(_interactionID, GetUserData(dataUserControl.CurrentData), isReplyAll);
                            InteractionService interactionService = new InteractionService();
                            Pointel.Interactions.Core.Common.OutputValues OutputValues = interactionService.PlaceInWorkbin(inboundInteractionID, EmailDataContext.GetInstance().AgentID, (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress") ? (string)ConfigContainer.Instance().GetValue("workbin.email.in-progress") : string.Empty), EmailDataContext.GetInstance().ProxyClientID);
                            if (OutputValues.MessageCode == "200")
                            {
                                ContactServerHelper.UpdateInteraction(inboundInteractionID, eventGetInteractionContent.InteractionAttributes.OwnerId == null ? ConfigContainer.Instance().PersonDbId : 0, notes, kvpUserData, 2);
                            }
                        }
                        else
                        {
                            if (IsEmailReachMaximumCount()) return;
                            EmailMainWindow emailMainWindow = new EmailMainWindow(_interactionID, isReplyAll);
                            emailMainWindow.Show();
                        }
                    }
                    else
                        ShowError("Action Aborted : \"workbin.email.in-progress\" option is not defined");
                }
                else
                    ShowError("Action Aborted : \"email.default-queue\" option is not defined");
            }
            catch (Exception exception)
            {
                logger.Error("Error occurred at btnReply_Click" + exception.ToString());
            }
        }

        /// <summary>
        /// Sends the interaction.
        /// </summary>
        private void SendInteraction()
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("email.outbound-queue") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("email.outbound-queue")))
                {
                    AddorUpdateCaseDatatoIxn();
                    AddorDeleteAttachments();
                    InteractionService interactionService = new InteractionService();
                    Pointel.Interactions.Core.Common.OutputValues OutputValues = interactionService.PlaceInQueue(_interactionID, EmailDataContext.GetInstance().ProxyClientID, (ConfigContainer.Instance().AllKeys.Contains("email.outbound-queue") ? (string)ConfigContainer.Instance().GetValue("email.outbound-queue") : string.Empty));
                    if (OutputValues.MessageCode == "200")
                    {
                        if (interactionTypes == InteractionTypes.EventPull || interactionTypes == InteractionTypes.Compose || interactionTypes == InteractionTypes.CreateReply)
                            UpdateOutboundInteraction(2, true);
                        //else
                        //{
                        //    bool isReply = false;
                        //    if (interactionTypes == InteractionTypes.CreateReply)
                        //        isReply = true;
                        //    InsertOutboundInteraction(isReply, true);
                        //}

                        // Finding Parent InteractionID(Inbound Mail) and stop processing(Mark Done)
                        if (!string.IsNullOrEmpty(_parentIxnID))
                        {
                            KeyValueCollection kvpixnIds = new KeyValueCollection();
                            kvpixnIds.Add("id", _parentIxnID);
                            OutputValues = interactionService.PullInteraction(EmailDataContext.GetInstance().TenantDbId, EmailDataContext.GetInstance().ProxyClientID, kvpixnIds);
                            if (OutputValues.MessageCode == "200")
                            {
                                EventPulledInteractions eventpullixnIds = (EventPulledInteractions)OutputValues.IMessage;
                                if (eventpullixnIds.Interactions != null && eventpullixnIds.Interactions.Count > 0)
                                {
                                    OutputValues = interactionService.StopProcessingInteraction(EmailDataContext.GetInstance().ProxyClientID, _parentIxnID);
                                    if (OutputValues.MessageCode == "200")
                                    {
                                        ContactServerHelper.UpdateInteraction(_parentIxnID, ConfigContainer.Instance().PersonDbId, string.Empty, null, 3, DateTime.UtcNow.ToString());
                                    }

                                }
                            }
                        }
                        //UpdatePendingStatus(false);
                        _isSaveMailToWorkbin = false;
                        grdrow_error.Height = new GridLength(0);
                        this.Close();
                    }
                    else
                    {
                        grdrow_error.Height = GridLength.Auto;
                        ShowError(OutputValues.Message.ToString());
                    }
                }
                else
                    ShowError("Action Aborted : \"email.outbound-queue\" option is not defined");
            }

            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Shows the default warning popup.
        /// </summary>
        private void ShowDefaultWarningPopup()
        {
            var showMessageBox = new MessageBox("Information",
                                        "Parent Inbound Email is not available anymore,Do you still want to delete the Email?", "Yes", "No", false);
            showMessageBox.Owner = this;
            showMessageBox.ShowDialog();
            if (showMessageBox.DialogResult == true)
            {
                DeleteInteraction(true);
            }
        }

        /// <summary>
        /// Showors the hide prompt.
        /// </summary>
        private void ShoworHidePrompt()
        {
            if (ConfigContainer.Instance().AllKeys.Contains("email.enable.prompt-for-done") && ((string)ConfigContainer.Instance().GetValue("email.enable.prompt-for-done")).ToLower().Equals("true"))
            {
                var showMessageBox = new MessageBox("Information",
                                                   "Are you sure want to mark done the interaction?", "Yes", "No", false);
                showMessageBox.Owner = this;
                showMessageBox.ShowDialog();
                if (showMessageBox.DialogResult == true)
                {
                    MarkDoneInteraction();
                }
            }
            else
            {
                MarkDoneInteraction();
            }
        }

        /// <summary>
        /// Start timer for error
        /// </summary>
        private void starttimerforerror()
        {
            try
            {
                if (_timerforcloseError == null)
                {
                    _timerforcloseError = null;
                    _timerforcloseError = new DispatcherTimer();
                    _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                    _timerforcloseError.Tick += new EventHandler(CloseError);
                    _timerforcloseError.Start();
                }
                else
                    _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }
        }

        /// <summary>
        /// Teams the communicator event notify.
        /// </summary>
        /// <param name="dictionaryValues">The dictionary values.</param>
        /// <returns>System.String.</returns>
        private string TeamCommunicatorEventNotify(Dictionary<string, string> dictionaryValues)
        {
            var threadTCtransferforward = new Thread((delegate()
            {
                try
                {
                    Dispatcher.Invoke((Action)(delegate
                        {
                            if (contextMenuTransfer.IsOpen)
                                contextMenuTransfer.IsOpen = false;
                            if (contextMenuTransfer.StaysOpen)
                                contextMenuTransfer.StaysOpen = false;
                        }
                    ));
                    TCEventNotify(dictionaryValues);
                }
                catch (Exception ex)
                {
                    logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                }
            }));
            threadTCtransferforward.Start();
            return string.Empty;
        }

        private void TCEventNotify(Dictionary<string, string> dictionaryValues)
        {

            if (dictionaryValues != null && dictionaryValues.Count > 0)
            {
                string InteractionType = "";
                string OperationType = "";
                string SearchedType = "";
                string Place = "";
                string SearchedValue = "";
                string transferEmployeeID = "";
                string emailAddress = "";

                if (dictionaryValues.ContainsKey("EmployeeId"))
                    transferEmployeeID = dictionaryValues["EmployeeId"];

                if (dictionaryValues.ContainsKey("InteractionType"))
                    InteractionType = dictionaryValues["InteractionType"];

                if (dictionaryValues.ContainsKey("OperationType"))
                    OperationType = dictionaryValues["OperationType"];

                if (dictionaryValues.ContainsKey("SearchedType"))
                    SearchedType = dictionaryValues["SearchedType"];

                if (dictionaryValues.ContainsKey("Place"))
                    Place = dictionaryValues["Place"];

                if (dictionaryValues.ContainsKey("UniqueIdentity"))
                    SearchedValue = dictionaryValues["UniqueIdentity"];

                if (dictionaryValues.ContainsKey("EmailAddress"))
                    emailAddress = dictionaryValues["EmailAddress"];

                Dispatcher.Invoke((Action)(delegate
                {


                    //Do the Email operation
                    if (InteractionType == Pointel.Interactions.IPlugins.InteractionType.Email.ToString())
                    {
                        if (OperationType == Pointel.Interactions.IPlugins.OperationType.Transfer.ToString())
                        {
                            if (SearchedType.ToString().Equals("Agent"))
                            {
                                if (!Place.Equals(null) || !Place.Equals(string.Empty) || Place != "")
                                {
                                    if (!String.IsNullOrEmpty(_interactionID))
                                    {
                                        AddorUpdateCaseDatatoIxn();
                                        InteractionService interactionService = new InteractionService();
                                        Pointel.Interactions.Core.Common.OutputValues output = interactionService.TransferInteractiontoAgent(EmailDataContext.GetInstance().ProxyClientID, _interactionID, transferEmployeeID);
                                        if (output.MessageCode == "200")
                                        {
                                            string notes = "Transferred  on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "  by " + EmailDataContext.GetInstance().Username + " - ";
                                            ContactServerHelper.UpdateInteraction(_interactionID, eventGetInteractionContent.InteractionAttributes.OwnerId == null ? ConfigContainer.Instance().PersonDbId : 0, !string.IsNullOrEmpty(dataUserControl.notes) ? dataUserControl.notes + "\n" + notes : notes, dataUserControl.CurrentData, 2);
                                            grdrow_error.Height = new GridLength(0);
                                            //UpdatePendingStatus(false);
                                            _isSaveMailToWorkbin = false;
                                            grdrow_error.Height = new GridLength(0);
                                            this.Close();
                                        }
                                        else
                                        {
                                            grdrow_error.Height = GridLength.Auto;
                                            if (output.Message.Contains("No answer"))
                                                ShowError("The target did not answer - Warning");
                                            else
                                                ShowError(output.Message.ToString());
                                        }
                                    }
                                }
                            }
                            else if (SearchedType.ToString().Equals("InteractionQueue") && SearchedValue != string.Empty)
                            {
                                if (!String.IsNullOrEmpty(_interactionID))
                                {
                                    AddorUpdateCaseDatatoIxn();
                                    InteractionService interactionService = new InteractionService();
                                    Pointel.Interactions.Core.Common.OutputValues output = interactionService.TransferInteractiontoQueue(EmailDataContext.GetInstance().ProxyClientID, _interactionID, SearchedValue);
                                    if (output.MessageCode == "200")
                                    {
                                        string notes = "Transferred  on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "  by " + EmailDataContext.GetInstance().Username + " -";
                                        ContactServerHelper.UpdateInteraction(_interactionID, eventGetInteractionContent.InteractionAttributes.OwnerId == null ? ConfigContainer.Instance().PersonDbId : 0, !string.IsNullOrEmpty(dataUserControl.notes) ? dataUserControl.notes + "\n" + notes : notes, dataUserControl.CurrentData, 2);
                                        grdrow_error.Height = new GridLength(0);
                                        //UpdatePendingStatus(false);
                                        _isSaveMailToWorkbin = false;
                                        grdrow_error.Height = new GridLength(0);
                                        this.Close();
                                    }
                                    else
                                    {
                                        grdrow_error.Height = GridLength.Auto;
                                        ShowError(output.Message.ToString());
                                    }
                                }
                            }
                        }
                    }
                    else if (InteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact.ToString())
                    {
                        if (ConfigContainer.Instance().AllKeys.Contains("email.forward-queue") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("email.forward-queue")))
                        {
                            //Do the forward operation
                            KeyValueCollection kvpForwardUserData = new KeyValueCollection();
                            kvpForwardUserData.Add("GD_ExternalAgentAddress", emailAddress);
                            kvpForwardUserData.Add("GD_OriginalAgentEmployeeId", EmailDataContext.GetInstance().AgentID);
                            kvpForwardUserData.Add("GD_TransferrerUserName", EmailDataContext.GetInstance().Username);
                            kvpForwardUserData.Add("IW_ExternalAgentAddress", emailAddress);
                            kvpForwardUserData.Add("IW_OriginalAgentEmployeeId", EmailDataContext.GetInstance().AgentID);
                            kvpForwardUserData.Add("IW_TransferrerUserName", EmailDataContext.GetInstance().Username);

                            dataUserControl.CurrentData.Add("GD_ExternalAgentAddress", emailAddress);
                            dataUserControl.CurrentData.Add("GD_OriginalAgentEmployeeId", EmailDataContext.GetInstance().AgentID);
                            dataUserControl.CurrentData.Add("GD_TransferrerUserName", EmailDataContext.GetInstance().Username);
                            dataUserControl.CurrentData.Add("IW_ExternalAgentAddress", emailAddress);
                            dataUserControl.CurrentData.Add("IW_OriginalAgentEmployeeId", EmailDataContext.GetInstance().AgentID);
                            dataUserControl.CurrentData.Add("IW_TransferrerUserName", EmailDataContext.GetInstance().Username);

                            InteractionService interactionService = new InteractionService();
                            Pointel.Interactions.Core.Common.OutputValues output = interactionService.AddCaseDataProperties(_interactionID, EmailDataContext.GetInstance().ProxyClientID, kvpForwardUserData);
                            AddorUpdateCaseDatatoIxn();
                            if (output.MessageCode == "200")
                            {
                                output = interactionService.PlaceInQueue(_interactionID, EmailDataContext.GetInstance().ProxyClientID, (ConfigContainer.Instance().AllKeys.Contains("email.forward-queue") ? (string)ConfigContainer.Instance().GetValue("email.forward-queue") : string.Empty));
                                if (output.MessageCode == "200")
                                {
                                    string notes = "Forwarded to external resource on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "  by " + EmailDataContext.GetInstance().Username + " -";
                                    ContactServerHelper.UpdateInteraction(_interactionID, eventGetInteractionContent.InteractionAttributes.OwnerId == null ? ConfigContainer.Instance().PersonDbId : 0, !string.IsNullOrEmpty(dataUserControl.notes) ? dataUserControl.notes + "\n" + notes : notes, dataUserControl.CurrentData, 2);
                                    //UpdatePendingStatus(false);
                                    _isSaveMailToWorkbin = false;
                                    grdrow_error.Height = new GridLength(0);
                                    this.Close();
                                }
                                else
                                {
                                    grdrow_error.Height = GridLength.Auto;
                                    ShowError(output.Message.ToString());
                                }
                            }
                        }
                        else
                            ShowError("Action Aborted : \"email.forward-queue\" option is not defined");

                    }
                }));
            }
        }

        /// <summary>
        /// Updates the pending status.
        /// </summary>
        /// <param name="isPending">if set to <c>true</c> [is pending].</param>
        private void UpdatePendingStatus(bool isPending)
        {
            if (EmailDataContext.GetInstance().MessageToClientEmail != null)
            {
                if (isPending)
                    EmailDataContext.GetInstance().MessageToClientEmail.PluginInteractionStatus(PluginType.Email, IXNState.Opened);
                else
                    EmailDataContext.GetInstance().MessageToClientEmail.PluginInteractionStatus(PluginType.Email, IXNState.Closed, EmailDataContext.GetInstance().isNotifyShowing);
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the userControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void userControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (contextMenuTransfer.IsOpen)
                //    contextMenuTransfer.IsOpen = false;
                //if (contextMenuTransfer.StaysOpen)
                //    contextMenuTransfer.StaysOpen = false;
            }
            catch (Exception ex)
            {
                logger.Error("userControl_Unloaded() " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// WNDs the proc.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_SYSCOMMAND) return IntPtr.Zero;
            //if (wParam.ToInt32().ToString() != "0" && wParam.ToInt32().ToString() != "1" && wParam.ToInt32().ToString() != "32")
            //    System.Windows.MessageBox.Show(wParam.ToInt32().ToString());
            //if (wParam.ToInt32().ToString() == "61536")
            //{
            //string d = "d";
            //}
            switch (wParam.ToInt32())
            {
                case CU_Maximize:
                    if (WindowState != System.Windows.WindowState.Maximized)
                    {
                        if (WindowState == System.Windows.WindowState.Minimized)
                        {
                            WindowState = System.Windows.WindowState.Normal;
                        }
                        btnMaximize_Click(null, null);
                        handled = true;
                    }
                    break;
                case CU_Minimize:
                    if (WindowState != System.Windows.WindowState.Minimized)
                    {
                        WindowState = System.Windows.WindowState.Minimized;
                    }

                    break;
                case CU_Restore:
                    if (WindowState != System.Windows.WindowState.Normal)
                        WindowState = System.Windows.WindowState.Normal;
                    else if (_emailWindowState == EmailWindowState.Maximized)
                    {
                        WindowState = System.Windows.WindowState.Normal;
                        btnMaximize_Click(null, null);
                        handled = true;
                    }
                    break;
                case CU_Close: //close
                    btnExit_Click(null, null);
                    handled = true;
                    break;

                default:
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the Click event of the _itemPhoneNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void _itemPhoneNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.MenuItem _itemPhoneNumber = (System.Windows.Controls.MenuItem)sender;
                EmailDataContext.GetInstance().MessageToClientEmail.PluginDialEvents(PluginType.Workbin, _itemPhoneNumber.Tag.ToString());
                EmailDataContext.GetInstance().MessageToClientEmail.PluginDialEvents(PluginType.Workbin, "Dial");
            }
            catch (Exception exception)
            {
                logger.Error("_itemPhoneNumber_Click:" + exception.ToString());
            }
            finally
            {
                //GC.Collect();
            }
        }

        #endregion Methods

        #region Other

        //End
        //public bool ValidateEmailAddress(string email)
        //{
        //    email = email.Replace(";", ",");
        //    string[] emailAddress = (email.EndsWith(",") ? email.Remove(email.Length - 1) : email).Split(',');
        //    bool status = true;
        //    foreach (string address in emailAddress)
        //    {
        //        status = System.Text.RegularExpressions.Regex.IsMatch(address.TrimStart().TrimEnd(), @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        //        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
        //        if (!status)
        //            break;
        //    }
        //    return status;
        //}

        #endregion Other
    }
}