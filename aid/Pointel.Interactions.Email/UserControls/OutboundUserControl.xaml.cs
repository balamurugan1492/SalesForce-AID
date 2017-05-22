#region Header

/*
* ======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on :
* Author     : Vinoth and Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

#endregion Header

namespace Pointel.Interactions.Email.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;

    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;

    using Microsoft.Win32;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Email.DataContext;
    using Pointel.Interactions.Email.Forms;
    using Pointel.Interactions.Email.Helper;
    using Pointel.Interactions.IPlugins;
    using Pointel.Windows.Views.Common.Editor.Controls;

    /// <summary>
    /// Interaction logic for OutboundUserControl.xaml
    /// </summary>
    public partial class OutboundUserControl : UserControl
    {
        #region Fields

        //User controls
        public HTMLEditor htmlEditor = null;
        public string startDate;

        private ContactDirectoryWindow contactDirectory = null;
        private string defaultFromAddress = string.Empty;
        private EventGetDocument eventGetDocument = null;
        string filePath = string.Empty;

        //Logger
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private Window parent = null;

        //Attachments
        private string _fileExtension = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundUserControl"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="toAddress">To address.</param>
        /// <param name="signature">The signature.</param>
        public OutboundUserControl(Window parent, string toAddress, string contactName, EmailSignature signature = null, EventGetInteractionContent eventGetInteractionContent = null)
        {
            try
            {
                InitializeComponent();
                this.parent = parent;
                colAttachError.Width = new GridLength(0);
                dockOutboundAttachments.Background = Brushes.White;
                attscroll.Background = Brushes.White;
                //Add StartDate
                startDate = DateTime.Now.ToString("G");

                string sign = string.Empty;
                if (signature != null)
                {
                    signature.EmailBody = signature.EmailBody.Replace("\r\n", "<br />");
                    signature.EmailBody = signature.EmailBody.Replace("\n", "<br />");
                    sign = (string.IsNullOrEmpty(signature.EmailBody) ? string.Empty : ("<br/><br/>" + signature.EmailBody));
                }

                if (eventGetInteractionContent == null)
                {
                    //Add From and To address
                    SetFromAddress(ConfigContainer.Instance().AllKeys.Contains("email.compose.default-from-address") ?
                                    ConfigContainer.Instance().GetAsString("email.compose.default-from-address") : string.Empty);
                    txtOutboundTo.Text = toAddress;
                    gridCc.Visibility = Visibility.Collapsed;
                    gridBcc.Visibility = Visibility.Collapsed;

                    //Add Signature attachments
                    if (signature != null)
                    {
                        if (signature.AttachmentList != null)
                            AddAttachmentListfromServer(signature.AttachmentList);
                    }

                    //Set Email content
                    htmlEditor = new HTMLEditor(true, (ConfigContainer.Instance().AllKeys.Contains("email.enable.html-format") &&
                                    ((string)ConfigContainer.Instance().GetValue("email.enable.html-format")).ToLower().Equals("true")),
                                    sign);
                    dockOutboundContent.Children.Clear();
                    dockOutboundContent.Children.Add(htmlEditor);
                }
                else
                {
                    //Resend Functionality

                    //Add Subject
                    if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.Subject))
                        txtOutboundSubject.Text = eventGetInteractionContent.InteractionAttributes.Subject;

                    //Assign From,To,Cc and Bcc
                    AssignFromToCcandBcc(eventGetInteractionContent.EntityAttributes, true);

                    //Add mail Attachments and Signature attachments
                    AttachmentList attachmentList = new AttachmentList();
                    //Add Signature attachments
                    if (signature != null && signature.AttachmentList != null)
                    {
                        for (int i = 0; i < signature.AttachmentList.Count; i++)
                        {
                            attachmentList.Add(signature.AttachmentList[i]);
                        }
                    }
                    //Add Email attachments
                    if (eventGetInteractionContent.Attachments != null)
                    {
                        for (int i = 0; i < eventGetInteractionContent.Attachments.Count; i++)
                        {
                            attachmentList.Add(eventGetInteractionContent.Attachments[i]);
                        }
                    }
                    if (attachmentList != null)
                        AddAttachmentListfromServer(attachmentList);

                    // Frame Email Content
                    if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                        FrameEmailContent(eventGetInteractionContent.InteractionContent.StructuredText, true, true, sign, eventGetInteractionContent, contactName);
                    else if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.Text))
                        FrameEmailContent(eventGetInteractionContent.InteractionContent.Text, false, true, sign, eventGetInteractionContent, contactName);
                    else
                        FrameEmailContent(string.Empty, false, true, sign, eventGetInteractionContent, contactName);

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at OutboundUserControl constructor" + ex.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundUserControl"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="eventGetInteractionContent">Content of the event get interaction.</param>
        /// <param name="isReplyALL">if set to <c>true</c> [is reply all].</param>
        /// <param name="contactName">Name of the contact.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        /// <param name="signature">The signature.</param>
        public OutboundUserControl(Window parent, EventGetInteractionContent eventGetInteractionContent, bool isReplyALL, string contactName, bool isNew, EmailSignature signature = null)
        {
            try
            {
                InitializeComponent();
                this.parent = parent;
                colAttachError.Width = new GridLength(0);
                dockOutboundAttachments.Background = Brushes.White;
                attscroll.Background = Brushes.White;

                if (eventGetInteractionContent != null)
                {
                    if (isNew)
                    {
                        //Add StartDate
                        startDate = DateTime.Now.ToString("G");

                        //Add Subject
                        if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.Subject))
                            AddReplyPrefixwithSubject(eventGetInteractionContent.InteractionAttributes.Subject.Trim());

                        //Assign From,To,Cc and Bcc
                        AssignFromToCcandBcc(eventGetInteractionContent.EntityAttributes, isReplyALL);

                        //Get Signature content
                        string sign = string.Empty;
                        if (signature != null)
                        {
                            signature.EmailBody = signature.EmailBody.Replace("\r\n", "<br />");
                            signature.EmailBody = signature.EmailBody.Replace("\n", "<br />");
                            sign = (string.IsNullOrEmpty(signature.EmailBody) ? string.Empty : ("<br/><br/>" + signature.EmailBody));

                            //Add Signature attachments
                            if (signature.AttachmentList != null)
                                AddAttachmentListfromServer(signature.AttachmentList);
                        }

                        // Frame Email Content
                        if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                            FrameEmailContent(eventGetInteractionContent.InteractionContent.StructuredText, true, true, sign, eventGetInteractionContent, contactName);
                        else if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.Text))
                            FrameEmailContent(eventGetInteractionContent.InteractionContent.Text, false, true, sign, eventGetInteractionContent, contactName);
                        else
                            FrameEmailContent(string.Empty, false, true, sign, eventGetInteractionContent, contactName);
                    }
                    else
                    {
                        //Add StartDate
                        if (eventGetInteractionContent.InteractionAttributes.StartDate != null && eventGetInteractionContent.InteractionAttributes.StartDate.ToString() != string.Empty)
                            startDate = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetLocalDateTime(eventGetInteractionContent.InteractionAttributes.StartDate.ToString());

                        //Add Subject
                        txtOutboundSubject.Text = eventGetInteractionContent.InteractionAttributes.Subject;

                        //Assign From,To,Cc and Bcc
                        AssignFromToCcandBcc(eventGetInteractionContent.EntityAttributes, isReplyALL);

                        //Add Attachments
                        if (eventGetInteractionContent.Attachments != null)
                            AddAttachmentListfromServer(eventGetInteractionContent.Attachments, false);

                        // Frame Email Content
                        if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                            FrameEmailContent(eventGetInteractionContent.InteractionContent.StructuredText, true, false);
                        else if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.Text))
                            FrameEmailContent(eventGetInteractionContent.InteractionContent.Text, false, false);
                        else
                            FrameEmailContent(string.Empty, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at OutboundUserControl constructor" + ex.ToString());
            }
        }

        #endregion Constructors

        #region Properties

        public List<string> AddedAttachDocPath
        {
            get;
            private set;
        }

        public List<string> DeletedAttachDocId
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the attachment list from server.
        /// </summary>
        /// <param name="_attachmentList">The _attachment list.</param>
        /// <param name="isAdd">if set to <c>true</c> [is add].</param>
        public void AddAttachmentListfromServer(AttachmentList _attachmentList, bool isAdd = true)
        {
            var finalError = string.Empty;
            var invalidFileError = string.Empty;
            var maxsizeError = string.Empty;
            int emailMaxAttachmentSize = EmailServerDetails.EmailMaxAttachmentSize();

            foreach (Attachment attachmentDetails in _attachmentList)
            {
                // Get File Extension ex: .txt,.xml,etc...
                var fileExt = Path.GetExtension(attachmentDetails.TheName);

                // Check the extension is restricted or not
                string[] RestrictAttachFileType = ConfigContainer.Instance().AllKeys.Contains("email.restricted-attachment-file-types") ? ((string)ConfigContainer.Instance().GetValue("email.restricted-attachment-file-types")).Replace(" ", "").Split(',') : null;
                if (RestrictAttachFileType.Contains(fileExt.Replace(".", "")) && string.IsNullOrEmpty(invalidFileError))
                    invalidFileError = "File with extensions (" + String.Join(", ", RestrictAttachFileType.Select(p => p.ToString()).ToArray()) + ") is restricted to attach with this mail.";

                if (!RestrictAttachFileType.Contains(fileExt.Replace(".", "")))
                {
                    // Add Attachments
                    if (AddAttachments(attachmentDetails.TheName, (Int64)attachmentDetails.TheSize, attachmentDetails.DocumentId, isAdd))
                        (this.parent as EmailMainWindow).totalfilelength += (Int64)attachmentDetails.TheSize;

                    //Check Maximum Size
                    if (emailMaxAttachmentSize > 0 && CheckAttachmentSize(false))
                        maxsizeError = "Maximum upload limit is restricted to " + emailMaxAttachmentSize + " MB.";
                    else
                    {
                        // Hide error message for attachment
                        colAttachError.Width = new GridLength(0);
                        dockOutboundAttachments.Background = Brushes.White;
                        attscroll.Background = Brushes.White;
                    }
                }
            }

            if (!string.IsNullOrEmpty(invalidFileError) && !string.IsNullOrEmpty(maxsizeError))
                finalError = invalidFileError + Environment.NewLine + maxsizeError;
            else
                finalError = invalidFileError + maxsizeError;
            (this.parent as EmailMainWindow).ShowError(finalError);
        }

        /// <summary>
        /// Adds the standard response.
        /// </summary>
        /// <param name="responseContent">Content of the response.</param>
        /// <param name="attachmentList">The attachment list.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AddStandardResponse(string responseContent, AttachmentList attachmentList)
        {
            bool isAdded = false;
            try
            {
                //Adding Response content to Email content
                htmlEditor.AddResponseContent(responseContent);

                //Adding Attachment from list
                if (attachmentList != null)
                    AddAttachmentListfromServer(attachmentList);
            }
            catch (Exception ex)
            {
                logger.Error("Error while adding standard response :" + ex.Message);
            }
            return isAdded;
        }

        /// <summary>
        /// Checks the size of the attachment.
        /// </summary>
        /// <param name="isShowError">if set to <c>true</c> [is show error].</param>
        /// <returns></returns>
        public bool CheckAttachmentSize(bool isShowError)
        {
            if (!((((this.parent as EmailMainWindow).totalfilelength / 1024f) / 1024f) <= EmailServerDetails.EmailMaxAttachmentSize()))
            {
                colAttachError.Width = new GridLength(25);
                dockOutboundAttachments.Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#FFFFCC")));
                attscroll.Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#FFFFCC")));
                if (isShowError)
                    (this.parent as EmailMainWindow).ShowError("Maximum upload limit is restricted to " + EmailServerDetails.EmailMaxAttachmentSize() + " MB.");
                return true;
            }
            else
            {
                // Hide Attachment error message
                colAttachError.Width = new GridLength(0);
                dockOutboundAttachments.Background = Brushes.White;
                attscroll.Background = Brushes.White;
                (this.parent as EmailMainWindow).CloseError(null, null);
                return false;
            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="Bytes">The bytes.</param>
        /// <returns></returns>
        public string GetFileSize(long Bytes)
        {
            if (Bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(Bytes, 1073741824);
                return String.Format("{0:##.##} GB", size);
            }
            else if (Bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(Bytes, 1048576);
                return String.Format("{0:##.##} MB", size);
            }
            else if (Bytes >= 1024)
            {
                Decimal size = Decimal.Divide(Bytes, 1024);
                return String.Format("{0:##.##} KB", size);
            }
            else if (Bytes > 0 & Bytes < 1024)
            {
                Decimal size = Bytes;
                return String.Format("{0:##.##} Bytes", size);
            }
            else
            {
                return "0 Bytes";
            }
        }

        /// <summary>
        /// Prints the inbound mail.
        /// </summary>
        public void PrintOutboundMail()
        {
            try
            {
                Paragraph normalMessageParagraph = new Paragraph();
                if (!string.IsNullOrEmpty(txtOutboundSubject.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Subject :")));
                    normalMessageParagraph.Inlines.Add(txtOutboundSubject.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                normalMessageParagraph.Inlines.Add(new Bold(new Run("From :")));
                normalMessageParagraph.Inlines.Add(txtOutboundFrom.Text);
                normalMessageParagraph.Inlines.Add(new LineBreak());
                if (!string.IsNullOrEmpty(startDate))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Date :")));
                    normalMessageParagraph.Inlines.Add(startDate);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                normalMessageParagraph.Inlines.Add(new Bold(new Run("To :")));
                normalMessageParagraph.Inlines.Add(txtOutboundTo.Text);
                normalMessageParagraph.Inlines.Add(new LineBreak());
                if (!string.IsNullOrEmpty(txtOutboundCc.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Cc :")));
                    normalMessageParagraph.Inlines.Add(txtOutboundCc.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                if (!string.IsNullOrEmpty(txtOutboundBcc.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Bcc :")));
                    normalMessageParagraph.Inlines.Add(txtOutboundBcc.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                htmlEditor.Print(normalMessageParagraph);
            }
            catch (Exception ex)
            {
                logger.Error("Error while printing outbound Mail" + ex.Message);
            }
        }

        /// <summary>
        /// Adds the attachments.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="size">The size.</param>
        /// <param name="docId">The document identifier.</param>
        internal bool AddAttachments(string fileName, Int64 size, string docId = null, bool isAdd = true)
        {
            string tagCheck = fileName;
            if (docId != null)
                tagCheck = docId;

            // Check the Attachments is already exists or not
            UIElement oldattachment = new UIElement();
            foreach (UIElement item in dockOutboundAttachments.Children)
            {
                if (item is Button)
                {
                    string pathordocId = (item as Button).Tag.ToString();
                    if ((pathordocId.Split('|')[0] == tagCheck) && docId == null)
                    {
                        oldattachment = item;
                        if (this.parent is EmailMainWindow)
                            (this.parent as EmailMainWindow).totalfilelength -= Convert.ToInt64(pathordocId.Split('|')[1]);
                        break;
                    }
                    else
                    {
                        pathordocId = pathordocId.Split('|')[0];
                        if (pathordocId == tagCheck)
                            return false;
                    }
                }
            }

            //Show error msg for file has been replaced with file name and size
            dockOutboundAttachments.Children.Remove(oldattachment);

            dockOutboundAttachments.Children.Add(LoadAttachments(fileName, size, docId, isAdd));
            attscroll.ScrollToBottom();
            return true;
        }

        private void AddReplyPrefixwithSubject(string subject)
        {
            string prefix = "Re:";

            if (ConfigContainer.Instance().AllKeys.Contains("email.reply-prefix"))
                prefix = string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("email.reply-prefix")) ? "Re:" : (string)ConfigContainer.Instance().GetValue("email.reply-prefix");

            prefix = prefix.Replace("<SPACE>", "");
            if (subject.Trim().StartsWith(prefix, true, null))
                txtOutboundSubject.Text = subject.Trim();
            else if (subject.Trim().StartsWith("Fwd:", true, null))
                txtOutboundSubject.Text = prefix + subject.Trim().Substring(4);
            else if (subject.Trim().StartsWith("Fw:", true, null))
                txtOutboundSubject.Text = prefix + subject.Trim().Substring(3);
            else if (subject.Trim().StartsWith("Re:", true, null))
                txtOutboundSubject.Text = prefix + subject.Trim().Substring(3);
            else
                txtOutboundSubject.Text = prefix + subject.Trim();
        }

        private void AssignFromToCcandBcc(BaseEntityAttributes baseEntityAttributes, bool isReplyALL)
        {
            string toAddress = string.Empty;
            string ccAddress = string.Empty;
            string bccAddress = string.Empty;
            if (baseEntityAttributes is EmailInEntityAttributes)
            {
                EmailInEntityAttributes emailInEntityAttributes = (EmailInEntityAttributes)baseEntityAttributes;
                toAddress = emailInEntityAttributes.FromAddress;
                SetFromAddress(emailInEntityAttributes.Mailbox);
                ccAddress = emailInEntityAttributes.CcAddresses;
                bccAddress = emailInEntityAttributes.BccAddresses;
            }
            if (baseEntityAttributes is EmailOutEntityAttributes)
            {
                EmailOutEntityAttributes emailOutEntityAttributes = (EmailOutEntityAttributes)baseEntityAttributes;
                toAddress = emailOutEntityAttributes.ToAddresses;
                SetFromAddress(emailOutEntityAttributes.FromAddress);
                ccAddress = emailOutEntityAttributes.CcAddresses;
                bccAddress = emailOutEntityAttributes.BccAddresses;
            }

            txtOutboundTo.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(toAddress);
            if (isReplyALL)
            {
                btnAddCcandBcc.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(ccAddress))
                {
                    txtOutboundCc.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(ccAddress);
                }
                else
                {
                    gridCc.Visibility = Visibility.Collapsed;
                    btnAddCcandBcc.Content = "Add Cc";
                    btnAddCcandBcc.Visibility = Visibility.Visible;
                }
                if (!string.IsNullOrEmpty(bccAddress))
                {
                    txtOutboundBcc.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(bccAddress);
                }
                else
                {
                    gridBcc.Visibility = Visibility.Collapsed;
                    if (btnAddCcandBcc.Visibility == Visibility.Collapsed)
                    {
                        btnAddCcandBcc.Content = "Add Bcc";
                        btnAddCcandBcc.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                gridCc.Visibility = Visibility.Collapsed;
                gridBcc.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the attscroll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void attscroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            attscroll.SizeChanged -= attscroll_SizeChanged;
            if (attscroll.ActualWidth > 20)
                dockOutboundAttachments.Width = attscroll.ActualWidth - 20;
            attscroll.SizeChanged += new SizeChangedEventHandler(attscroll_SizeChanged);
        }

        /// <summary>
        /// Handles the Click event of the btnAddCcandBcc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAddCcandBcc_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;
            switch (obj.Content.ToString())
            {
                case "Add Cc":
                    txtOutboundCc.Text = string.Empty;
                    gridCc.Visibility = Visibility.Visible;
                    if (gridBcc.Visibility == Visibility.Collapsed)
                        obj.Content = "Add Bcc";
                    else
                    {
                        obj.Visibility = Visibility.Collapsed;
                        btnMenuCCBcc.Visibility = Visibility.Collapsed;
                    }

                    break;
                case "Add Bcc":
                    txtOutboundBcc.Text = string.Empty;
                    gridBcc.Visibility = Visibility.Visible;
                    obj.Visibility = Visibility.Collapsed;
                    btnMenuCCBcc.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAttachment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Handles the Click event of the btnBccDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBccDelete_Click(object sender, RoutedEventArgs e)
        {
            switch (btnAddCcandBcc.Visibility)
            {
                case Visibility.Collapsed: btnAddCcandBcc.Content = "Add Bcc";
                    btnAddCcandBcc.Visibility = Visibility.Visible;
                    btnMenuCCBcc.Visibility = Visibility.Visible;
                    break;
                case Visibility.Visible: if (btnAddCcandBcc.Content != "Add Cc")
                        btnAddCcandBcc.Content = "Add Bcc";
                    break;
            }
            txtOutboundBcc.Text = string.Empty;
            gridBcc.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the btnCcDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCcDelete_Click(object sender, RoutedEventArgs e)
        {
            switch (btnAddCcandBcc.Visibility)
            {
                case Visibility.Collapsed: btnAddCcandBcc.Content = "Add Cc";
                    btnAddCcandBcc.Visibility = Visibility.Visible;
                    btnMenuCCBcc.Visibility = Visibility.Visible;
                    break;
                case Visibility.Visible: btnAddCcandBcc.Content = "Add Cc";
                    break;
            }
            txtOutboundCc.Text = string.Empty;
            gridCc.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Click event of the btnMenuCCBcc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMenuCCBcc_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnuAddCc = new MenuItem();
            mnuAddCc.Header = "Add Cc";
            mnuAddCc.Click += new RoutedEventHandler(mnuAddCc_Click);

            MenuItem mnuAddBcc = new MenuItem();
            mnuAddBcc.Header = "Add Bcc";
            mnuAddBcc.Click += new RoutedEventHandler(mnuAddBcc_Click);

            ContextMenu menu = new ContextMenu();
            if (gridCc.Visibility == Visibility.Collapsed)
                menu.Items.Add(mnuAddCc);
            if (gridBcc.Visibility == Visibility.Collapsed)
                menu.Items.Add(mnuAddBcc);
            menu.PlacementTarget = sender as Button;
            menu.IsOpen = true;
        }

        /// <summary>
        /// Handles the Click event of the btnOutboundBcc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOutboundBcc_Click(object sender, RoutedEventArgs e)
        {
            ShowContactDirectory();
        }

        /// <summary>
        /// Handles the Click event of the btnOutboundCc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOutboundCc_Click(object sender, RoutedEventArgs e)
        {
            ShowContactDirectory();
        }

        /// <summary>
        /// Handles the Click event of the btnOutboundTo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOutboundTo_Click(object sender, RoutedEventArgs e)
        {
            ShowContactDirectory();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbFromAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmbFromAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            string value = ((ComboBoxItem)cmbFromAddress.SelectedItem).Tag.ToString();
            txtOutboundFrom.Text = value;
        }

        /// <summary>
        /// Converts to HTML tags.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        private string ConvertToHtmlTags(string message)
        {
            try
            {
                message = message.Replace("<", "&lt;");
                message = message.Replace(">", "&gt;");
                // message = message.Replace("&", "&amp;");    // No query string breaks
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at ConvertToHtmlTags " + ex.Message);
            }
            return message;
        }

        void EmailServerDetails__OnFromAddressChanged()
        {
            try
            {
                SetFromAddress(defaultFromAddress);
                Pointel.Interactions.Email.Helper.EmailServerDetails.OnFromAddressChanged -= EmailServerDetails__OnFromAddressChanged;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at EmailServerDetails__notifyFromAddress " + ex.Message);
            }
        }

        private void FrameEmailContent(string content, bool isHTML, bool isNew, string signature = null, EventGetInteractionContent eventGetInteractionContent = null, string contactName = null)
        {
            if (isNew)
            {
                if (ConfigContainer.Instance().AllKeys.Contains("email.enable.include-original-text-in-reply") &&
                                ((string)ConfigContainer.Instance().GetValue("email.enable.include-original-text-in-reply")).ToLower().Equals("true"))
                {
                    string QuoteChar = (ConfigContainer.Instance().AllKeys.Contains("email.quote-char") ? (string)ConfigContainer.Instance().GetValue("email.quote-char") : string.Empty);
                    string QuoteSeperator = (ConfigContainer.Instance().AllKeys.Contains("email.quote-separator") ? (string)ConfigContainer.Instance().GetValue("email.quote-separator") : string.Empty);
                    if (string.IsNullOrEmpty(QuoteChar.Trim()))
                        QuoteChar = ">";
                    if (string.IsNullOrEmpty(QuoteSeperator.Trim()))
                        QuoteSeperator = "---------Original Message---------";
                    if (!isHTML)
                    {
                        content = ConvertToHtmlTags(content);
                        string quoteChar = "<br/>" + ConvertToHtmlTags(QuoteChar);
                        content = content.Replace("\r\n", quoteChar);
                        content = content.Replace("\n", quoteChar);
                        content = QuoteChar + content;
                        content = content.Replace(Environment.NewLine, quoteChar);
                    }
                    content = "<div>" + signature + "</div><br/>" +
                              "<div>" + QuoteSeperator + "</div><br/>" +
                               GetQuoteDetails(eventGetInteractionContent) + "<br/>" +
                              "<div>" + GetQuoteHeader(contactName) + "</div>" +
                              "<div>" + content + "</div>";
                }
                else
                {
                    string orginalEmailContent = string.Empty;
                    orginalEmailContent = GetQuoteHeader(contactName) + "<br/><br/>" + content;
                    if (string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                    {
                        orginalEmailContent = orginalEmailContent.Replace("\r\n", "<br />");
                        orginalEmailContent = orginalEmailContent.Replace("\n", "<br />");
                    }
                    HTMLEditor htmlEditorforOrginalEmail = new HTMLEditor(false, true, orginalEmailContent);
                    dpOrginalEmail.Children.Clear();
                    dpOrginalEmail.Children.Add(htmlEditorforOrginalEmail);
                    expOrginalEmail.Visibility = Visibility.Visible;
                    content = signature + "<br/><br/>";
                }
                htmlEditor = new HTMLEditor(true, ReplyFormat(isHTML), content);
                dockOutboundContent.Children.Clear();
                dockOutboundContent.Children.Add(htmlEditor);
            }
            else
            {
                if (!isHTML)
                {
                    content = content.Replace("\r\n", "<br />");
                    content = content.Replace("\n", "<br />");
                }
                htmlEditor = new HTMLEditor(true, isHTML, content);
                dockOutboundContent.Children.Clear();
                dockOutboundContent.Children.Add(htmlEditor);
            }
        }

        /// <summary>
        /// Gets the attachment menus.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <returns>System.Windows.Controls.ContextMenu.</returns>
        private System.Windows.Controls.ContextMenu GetAttachmentMenus(UIElement uiElement)
        {
            System.Windows.Controls.ContextMenu conMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
            open.Header = "Open";
            open.Click += new RoutedEventHandler(MenuItem_Click);
            conMenu.Items.Add(open);

            System.Windows.Controls.MenuItem save = new System.Windows.Controls.MenuItem();
            save.Header = "Save";
            save.Click += new RoutedEventHandler(MenuItem_Click);
            conMenu.Items.Add(save);

            System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
            delete.Header = "Delete";
            delete.Click += new RoutedEventHandler(MenuItem_Click);
            conMenu.Items.Add(delete);

            conMenu.PlacementTarget = uiElement;
            return conMenu;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns>System.Byte[].</returns>
        private byte[] GetContent(string fileUrl)
        {
            byte[] allbytes = null;
            if (File.Exists(fileUrl.ToString()))
            {
                FileStream fs = File.OpenRead(fileUrl.ToString());
                try
                {
                    allbytes = new byte[fs.Length];
                    fs.Read(allbytes, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                }
                finally
                {
                    fs.Close();
                }
            }
            return allbytes;
        }

        /// <summary>
        /// Gets the quote details.
        /// </summary>
        /// <param name="eventGetInteractionContent">Content of the event get interaction.</param>
        /// <returns>System.String.</returns>
        private string GetQuoteDetails(EventGetInteractionContent eventGetInteractionContent)
        {
            string quoteDetails = string.Empty;
            //quoteDetails="<div style=\"font-size:12pt;\">";
            try
            {
                List<string> QuoteDeatils = ConfigContainer.Instance().AllKeys.Contains("email.quote-details") ? ((string)ConfigContainer.Instance().GetValue("email.quote-details")).Split(',').ToList() : null;

                EmailInEntityAttributes emailInEntityAttributes = (EmailInEntityAttributes)eventGetInteractionContent.EntityAttributes;
                if (QuoteDeatils != null && QuoteDeatils.Count > 0)
                {
                    foreach (var item in QuoteDeatils)
                    {
                        if (item.ToLower() == "from")
                            quoteDetails += "<div><b>From : </b>" + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.FromAddress) + "</div>";
                        if (item.ToLower() == "to")
                            quoteDetails += "<div><b>To : </b>" + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.ToAddresses) + "</div>";
                        if (!string.IsNullOrEmpty(emailInEntityAttributes.CcAddresses))
                        {
                            if (item.ToLower() == "cc")
                                quoteDetails += "<div><b>Cc : </b>" + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.CcAddresses) + "</div>";
                        }
                        if (!string.IsNullOrEmpty(emailInEntityAttributes.BccAddresses))
                        {
                            if (item.ToLower() == "bcc")
                                quoteDetails += "<div><b>Bcc : </b>" + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.BccAddresses) + "</div>";
                        }
                        if (item.ToLower() == "sent")
                            quoteDetails += "<div><b>Sent : </b>" + startDate + "</div>";
                        if (item.ToLower() == "subject")
                            quoteDetails += "<div><b>Subject : </b>" + eventGetInteractionContent.InteractionAttributes.Subject + "</div>";
                    }
                }
                else
                {
                    quoteDetails += "<div><b>From : " + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.FromAddress) + "</div>";
                    quoteDetails += "<div><b>To : " + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.ToAddresses) + "</div>";
                    if (!string.IsNullOrEmpty(emailInEntityAttributes.CcAddresses))
                    {
                        quoteDetails += "<div><b>Cc : " + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.CcAddresses) + "</div>";
                    }
                    if (!string.IsNullOrEmpty(emailInEntityAttributes.BccAddresses))
                    {
                        quoteDetails += "<div><b>Bcc : " + Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.BccAddresses) + "</div>";
                    }
                    quoteDetails += "<div><b>Sent : </b>" + startDate + "</div>";
                    quoteDetails += "<div><b>Subject : </b>" + eventGetInteractionContent.InteractionAttributes.Subject + "</div>";
                }
                //quoteDetails += "</div>";
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at GetQuoteDetails " + ex.Message);
            }
            return quoteDetails;
        }

        /// <summary>
        /// Gets the quote header.
        /// </summary>
        /// <param name="contactName">Name of the contact.</param>
        /// <returns>System.String.</returns>
        private string GetQuoteHeader(string contactName)
        {
            string quoteHeader = string.Empty;
            string QuoteHeader = (ConfigContainer.Instance().AllKeys.Contains("email.quote-header") ? (string)ConfigContainer.Instance().GetValue("email.quote-header") : string.Empty);

            try
            {
                if (!string.IsNullOrEmpty(QuoteHeader))
                {
                    quoteHeader = QuoteHeader;
                    quoteHeader = quoteHeader.Replace("<date>", startDate);
                    quoteHeader = quoteHeader.Replace("<contact>", contactName);
                }
                else
                {
                    quoteHeader = "On " + startDate + ", " + contactName + " wrote :";
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at GetQuoteHeader " + ex.Message);
            }
            return quoteHeader;
        }

        /// <summary>
        /// Loads the attachments.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns>UIElement.</returns>/
        private UIElement LoadAttachments(string Name, Int64 size, string docId = null, bool isAdd = true)
        {
            Button btnAttachment = new Button();
            btnAttachment.Style = (Style)FindResource("AttachmentButton");
            btnAttachment.Click += new RoutedEventHandler(btnAttachment_Click);
            btnAttachment.Margin = new Thickness(3);
            btnAttachment.Height = 20;
            if (AddedAttachDocPath == null)
                AddedAttachDocPath = new List<string>();

            if (docId == null)
            {
                btnAttachment.Tag = Name + "|" + size; // Tag = Path + Size
                btnAttachment.Content = "  " + System.IO.Path.GetFileName(Name) + " (" + GetFileSize(size) + ")  ";
                if (!AddedAttachDocPath.Contains(Name))
                    AddedAttachDocPath.Add(Name);
            }
            else
            {
                btnAttachment.Tag = docId + "|" + size; // Tag = DocumentID + Size
                btnAttachment.Content = "  " + Name + " (" + GetFileSize(size) + ")  ";
                if (!AddedAttachDocPath.Contains(docId) && isAdd)
                    AddedAttachDocPath.Add(docId);
            }
            btnAttachment.ContextMenu = GetAttachmentMenus(btnAttachment);

            return btnAttachment;
        }

        /// <summary>
        /// Handles the Click event of the MenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as System.Windows.Controls.MenuItem;
            FileStream fs = null;
            IMessage response = null;
            try
            {
                switch (menuitem.Header.ToString().ToLower())
                {
                    case "open":
                        filePath = ((menuitem.Parent as ContextMenu).PlacementTarget as Button).Tag.ToString();
                        filePath = filePath.Split('|')[0];
                        if (!filePath.Contains("\\"))
                        {
                            response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetDocument(filePath);
                            if (response != null && response.Id == EventGetDocument.MessageId)
                            {
                                eventGetDocument = (EventGetDocument)response;
                                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp\" + filePath.ToString() + @"\";
                                logger.Info("Opening the file : " + eventGetDocument.TheName);
                                if (string.IsNullOrEmpty(Path.GetDirectoryName(eventGetDocument.TheName)))
                                    eventGetDocument.TheName = path + @"\" + eventGetDocument.TheName;
                                logger.Info("Creating the file : " + eventGetDocument.TheName);
                                if (!Directory.Exists(Path.GetDirectoryName(eventGetDocument.TheName)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(eventGetDocument.TheName));
                                using (fs = new FileStream(eventGetDocument.TheName, FileMode.Create)) { }
                                File.WriteAllBytes(eventGetDocument.TheName, eventGetDocument.Content);
                                Process.Start(eventGetDocument.TheName);
                            }
                        }
                        else
                        {
                            if (File.Exists(filePath))
                                Process.Start(filePath);
                        }
                        break;
                    case "save":
                        filePath = ((menuitem.Parent as ContextMenu).PlacementTarget as Button).Tag.ToString();
                        filePath = filePath.Split('|')[0];
                        if (!filePath.Contains("\\"))
                        {
                            response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetDocument(filePath);
                            if (response != null && response.Id == EventGetDocument.MessageId)
                            {
                                eventGetDocument = (EventGetDocument)response;
                                SaveFileDialog saveDialog = new SaveFileDialog();
                                saveDialog.FileName = eventGetDocument.TheName;
                                string _fileExtension = Path.GetExtension(eventGetDocument.TheName);
                                saveDialog.FileOk += new System.ComponentModel.CancelEventHandler(saveDialog_FileOk);
                                saveDialog.DefaultExt = "." + _fileExtension;
                                saveDialog.Filter = "All files (*.*) | *.*";
                                saveDialog.AddExtension = true;
                                saveDialog.ShowDialog();
                            }
                        }
                        else if (File.Exists(filePath))
                        {
                            eventGetDocument = null;
                            var fileinfo = new FileInfo(filePath);
                            SaveFileDialog saveDialog = new SaveFileDialog();
                            saveDialog.FileName = fileinfo.Name;
                            saveDialog.FileOk += new System.ComponentModel.CancelEventHandler(saveDialog_FileOk);
                            saveDialog.DefaultExt = "." + fileinfo.Extension;
                            saveDialog.Filter = "All files (*.*) | *.*";
                            saveDialog.AddExtension = true;
                            saveDialog.ShowDialog();
                        }
                        break;
                    case "delete":
                        var uiElement = (menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget;
                        var filepath = (uiElement as Button).Tag.ToString();
                        logger.Info("Deleting the file : " + filepath);
                        if (!filepath.Contains("\\"))
                        {
                            if (DeletedAttachDocId == null)
                                DeletedAttachDocId = new List<string>();
                            DeletedAttachDocId.Add(filepath.Split('|')[0]);
                        }
                        else
                            AddedAttachDocPath.Remove(filepath.Split('|')[0]);
                        var uiItem = (menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as Button;
                        if (dockOutboundAttachments.Children.Contains(uiItem))
                        {
                            dockOutboundAttachments.Children.Remove(uiItem);
                            if (this.parent is EmailMainWindow)
                                (this.parent as EmailMainWindow).totalfilelength -= Convert.ToInt64(filepath.Split('|')[1]);
                            CheckAttachmentSize(false);
                        }
                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while " + menuitem.Header.ToString().ToLower() + " the attachment '" + menuitem.Header.ToString().Trim() + "'  as :" + generalException.ToString());
            }
            finally
            {
                fs = null;
                //GC.Collect();
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuAddBcc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void mnuAddBcc_Click(object sender, RoutedEventArgs e)
        {
            gridBcc.Visibility = Visibility.Visible;
            if (gridCc.Visibility == Visibility.Visible)
            {
                btnAddCcandBcc.Visibility = Visibility.Collapsed;
                btnMenuCCBcc.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnAddCcandBcc.Content = "Add Cc";
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuAddCc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void mnuAddCc_Click(object sender, RoutedEventArgs e)
        {
            gridCc.Visibility = Visibility.Visible;
            if (gridBcc.Visibility == Visibility.Collapsed)
                btnAddCcandBcc.Content = "Add Bcc";
            else
            {
                btnAddCcandBcc.Visibility = Visibility.Collapsed;
                btnMenuCCBcc.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Notifies the selected email.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool NotifySelectedEmail(string to, string cc, string bcc)
        {
            try
            {
                //if (!string.IsNullOrEmpty(to))
                // txtOutboundTo.Text += (string.IsNullOrEmpty(txtOutboundTo.Text.Trim()) ? "" : ";") + to;
                txtOutboundTo.Text = to;
                //if (!string.IsNullOrEmpty(cc))
                //txtOutboundCc.Text += (string.IsNullOrEmpty(txtOutboundCc.Text.Trim()) ? "" : ";") + cc;
                txtOutboundCc.Text = cc;
                //if (!string.IsNullOrEmpty(bcc))
                //txtOutboundBcc.Text += (string.IsNullOrEmpty(txtOutboundBcc.Text.Trim()) ? "" : ";") + bcc;
                txtOutboundBcc.Text = bcc;

                //The code used to show cc and BCC if the email address available. Otherwise it will hide. -- By sakthi (25-06-2015).
                if (!string.IsNullOrEmpty(txtOutboundCc.Text))
                {
                    mnuAddCc_Click(null, null);
                }
                else
                {
                    btnCcDelete_Click(null, null);
                }
                if (!string.IsNullOrEmpty(txtOutboundBcc.Text))
                {
                    mnuAddBcc_Click(null, null);
                }
                else
                {
                    btnBccDelete_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at NotifySelectedEmail: " + ex.Message);
            }
            return true;
        }

        private bool ReplyFormat(bool mailFormat)
        {
            string replyFormat = "auto";

            if (ConfigContainer.Instance().AllKeys.Contains("email.reply-format") &&
                !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("email.reply-format").Trim()))
                replyFormat = ConfigContainer.Instance().GetAsString("email.reply-format").Trim();

            switch (replyFormat.ToLower())
            {
                case "html": return true;
                case "plain-text": return false;
                default: return mailFormat;
            }
        }

        /// <summary>
        /// Handles the FileOk event of the saveFileDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void saveDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string fileName = (sender as SaveFileDialog).FileName;
                if (eventGetDocument != null)
                {
                    byte[] content = eventGetDocument.Content;
                    File.WriteAllBytes(fileName, content);
                }
                else
                {
                    File.Copy(filePath, fileName);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while save File Dialog as : " + ex.Message);
            }
        }

        /// <summary>
        /// Sets from address.
        /// </summary>
        /// <param name="selectedAddress">The selected address.</param>
        private void SetFromAddress(string selectedAddress = "")
        {
            if (EmailDataContext.GetInstance().isFromAddressPopulated)
            {
                stkFromAddressError.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(selectedAddress))
                {
                    selectedAddress = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(selectedAddress);
                    if (selectedAddress.Contains(";"))
                        selectedAddress = selectedAddress.Split(';')[0];
                }
                if (EmailDataContext.GetInstance().MailBox != null && EmailDataContext.GetInstance().MailBox.Count > 0)
                {
                    if (EmailDataContext.GetInstance().MailBox.Count == 1)
                    {
                        txtOutboundFrom.Text = EmailDataContext.GetInstance().MailBox[0].Tag.ToString();
                        cmbFromAddress.Visibility = Visibility.Collapsed;
                        txtOutboundFrom.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        int? defaultIndex = null;
                        for (int itemIndex = 0; itemIndex < EmailDataContext.GetInstance().MailBox.Count; itemIndex++)
                        {
                            ComboBoxItem cbItem = new ComboBoxItem();
                            cbItem.Tag = EmailDataContext.GetInstance().MailBox[itemIndex].Tag;
                            cbItem.Content = EmailDataContext.GetInstance().MailBox[itemIndex].Content;
                            cbItem.ToolTip = EmailDataContext.GetInstance().MailBox[itemIndex].ToolTip;
                            cbItem.IsSelected = EmailDataContext.GetInstance().MailBox[itemIndex].IsSelected;
                            if (cbItem.IsSelected)
                                defaultIndex = itemIndex;
                            cmbFromAddress.Items.Add(cbItem);
                        }

                        cmbFromAddress.SelectedItem = (cmbFromAddress.Items.Cast<ComboBoxItem>().Where(x =>
                            x.Tag.ToString().Contains(selectedAddress) && x.Tag.ToString() == selectedAddress
                            ).FirstOrDefault()) ?? (defaultIndex != null ? cmbFromAddress.Items[Convert.ToInt32(defaultIndex)] : cmbFromAddress.Items[0]);

                        txtOutboundFrom.Text = ((ComboBoxItem)cmbFromAddress.SelectedItem).Tag.ToString();
                        //cmbFromAddress.ItemsSource = _fromAddresses;
                        cmbFromAddress.Visibility = Visibility.Visible;
                        txtOutboundFrom.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (this.parent is EmailMainWindow)
                        (this.parent as EmailMainWindow).ShowError("Action Aborted: From Address is missing");
                }
            }
            else
            {
                stkFromAddressError.Visibility = Visibility.Visible;
                txtOutboundFrom.Visibility = Visibility.Collapsed;
                cmbFromAddress.Visibility = Visibility.Collapsed;
                defaultFromAddress = selectedAddress;
                Pointel.Interactions.Email.Helper.EmailServerDetails.OnFromAddressChanged += EmailServerDetails__OnFromAddressChanged;
            }
        }

        /// <summary>
        /// Shows the contact directory.
        /// </summary>
        private void ShowContactDirectory()
        {
            try
            {
                contactDirectory = new ContactDirectoryWindow(txtOutboundTo.Text, txtOutboundCc.Text, txtOutboundBcc.Text)
                {
                    NotifySelectedEmailId = NotifySelectedEmail,
                    Owner = parent
                };
                contactDirectory.ShowDialog();

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at ShowContactDirectory " + ex.Message);
            }
            finally
            {
                contactDirectory = null;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _fileExtension = string.Empty;
            eventGetDocument = null;
            htmlEditor = null;
            contactDirectory = null;
            parent = null;
            startDate = null;
        }

        #endregion Methods
    }
}