/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
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
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Interactions.Email.Listener;
using Genesyslab.Platform.Commons.Protocols;
using Pointel.Interactions.IPlugins;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using System.IO;
using System.Diagnostics;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using System.ComponentModel;
using Pointel.Windows.Views.Common.Editor.Controls;
using Pointel.Interactions.Email.DataContext;
using Genesyslab.Platform.Commons.Collections;
using Microsoft.Win32;
using System.Globalization;

namespace Pointel.Interactions.Email.UserControls
{
    /// <summary>
    /// Interaction logic for InboundUserControl.xaml
    /// </summary>
    public partial class InboundUserControl : UserControl
    {
        #region Variable Declaration
        //Logger
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        //Attachments
        private string _fileExtension = string.Empty;
        private EventGetDocument eventGetDocument = null;

        //User controls
        private HTMLEditor htmlEditor = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundUserControl"/> class.
        /// </summary>
        public InboundUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundUserControl"/> class.
        /// </summary>
        /// <param name="_eventInvite">The _event invite.</param>
        /// <param name="eventGetInteractionContent">Content of the event get interaction.</param>
        public InboundUserControl(EventInvite _eventInvite, EventGetInteractionContent eventGetInteractionContent)
        {
            try
            {
                InitializeComponent();
                dockCc.Visibility = Visibility.Collapsed;
                dockBcc.Visibility = Visibility.Collapsed;
                if (_eventInvite.Interaction.InteractionUserData.Contains("Subject")
                    && !string.IsNullOrEmpty(_eventInvite.Interaction.InteractionUserData["Subject"].ToString()))
                        txtInboundSubject.ToolTip = txtInboundSubject.Text = _eventInvite.Interaction.InteractionUserData["Subject"].ToString();
                else
                    txtInboundSubject.ToolTip = txtInboundSubject.Text = "(No Subject)";   
                if (_eventInvite.Interaction.InteractionUserData.Contains("To"))
                    txtInboundTo.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(_eventInvite.Interaction.InteractionUserData["To"].ToString());
                if (_eventInvite.Interaction.InteractionUserData.Contains("FromAddress"))
                    txtInboundFrom.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(_eventInvite.Interaction.InteractionUserData["FromAddress"].ToString());
                BindContent(eventGetInteractionContent);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at InboundUserControl constructor" + ex.ToString());
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundUserControl"/> class.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="eventGetInteractionContent">Content of the event get interaction.</param>
        public InboundUserControl(KeyValueCollection userData, EventGetInteractionContent eventGetInteractionContent)
        {
            try
            {
                InitializeComponent();
                if (userData.Contains("Subject") && !string.IsNullOrEmpty(userData["Subject"].ToString()))
                    txtInboundSubject.Text = userData["Subject"].ToString();
                else
                    txtInboundSubject.ToolTip = txtInboundSubject.Text = "(No Subject)";   
                if (userData.Contains("To"))
                    txtInboundTo.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(userData["To"].ToString());
                if (userData.Contains("FromAddress"))
                    txtInboundFrom.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(userData["FromAddress"].ToString());
                BindContent(eventGetInteractionContent);

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at InboundUserControl constructor" + ex.ToString());
            }
        }

        /// <summary>
        /// Binds the content.
        /// </summary>
        /// <param name="eventGetInteractionContent">Content of the event get interaction.</param>
        public void BindContent(EventGetInteractionContent eventGetInteractionContent)
        {
            try
            {
                if (eventGetInteractionContent != null)
                {
                    if (eventGetInteractionContent.InteractionAttributes.StartDate != null && eventGetInteractionContent.InteractionAttributes.StartDate.ToString() != string.Empty)
                    {
                        lblInboundDateTime.Text = eventGetInteractionContent.InteractionAttributes.StartDate.ToString();
                        lblInboundDateTime.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetLocalDateTime(eventGetInteractionContent.InteractionAttributes.StartDate.ToString());
                    }
                    if (eventGetInteractionContent.EntityAttributes != null)
                    {
                        EmailInEntityAttributes emailInEntityAttributes = (EmailInEntityAttributes)eventGetInteractionContent.EntityAttributes;
                        if (!string.IsNullOrEmpty(emailInEntityAttributes.CcAddresses))
                        {
                            txtInboundCc.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.CcAddresses);
                            dockCc.Visibility = Visibility.Visible;
                        }
                        else
                            dockCc.Visibility = Visibility.Collapsed;

                        if (!string.IsNullOrEmpty(emailInEntityAttributes.BccAddresses))
                        {
                            txtInboundBcc.Text = Pointel.Interactions.Email.Helper.EmailAddressValidation.GetEmailAddress(emailInEntityAttributes.BccAddresses);
                            dockBcc.Visibility = Visibility.Visible;
                        }
                        else
                            dockBcc.Visibility = Visibility.Collapsed;

                    }
                    if (eventGetInteractionContent.Attachments != null)
                    {
                        foreach (Attachment attachementDetails in eventGetInteractionContent.Attachments)
                        {
                            wrapInboundAttachments.Children.Add(LoadAttachments(attachementDetails.TheName, Convert.ToInt64(attachementDetails.TheSize.Value.ToString()), attachementDetails.DocumentId));
                            attacScroll.ScrollToTop();
                        }
                    }

                    if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                    {
                        htmlEditor = new HTMLEditor(false, true, eventGetInteractionContent.InteractionContent.StructuredText);
                        //htmlEditor.Width = dockInboundContent.ActualWidth;
                        //htmlEditor.Height = dockInboundContent.ActualHeight;
                        dockInboundContent.Children.Clear();
                        dockInboundContent.Children.Add(htmlEditor);
                    }
                    else if (!string.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.Text))
                    {
                        string content = eventGetInteractionContent.InteractionContent.Text.Replace("\r\n", "<br />");
                        content = eventGetInteractionContent.InteractionContent.Text.Replace("\n", "<br />");
                        htmlEditor = new HTMLEditor(false, false, content);
                        //htmlEditor.Width = dockInboundContent.ActualWidth;
                        //htmlEditor.Height = dockInboundContent.ActualHeight;
                        dockInboundContent.Children.Clear();
                        dockInboundContent.Children.Add(htmlEditor);
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at BindContent " + ex.ToString());
            }
        }

        #endregion

        #region WindowsEvents

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _fileExtension = string.Empty;
            eventGetDocument = null;
            htmlEditor = null;
        }
        #endregion

        #region Subject Selection Style
        /// <summary>
        /// Handles the LostFocus event of the txtInboundSubject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtInboundSubject_LostFocus(object sender, RoutedEventArgs e)
        {
            txtInboundSubject.Select(0, 0);
        }

        /// <summary>
        /// Handles the GotFocus event of the txtInboundSubject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtInboundSubject_GotFocus(object sender, RoutedEventArgs e)
        {
            txtInboundSubject.SelectAll();
        }
        #endregion

        #region Attachments

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

            conMenu.PlacementTarget = uiElement;

            return conMenu;
        }

        /// <summary>
        /// Loads the attachments.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns>UIElement.</returns>
        private UIElement LoadAttachments(string Name, Int64 size, string docId)
        {
            System.Windows.Controls.Button btnAttachment = new System.Windows.Controls.Button();
            btnAttachment.Style = (Style)FindResource("AttachmentButton");
            btnAttachment.Click += new RoutedEventHandler(btnAttachment_Click);
            btnAttachment.Margin = new Thickness(3);
            btnAttachment.Height = 20;

            btnAttachment.Content = "  " + Name + " (" + GetFileSize(size) + ")  ";
            btnAttachment.Tag = docId;
            btnAttachment.ContextMenu = GetAttachmentMenus(btnAttachment);
            btnAttachment.HorizontalAlignment = HorizontalAlignment.Left;

            return btnAttachment;
        }

        /// <summary>
        /// Handles the Click event of the btnAttachment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;
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
            IMessage response;
            try
            {
                switch (menuitem.Header.ToString().ToLower())
                {
                    case "open":
                        var docId = ((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString();
                        response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetDocument(docId);
                        if (response != null && response.Id == EventGetDocument.MessageId)
                        {
                            eventGetDocument = (EventGetDocument)response;
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp\" + docId.ToString() + @"\"; logger.Info("Opening the file : " + eventGetDocument.TheName);
                            if (string.IsNullOrEmpty(Path.GetDirectoryName(eventGetDocument.TheName)))
                                eventGetDocument.TheName = path + @"\" + eventGetDocument.TheName;
                            logger.Info("Creating the file : " + eventGetDocument.TheName);
                            if (!Directory.Exists(Path.GetDirectoryName(eventGetDocument.TheName)))
                                Directory.CreateDirectory(Path.GetDirectoryName(eventGetDocument.TheName));
                            using (fs = new FileStream(eventGetDocument.TheName, FileMode.Create)) { }
                            File.WriteAllBytes(eventGetDocument.TheName, eventGetDocument.Content);
                            Process.Start(eventGetDocument.TheName);
                        }
                        break;
                    case "save":
                        response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetDocument(((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString());
                        if (response != null && response.Id == EventGetDocument.MessageId)
                        {
                            eventGetDocument = (EventGetDocument)response;
                            SaveFileDialog saveDialog = new SaveFileDialog();
                            saveDialog.FileName = eventGetDocument.TheName;
                            string _fileExtension = System.IO.Path.GetExtension(eventGetDocument.TheName);
                            saveDialog.FileOk += new CancelEventHandler(saveFileDialog_FileOk);
                            saveDialog.DefaultExt = "." + _fileExtension;
                            saveDialog.Filter = "All files (*.*) | *.*";
                            saveDialog.AddExtension = true;
                            saveDialog.ShowDialog();
                        }
                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while " + menuitem.Header.ToString().ToLower() + " the attachment '" + menuitem.Header.ToString().Trim() + "'  as :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Exited event of the process control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void process_Exited(object sender, EventArgs e)
        {
            try
            {
                Process p = sender as Process;
                File.Delete(p.StartInfo.FileName);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
        }


        /// <summary>
        /// Handles the FileOk event of the saveFileDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileName = (sender as SaveFileDialog).FileName;
                if (eventGetDocument != null)
                {
                    File.WriteAllBytes((string.IsNullOrEmpty(fileName) ? (eventGetDocument.TheName + (eventGetDocument.TheName.EndsWith(_fileExtension) ? "" : _fileExtension)) : fileName), eventGetDocument.Content);
                    eventGetDocument = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while save File Dialog as : " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="Bytes">The bytes.</param>
        /// <returns>System.String.</returns>
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
        /// Handles the SizeChanged event of the attacScroll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void attacScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            attacScroll.SizeChanged -= attacScroll_SizeChanged;
            if (attacScroll.ActualWidth > 20)
                wrapInboundAttachments.Width = attacScroll.ActualWidth - 20;
            attacScroll.SizeChanged += new SizeChangedEventHandler(attacScroll_SizeChanged);
        }
        #endregion

        #region Print Inbound Mail

        /// <summary>
        /// Prints the inbound mail.
        /// </summary>
        public void PrintInboundMail()
        {
            try
            {
                Paragraph normalMessageParagraph = new Paragraph();
                if (!string.IsNullOrEmpty(txtInboundSubject.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Subject :")));
                    normalMessageParagraph.Inlines.Add(txtInboundSubject.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                normalMessageParagraph.Inlines.Add(new Bold(new Run("From :")));
                normalMessageParagraph.Inlines.Add(txtInboundFrom.Text);
                normalMessageParagraph.Inlines.Add(new LineBreak());
                if (!string.IsNullOrEmpty(lblInboundDateTime.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Date :")));
                    normalMessageParagraph.Inlines.Add(lblInboundDateTime.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                normalMessageParagraph.Inlines.Add(new Bold(new Run("To :")));
                normalMessageParagraph.Inlines.Add(txtInboundTo.Text);
                normalMessageParagraph.Inlines.Add(new LineBreak());
                if (!string.IsNullOrEmpty(txtInboundCc.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Cc :")));
                    normalMessageParagraph.Inlines.Add(txtInboundCc.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                if (!string.IsNullOrEmpty(txtInboundBcc.Text))
                {
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("Bcc :")));
                    normalMessageParagraph.Inlines.Add(txtInboundBcc.Text);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                }
                htmlEditor.Print(normalMessageParagraph);
            }
            catch (Exception ex)
            {
                logger.Error("Error while priniting E-Mail :" + ex.Message);
            }
        }

        #endregion

        #region SelectallText

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="tb">The tb.</param>
        private void SelectAll(TextBox tb)
        {
            Keyboard.Focus(tb);
            tb.SelectAll();
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void txt_GotMouseCapture(object sender, MouseEventArgs e)
        {
            SelectAll(sender as TextBox);
        }

        #endregion
    }
}
