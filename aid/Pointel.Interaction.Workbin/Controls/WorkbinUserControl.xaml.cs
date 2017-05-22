#region Header

/*
* =======================================================
* Pointel.Interaction.Workbin.Controls.WorkbinUserControl
* =======================================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================================
*/

#endregion Header

namespace Pointel.Interaction.Workbin.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Interaction.Workbin.Forms;
    using Pointel.Interaction.Workbin.Helpers;
    using Pointel.Interaction.Workbin.Utility;
    using Pointel.Interactions.Core;
    using Pointel.Interactions.IPlugins;
    using System.Data;
    using System.Globalization;
    using Pointel.Windows.Views.Common.Editor.Controls;

    /// <summary>
    /// Interaction logic for WorkbinUserControl.xaml
    /// </summary>
    public partial class WorkbinUserControl : UserControl
    {
        #region Fields

        string addedWorkbin = null;

        //private System.Data.DataTable dtWorkbinField = new System.Data.DataTable();
        private bool isExpandFlag;
        private bool isPersonalWorkbinSelected;
        private bool isMyTeamWorkbinSelected;
        private bool isShowDelete;
        private bool isShowMarkDone;
        private bool isShowOpen;
        private bool isShowReply;
        private bool isShowReplyAll;
        private bool _isActivelyHandling = true;
        private KeyValueCollection kvpInteractions = null;
        private Pointel.Logger.Core.ILog logger = null;
        private List<string> lstEmailStatusProgress = new List<string>();
        private WorkbinData workbinData = null;
        private bool workbinLoaded;
        private DispatcherTimer _timerforcloseError = null;
        private int totalIQInteractions = 0;
        int startIndex = 0;
        int endIndex = 0;
        private int iqSnapshotID = 0;
        private string iqDisplayedColoumns;
        private DataTable dtIQInteractions = null;
        private Dictionary<string, KeyValueCollection> dicKVPIQInteractions = null;
        private bool isAddedMediaIcon = false;
        private bool _isInteractionQueue;
        private bool _isInteractionActive;
        private IQMenuItem _selectedIQCondition;

        #endregion Fields

        #region Constructors

        public WorkbinUserControl()
        {
            logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            kvpInteractions = new KeyValueCollection();
            workbinData = new WorkbinData();
            isPersonalWorkbinSelected = true;
            InitializeComponent();
            DataContext = workbinData;
        }

        #endregion Constructors

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

        public static string GetFileSize(long Bytes)
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

        private void attacScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            attacScroll.SizeChanged -= attacScroll_SizeChanged;
            if (attacScroll.ActualWidth > 20)
                dpAttachments.Width = attacScroll.ActualWidth - 20;
            attacScroll.SizeChanged += new SizeChangedEventHandler(attacScroll_SizeChanged);
        }

        private void BindSelectedMailDetails(WorkbinMailDetails selectedMailDetails)
        {
            if (selectedMailDetails != null)
            {
                if (WorkbinUtility.Instance().IsContactServerActive)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionContent(selectedMailDetails.InteractionId, true);
                    if (result != null && result.Id == EventGetInteractionContent.MessageId)
                    {
                        EventGetInteractionContent interactionContent = result as EventGetInteractionContent;
                        //ParseAndStoreMail(interactionContent, ref selectedMailDetails);
                        ParseAndStoreMail(interactionContent, ref selectedMailDetails);

                        txtNotes.Text = selectedMailDetails.EmailNotes;
                    }
                    //if (!string.IsNullOrEmpty(selectedMailDetails.MailBody))
                    //    dcMailMessageContainer.MinHeight = 150;
                    //else
                    //    dcMailMessageContainer.MinHeight = 0;

                    tbCallBack.DataContext = selectedMailDetails;
                    txtNoDetails.Visibility = Visibility.Collapsed;

                    gridDetails.Visibility = dockCaseData.Visibility = Visibility.Visible;
                    if (string.IsNullOrEmpty(selectedMailDetails.Subject))
                        txtSubjectBold.Text = "(No Subject)";
                    txtDot.Visibility = Utility.ControlUtility.IsContentExceed(txtSubjectBold) ? Visibility.Visible : Visibility.Collapsed;

                    gcTo.Height = string.IsNullOrEmpty(txtTo.Text) ? new GridLength(0) : GridLength.Auto;
                    dockCC.Height = string.IsNullOrEmpty(txtCcText.Text) ? new GridLength(0) : GridLength.Auto;
                    gcFrom.Height = string.IsNullOrEmpty(txtFrom.Text) ? new GridLength(0) : GridLength.Auto;
                    dockBCC.Height = string.IsNullOrEmpty(txtBCcText.Text) ? new GridLength(0) : GridLength.Auto;
                    txtNocontactServer.Visibility = Visibility.Collapsed;

                    Pointel.Windows.Views.Common.Editor.Controls.HTMLEditor htmlEditor = new Windows.Views.Common.Editor.Controls.HTMLEditor(false, true, selectedMailDetails.MailBody);
                    dcMailMessageContainer.Children.Clear();
                    dcMailMessageContainer.Children.Add(htmlEditor);
                    dpAttachments.Children.Clear();
                    foreach (AttachmentDetails attachementDetails in selectedMailDetails.Attachment)
                    {
                        dpAttachments.Children.Add(LoadAttachments(attachementDetails.FileName, attachementDetails.Size, attachementDetails.DocumentId));
                    }
                    if (ExpandMailHeading.Text == "Hide Details")
                    {
                        if (!string.IsNullOrEmpty(selectedMailDetails.MailBody))
                            rowdefDetails.MinHeight = 250;
                        else
                            rowdefDetails.MinHeight = gridDetails.ActualHeight;
                    }

                }
                else
                {
                    if (ExpandMailHeading.Text == "Hide Details")
                        rowdefDetails.MinHeight = rowdefDetails.ActualHeight;
                    txtNocontactServer.Visibility = Visibility.Visible;
                    gridDetails.Visibility = Visibility.Collapsed;
                }

                // Added by sakthi to check the selected item is interaction queue
                // and need to show reply all button.

                if (_isInteractionQueue && isShowReply && dgIQ.SelectedItems != null && dgIQ.SelectedItems.Count == 0)
                    isShowReplyAll = (!string.IsNullOrEmpty(selectedMailDetails.CC) || !string.IsNullOrEmpty(selectedMailDetails.BCC));

                ViewMailButton();

                if (selectedMailDetails.CaseData == null || selectedMailDetails.CaseData.Count == 0)
                {
                    dgUserData.Visibility = Visibility.Collapsed;
                    txtNoCaseData.Visibility = Visibility.Visible;
                }
                else
                {
                    dgUserData.Visibility = Visibility.Visible;
                    txtNoCaseData.Visibility = Visibility.Collapsed;
                }
                dgUserData.Width = bdrUSerDataContainer.Width;
            }
            else
            {
                txtNoDetails.Visibility = Visibility.Visible;
                gridDetails.Visibility = dockCaseData.Visibility = Visibility.Collapsed;
            }
        }

        void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            CloseError(null, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseErrorMessage();
                List<string> lstInteraction = new List<string>();

                if (_isInteractionQueue)
                {
                    if (dgIQ.SelectedItems != null)
                    {
                        foreach (var selectedItem in dgIQ.SelectedItems)
                        {
                            DataRowView selectedRow = selectedItem as DataRowView;
                            if (selectedRow != null)
                                lstInteraction.Add(selectedRow["InteractionId"].ToString());
                        }
                    }
                }
                else if (dgWorkbin.SelectedItems != null)
                {
                    foreach (var item in dgWorkbin.SelectedItems)
                    {
                        WorkbinMailDetails mailDetails = item as WorkbinMailDetails;
                        if (mailDetails != null)
                            lstInteraction.Add(mailDetails.InteractionId);
                    }
                }

                if (lstInteraction.Count > 0)
                {
                    PopUpWindow popup = new PopUpWindow();
                    popup.Message = "Are you sure that you want to delete " + (lstInteraction.Count > 1 ? "these" : "this") + " E-mail?";
                    popup.btnOK.Content = "Yes";
                    popup.btnCancel.Visibility = System.Windows.Visibility.Visible;
                    if (popup.ShowDialog() == true)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            int failureCount = 0;
                            int successCount = 0;
                            foreach (var interactionId in lstInteraction)
                            {
                                try
                                {

                                    InteractionService interactionService = new InteractionService();
                                    KeyValueCollection keyValue = new KeyValueCollection();
                                    keyValue.Add("id", interactionId);
                                    Pointel.Interactions.Core.Common.OutputValues result = interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, keyValue);
                                    if (result.MessageCode.Equals("200"))
                                    {
                                        result = interactionService.StopProcessingInteraction(WorkbinUtility.Instance().IxnProxyID, interactionId);
                                        if (result.MessageCode.Equals("200"))
                                        {
                                            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                            {
                                                IMessage message = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance()
                                                    .PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).DeleteInteraction(interactionId);
                                                successCount++;
                                                // If it is interaction Queue , Need to perform the notification to interaction queue.
                                                // Start

                                                if (_isInteractionQueue)
                                                    IQRemoveNotification();

                                                // End

                                                NotifyHistoryRefresh(interactionId, true);
                                            }
                                        }
                                        else
                                            failureCount++;

                                    }
                                    else
                                        failureCount++;

                                }
                                catch (Exception ex)
                                {
                                    logger.Error("Error occurred while delete interaction as " + ex.Message);
                                    failureCount++;
                                }
                            }

                            if (failureCount > 0)
                            {
                                if (lstInteraction.Count == 1)
                                    starttimerforerror("The delete operation got failed.");
                                else if (failureCount == 1)
                                    starttimerforerror("The delete operation got failed for one interaction.");
                                else
                                    starttimerforerror("The delete operation got failed for " + failureCount + " interactions.");
                            }
                            else if (successCount > 0)
                                starttimerforerror("The interaction removed successfully.");


                        }));
                    }
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseErrorMessage();
                if (isExpandFlag == false)
                {
                    isExpandFlag = true;
                    imgExpand.Source = null;
                    imgExpand.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\showDetails.png", UriKind.Relative));
                    ExpandMailHeading.Text = "Show Details";
                    ExpandMailContent.Text = "Show details for selected mail.";
                    TitleExpand.Text = "Show Details";
                    rowdefDetails.MinHeight = 0;
                    dgWorkbin.Height = dgIQ.Height = double.NaN;
                    rowdefDetails.Height = new GridLength(0);
                    rowdefHistory.Height = new GridLength(MiddleGrid.ActualHeight, GridUnitType.Star);
                    if (dgIQ.Visibility == Visibility.Visible && brdIQ.ActualWidth > 0)
                        dgIQ.Width = double.NaN;
                }
                else
                {
                    imgExpand.Source = null;
                    //imgExpand.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Expand-01.png", UriKind.Relative));
                    imgExpand.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\HideDetails.png", UriKind.Relative));
                    //TitleExpand.Text = "Expand";
                    ExpandMailHeading.Text = "Hide Details";
                    ExpandMailContent.Text = "Hide details for selected mail.";
                    TitleExpand.Text = "Hide Details";
                    rowdefDetails.Height = new GridLength(300, GridUnitType.Star);

                    if (gridDetails.Visibility == Visibility.Visible)
                        rowdefDetails.MinHeight = 250;
                    else
                        rowdefDetails.MinHeight = gridOtherMediaDetails.ActualHeight;


                    if (_isInteractionQueue)
                    {
                        dgIQ.Height = 150;
                        rowdefHistory.Height = new GridLength(165 + brdPaging.ActualHeight);
                        if (dgIQ.Visibility == Visibility.Visible && brdIQ.ActualWidth > 0)
                            dgIQ.Width = double.NaN;
                    }
                    else
                    {
                        dgWorkbin.Height = 150;
                        rowdefHistory.Height = new GridLength(160);
                    }

                    isExpandFlag = false;
                }
            }
            catch (Exception exception)
            {
                logger.Error("btnExpand_Click" + exception.ToString());
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CloseErrorMessage();
                    ShowPrintDocument();
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred as :" + generalException.ToString());
                }
            }));
        }

        private void ShowPrintDocument()
        {
            try
            {

                WorkbinMailDetails mailDetails = tbCallBack.DataContext as WorkbinMailDetails;
                if (mailDetails != null)
                {
                    Paragraph normalMessageParagraph = new Paragraph();
                    if (!string.IsNullOrEmpty(mailDetails.Subject))
                    {
                        normalMessageParagraph.Inlines.Add(new Bold(new Run("Subject :")));
                        normalMessageParagraph.Inlines.Add(mailDetails.Subject);
                        normalMessageParagraph.Inlines.Add(new LineBreak());
                    }
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("From :")));
                    normalMessageParagraph.Inlines.Add(mailDetails.From);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                    if (!string.IsNullOrEmpty(mailDetails.StartDate))
                    {
                        normalMessageParagraph.Inlines.Add(new Bold(new Run("Date :")));
                        normalMessageParagraph.Inlines.Add(mailDetails.StartDate);
                        normalMessageParagraph.Inlines.Add(new LineBreak());
                    }
                    else if (!string.IsNullOrEmpty(mailDetails.ReceivedDate))
                    {
                        normalMessageParagraph.Inlines.Add(new Bold(new Run("Date :")));
                        normalMessageParagraph.Inlines.Add(mailDetails.ReceivedDate);
                        normalMessageParagraph.Inlines.Add(new LineBreak());
                    }
                    normalMessageParagraph.Inlines.Add(new Bold(new Run("To :")));
                    normalMessageParagraph.Inlines.Add(mailDetails.To);
                    normalMessageParagraph.Inlines.Add(new LineBreak());
                    if (!string.IsNullOrEmpty(mailDetails.CC))
                    {
                        normalMessageParagraph.Inlines.Add(new Bold(new Run("Cc :")));
                        normalMessageParagraph.Inlines.Add(mailDetails.CC);
                        normalMessageParagraph.Inlines.Add(new LineBreak());
                    }
                    if (!string.IsNullOrEmpty(mailDetails.BCC))
                    {
                        normalMessageParagraph.Inlines.Add(new Bold(new Run("Bcc :")));
                        normalMessageParagraph.Inlines.Add(mailDetails.BCC);
                        normalMessageParagraph.Inlines.Add(new LineBreak());
                    }
                    HTMLEditor htmlEditor = new HTMLEditor(false, true, mailDetails.MailBody);
                    htmlEditor.Print(normalMessageParagraph);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error while priniting E-Mail :" + ex.Message);
            }
        }

        private void btnMarkDone_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CloseErrorMessage();

                    if (_isInteractionQueue && dgIQ.SelectedItems != null && dgIQ.SelectedItems.Count > 1)
                        MarkDoneMultipleMailIQ();
                    else if (dgWorkbin.SelectedItems != null && dgWorkbin.SelectedItems.Count > 1)
                        MarkDoneAllSelectedMails();
                    else
                        MarkDoneSelectedMail();
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred as :" + generalException.ToString());
                }
            }));
        }

        /// <summary>
        /// Handles the Click event of the btnMyInteractionQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMyInteractionQueue_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
         {
             try
             {
                 //isPersonalWorkbinSelected = false;
                 ClearMyWorkbinAndTeamWorkbin();
                 _isInteractionQueue = true;
                 if (workbinData.ListIQMenuItem == null)
                 {
                     IQHelper objIQHelper = new IQHelper();
                     workbinData.ListIQMenuItem = objIQHelper.LoadIQ();
                     objIQHelper = null;

                     gridWorkbin.Visibility = Visibility.Collapsed;
                     HideIQ();

                     //Assign Paging size. it should be less than or equal to 100.
                     if (ConfigContainer.Instance().AllKeys.Contains("interaction-management.available-interaction-page-sizes")
                            && !string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("interaction-management.available-interaction-page-sizes"))))
                     {
                         List<string> lstPaging = ((string)ConfigContainer.Instance().GetValue("interaction-management.available-interaction-page-sizes")).Split(',').ToList();
                         int outvalues = 1000;
                         //cmbItemsPerPage.ItemsSource = lstPaging.Distinct().Where(x => int.TryParse(x, out outvalues) && outvalues <= 100).ToList();
                         cmbItemsPerPage.ItemsSource = lstPaging.Distinct().Where(x => int.TryParse(x, out outvalues) && outvalues <= 100).ToList().Distinct(); ;
                     }
                     else
                         cmbItemsPerPage.ItemsSource = new string[] { "5", "10", "25", "50" };

                     cmbItemsPerPage.SelectedItem = cmbItemsPerPage.Items[0];

                 }

                 if (workbinData.ListIQMenuItem != null && workbinData.ListIQMenuItem.Count > 0)
                 {
                     txtErrorinConfig.Visibility = Visibility.Collapsed;
                     tvWorkbin.Visibility = Visibility.Collapsed;
                     tvInteractionQueue.Visibility = Visibility.Visible;
                     gridWorkbin.Visibility = Visibility.Collapsed;

                     if (_selectedIQCondition != null)
                     {
                         for (int index = 0; index < workbinData.ListIQMenuItem.Count; index++)
                         {
                             if (workbinData.ListIQMenuItem[index].Items.Where(x => x.Condition == _selectedIQCondition.Condition).ToList().Count > 0)
                             {
                                 int itemIndex = workbinData.ListIQMenuItem[index].Items.IndexOf(
                                     workbinData.ListIQMenuItem[index].Items.Where(x => x.Condition == _selectedIQCondition.Condition).SingleOrDefault());
                                 workbinData.ListIQMenuItem[index].Items[itemIndex].IsSelected = true;
                                 workbinData.ListIQMenuItem[index].IsExpanded = true;
                                 goto BindigPlace;
                             }
                         }

                         for (int index = 0; index < workbinData.ListIQMenuItem.Count; index++)
                         {
                             if (workbinData.ListIQMenuItem[index].Items[0].Items.Where(x => x.Condition == _selectedIQCondition.Condition).ToList().Count > 0)
                             {
                                 int itemIndex = workbinData.ListIQMenuItem[index].Items[0].Items.IndexOf(workbinData.ListIQMenuItem[index].Items[0].Items.Where(x => x.Condition == _selectedIQCondition.Condition).SingleOrDefault());
                                 workbinData.ListIQMenuItem[index].Items[0].Items[itemIndex].IsSelected = true;
                                 workbinData.ListIQMenuItem[index].Items[0].IsExpanded = true;
                                 workbinData.ListIQMenuItem[index].IsExpanded = true;
                                 goto BindigPlace;
                             }
                         }

                     }

                 BindigPlace:
                     tvInteractionQueue.ItemsSource = null;
                     tvInteractionQueue.ItemsSource = workbinData.ListIQMenuItem;


                     //if (dtIQInteractions != null && dtIQInteractions.Rows.Count > 0)
                     //{
                     //    workbinData.IQVisiblity = Visibility.Visible;
                     //    ShowMainMenu();
                     //    dgIQ_SelectionChanged(null, null);

                     //}
                     //else
                     //    HideIQ();

                     WorkbinService.InteractionClosedEvent -= InteractionClosedEvent;
                     WorkbinService.InteractionClosedEvent += InteractionClosedEvent;
                     SetWidthandHeightforIQ();

                 }

                 else
                 {
                     txtErrorinConfig.Visibility = Visibility.Visible;
                     txtErrorinConfig.Text = "No interaction queue configured";
                     tvWorkbin.Visibility = Visibility.Collapsed;
                     tvInteractionQueue.Visibility = Visibility.Collapsed;
                     gridWorkbin.Visibility = Visibility.Collapsed;
                     HideIQ();
                     // It is used to prevent second time IQ load when it fail or not configured.
                     workbinData.ListIQMenuItem = new List<IQMenuItem>();
                 }
             }
             catch (Exception generalException)
             {
                 logger.Error("Error occurred as :" + generalException.ToString());
             }
         }));
        }

        private void btnMyTeamWorkbin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //btnMyTeamWorkbin.Background=Color
                tvWorkbin.Visibility = Visibility.Visible;
                txtErrorinConfig.Visibility = Visibility.Collapsed;
                tvInteractionQueue.Visibility = Visibility.Collapsed;
                workbinData.IQVisiblity = Visibility.Collapsed;

                if (!isMyTeamWorkbinSelected)
                {
                    if (workbinData.WorkbinMenu != null)
                        workbinData.WorkbinMenu.Clear();

                    ShowCurrentWorkbin.Header = "No Workbin Selected";
                    if (ConfigContainer.Instance().AllKeys.Contains("supervisor.enable.move-interactionqueue")
                        && "true".Equals(ConfigContainer.Instance().GetValue("supervisor.enable.move-interactionqueue").ToString()))
                    {
                        gcWorkbin.Width = new GridLength(0);
                    }
                    workbinData.SelectedWorkbinDetails = new List<WorkbinMailDetails>();
                    isPersonalWorkbinSelected = false;
                    isMyTeamWorkbinSelected = true;
                    LoadMyTeamlWorkbin();
                    BrushConverter brushConverter = new BrushConverter();
                    btnMyTeamWorkbin.Background = (Brush)brushConverter.ConvertFrom("#A7BFD6");
                    btnMyWorkbin.Background = (Brush)brushConverter.ConvertFrom("#D9EBFC");
                }
                else
                {
                    tvWorkbin_SelectedItemChanged(null, null);
                    dgWorkbin_SelectedCellsChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private void btnMyWorkbin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tvWorkbin.Visibility = Visibility.Visible;
                txtErrorinConfig.Visibility = Visibility.Collapsed;
                tvInteractionQueue.Visibility = Visibility.Collapsed;
                workbinData.IQVisiblity = Visibility.Collapsed;


                if (!isPersonalWorkbinSelected)
                {
                    if (workbinData.MyTeamWorkbinMenu != null)
                        workbinData.MyTeamWorkbinMenu.Clear();
                    gcWorkbin.Width = GridLength.Auto;
                    LoadPersonalWorkbin();
                    isPersonalWorkbinSelected = true;
                    isMyTeamWorkbinSelected = false;
                    SetPersonalWorkbinBackground();
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private void ClearMyWorkbinAndTeamWorkbin()
        {
            workbinData.SelectedWorkbinDetails = new List<WorkbinMailDetails>();
            tvWorkbin.ItemsSource = null;

            if (workbinData.WorkbinMenu != null)
                workbinData.WorkbinMenu.Clear();

            if (workbinData.MyTeamWorkbinMenu != null)
                workbinData.MyTeamWorkbinMenu.Clear();

            workbinData.SelectedWorkbinDetails = new List<WorkbinMailDetails>();

            isPersonalWorkbinSelected = isMyTeamWorkbinSelected = false;

        }

        private void btnOpenMail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseErrorMessage();

                if (_isInteractionQueue)
                {
                    DataRowView selectedRow = dgIQ.SelectedItem as DataRowView;
                    if (selectedRow != null)
                        OpenMailInWindow(selectedRow["InteractionId"].ToString());
                }
                else
                {
                    WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;
                    dgWorkbin.IsEnabled = false;
                    if (mailDetails != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //if (CheckWorkbinAvailability()) return;
                            OpenMailInWindow(mailDetails.InteractionId);
                            dgWorkbin.IsEnabled = true;
                        }));
                    }
                    else
                        dgWorkbin.IsEnabled = true;
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as :" + generalException.ToString());
            }
        }

        private void btnQueueMail_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnReplyAllMail_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CloseErrorMessage();
                    //if (CheckWorkbinAvailability()) return;
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                    {
                        if (IsEmailReachMaximumCount()) return;

                        string interactionID = string.Empty;

                        if (_isInteractionQueue && dgIQ.SelectedItem != null)
                        {
                            DataRowView selectedMailRow = dgIQ.SelectedItem as DataRowView;

                            if (selectedMailRow != null)
                                interactionID = selectedMailRow["InteractionId"].ToString();
                            else
                                logger.Warn("The selected mail object is null.");
                        }
                        else if (dgWorkbin.SelectedItem != null)
                        {
                            WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;

                            if (mailDetails != null)
                                interactionID = mailDetails.InteractionId;
                            else
                                logger.Warn("The selected mail object is null.");

                        }

                        if (!string.IsNullOrEmpty(interactionID))
                        {
                            if (IsChildInOpen(interactionID))
                            {
                                starttimerforerror("Reply All action failed.");

                                // TODO : Need to notify the reply failed message.

                                return;
                            }

                            ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email])
                                .NotifyEmailReply(interactionID, true);
                        }
                        else
                            logger.Warn("The selected interaction Id is null.");
                    }
                    else
                    {
                        // TODO : Need to hanlde the email unavailablity.
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred as :" + generalException.ToString());
                }
            }));
        }

        private void btnReplyMail_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CloseErrorMessage();
                    //if (CheckWorkbinAvailability()) return;
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                    {
                        if (IsEmailReachMaximumCount()) return;

                        string interactionID = string.Empty;

                        if (_isInteractionQueue && dgIQ.SelectedItem != null)
                        {
                            DataRowView selectedMailRow = dgIQ.SelectedItem as DataRowView;

                            if (selectedMailRow != null)
                                interactionID = selectedMailRow["InteractionId"].ToString();
                            else
                                logger.Warn("The selected mail object is null.");
                        }
                        else if (dgWorkbin.SelectedItem != null)
                        {
                            WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;

                            if (mailDetails != null)
                                interactionID = mailDetails.InteractionId;
                            else
                                logger.Warn("The selected mail object is null.");

                        }

                        if (!string.IsNullOrEmpty(interactionID))
                        {
                            if (IsChildInOpen(interactionID))
                            {
                                starttimerforerror("Reply action failed.");

                                // TODO : Need to notify the reply failed message.

                                return;
                            }

                            ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email])
                                .NotifyEmailReply(interactionID);
                        }
                        else
                            logger.Warn("The selected interaction Id is null.");
                    }
                    else
                    {
                        // TODO : Need to hanlde the email unavailablity.
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred as :" + generalException.ToString());
                }
            }));
        }

        private void btnWorkbinMail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseErrorMessage();
                System.Windows.Controls.UserControl teamCommunicatorWorkbin = LoadTeamCommunicatorWorkbin();
                ShowTeamCommunicator(teamCommunicatorWorkbin, btnWorkbinMail);
            }
            catch (Exception ex)
            {
                logger.Error("btnTransfer_Click:" + ex.ToString());
            }
        }

        private bool CheckDispositionCode(WorkbinMailDetails mailDetails)
        {
            return (mailDetails != null && !string.IsNullOrEmpty(mailDetails.DispositionKey));
        }

        private bool CheckWorkbinAvailability()
        {
            if (WorkbinUtility.Instance().IsWorkbinEnable)
            {
                gridCompleteContainer.Visibility = Visibility.Visible;
                txtUnavailableError.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridCompleteContainer.Visibility = Visibility.Collapsed;
                txtUnavailableError.Visibility = Visibility.Visible;
            }
            //return !WorkbinUtility.Instance().IsWorkbinEnable;
            return true;
        }

        private void CloseError(object sender, EventArgs e)
        {
            try
            {
                CloseErrorMessage();
            }
            catch (Exception ex)
            {
                logger.Error("CloseSignatureError() : " + ex.Message);
            }
        }

        private void CloseErrorMessage()
        {
            grMessage.Height = new GridLength(0);
            if (_timerforcloseError != null)
            {
                _timerforcloseError.Stop();
                _timerforcloseError.Tick -= CloseError;
                _timerforcloseError = null;
            }
        }

        private void ContactServerNotification()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (WorkbinUtility.Instance().IsContactServerActive)
                    {
                        dgWorkbin_SelectedCellsChanged(null, null);
                    }
                    else
                        ViewMailButton();

                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            if (!(isShowOpen && isShowMarkDone && isShowReply && isShowDelete))
                e.Handled = true;

            //open
            (menu.Items[0] as MenuItem).Visibility = (isShowOpen && gcOpenMail.Width == GridLength.Auto) ? Visibility.Visible : Visibility.Collapsed;
            //markdone
            (menu.Items[1] as MenuItem).Visibility = (isShowMarkDone && gcMarkdone.Width == GridLength.Auto) ? Visibility.Visible : Visibility.Collapsed;
            //reply
            (menu.Items[2] as MenuItem).Visibility = (isShowReply && gcReply.Width == GridLength.Auto) ? Visibility.Visible : Visibility.Collapsed;
            //reply all
            (menu.Items[3] as MenuItem).Visibility = (isShowReplyAll && gcReplyAll.Width == GridLength.Auto) ? Visibility.Visible : Visibility.Collapsed;
            //delete
            (menu.Items[4] as MenuItem).Visibility = (isShowDelete && gcDelete.Width == GridLength.Auto) ? Visibility.Visible : Visibility.Collapsed;

            List<WorkbinMenuItem> SelectedWorkbin = null;
            WorkbinMenuItem selectedWorkbinMenu = tvWorkbin.SelectedItem as WorkbinMenuItem;
            //Move to option for Personal workbin.
            if (selectedWorkbinMenu != null)
            {
                if (isPersonalWorkbinSelected && workbinData.WorkbinMenu != null && workbinData.WorkbinMenu.Count > 0
                    && addedWorkbin != (selectedWorkbinMenu.AgentId + selectedWorkbinMenu.Title))
                {
                    SelectedWorkbin = workbinData.WorkbinMenu[0].Items.ToList();
                }
                else if (addedWorkbin != (selectedWorkbinMenu.AgentId + selectedWorkbinMenu.Title)) //Move to option
                {
                    SelectedWorkbin = workbinData.MyTeamWorkbinMenu[0].Items.Where(x => x.Items.Where(y => y.AgentId == selectedWorkbinMenu.AgentId).ToList().Count > 0)
                        .SingleOrDefault().Items.Where(y => y.AgentId == selectedWorkbinMenu.AgentId).SingleOrDefault().Items.ToList();
                }
                addedWorkbin = selectedWorkbinMenu.AgentId + selectedWorkbinMenu.Title;
            }

            if (SelectedWorkbin != null && (menu.Items[5] as MenuItem).Items != null)
            {
                (menu.Items[5] as MenuItem).Items.Clear();
                for (int index = 0; index < SelectedWorkbin.Count; index++)
                {
                    if (WorkbinUtility.Instance().SelectedWorkbinName != SelectedWorkbin[index].Title)
                    {
                        MenuItem menuItem = new MenuItem() { Header = SelectedWorkbin[index].Title };
                        menuItem.Click += new RoutedEventHandler(MoveMailWithInAgent_Click);
                        (menu.Items[5] as MenuItem).Items.Add(menuItem);
                    }

                }
                if ((menu.Items[5] as MenuItem).Items.Count > 0)
                    (menu.Items[5] as MenuItem).Visibility = Visibility.Visible;
                else
                    (menu.Items[5] as MenuItem).Visibility = Visibility.Collapsed;
            }
        }

        private void CreateWorkbinFieldsContextMenuItem(params string[] menuItems)
        {
            dgWorkbin.ContextMenu.Items.Clear();
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
                dgWorkbin.ContextMenu.Items.Add(menuItem);
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btnDelete.Visibility == Visibility.Visible;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnDelete_Click(null, null);
        }

        private void DetermineButtonVisibility()
        {
            if (workbinData.SelectedWorkbinDetails.Count > 0)
            {
                txtNoItem.Visibility = Visibility.Collapsed;
                ShowMainMenu();
                dgWorkbin.SelectedIndex = 0;
            }
            else
            {
                //txtNoItem.Visibility = Visibility.Visible;
                HideMainMenu();
                txtNoDetails.Visibility = Visibility.Visible;
                gridDetails.Visibility = dockCaseData.Visibility = Visibility.Collapsed;
            }
        }

        private void dgWorkbin_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            try
            {
                e.Cancel = true;
                WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;
                if (mailDetails != null)
                {
                    OpenMailInWindow(mailDetails.InteractionId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            //logger.Info("fired");
        }

        private void dgWorkbin_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //    if (e.Key == System.Windows.Input.Key.Delete && gcDelete.Width == GridLength.Auto)
                //    {
                //        btnDelete_Click(null, null);
                //    }
                //else if(e.Key==System.Windows.Input.Key.Enter && gcOpenMail.Width==GridLength.Auto)
                //{
                //    btnOpenMail_Click(null,null);
                //}
                //else
                if (e.Key == System.Windows.Input.Key.M && gcMarkdone.Width == GridLength.Auto)
                {
                    btnMarkDone_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void dgWorkbin_SelectedCellsChanged(object sender, Microsoft.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            //if (CheckWorkbinAvailability()) return;
            try
            {
                if (dgWorkbin.SelectedItem == null) return;
                WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;
                gcWorkbin.Width = GridLength.Auto;
                if (dgWorkbin.SelectedItems != null && dgWorkbin.SelectedItems.Count > 1)
                {
                    isShowReply = isShowReplyAll = isShowOpen = false;
                    isShowMarkDone = (dgWorkbin.SelectedItems.Cast<WorkbinMailDetails>().Where(x => x.TypeId.Equals("Inbound")).ToList().Count == dgWorkbin.SelectedItems.Count);
                    isShowDelete = (dgWorkbin.SelectedItems.Cast<WorkbinMailDetails>().Where(x => x.TypeId.Equals("Outbound")).ToList().Count == dgWorkbin.SelectedItems.Count);
                }
                else
                {
                    if (mailDetails.TypeId.Equals("Inbound"))
                    {
                        isShowDelete = false;
                        isShowReply = isShowOpen = true;
                        isShowMarkDone = true;
                        //isShowReplyAll = (!string.IsNullOrEmpty(mailDetails.CC) || !string.IsNullOrEmpty(mailDetails.BCC) || mailDetails.To.Contains(','));
                        isShowReplyAll = (!string.IsNullOrEmpty(mailDetails.CC) || !string.IsNullOrEmpty(mailDetails.BCC));
                    }
                    else
                    {
                        isShowMarkDone = isShowReply = isShowReplyAll = false;
                        isShowOpen = isShowDelete = true;
                    }
                }

                BindSelectedMailDetails(mailDetails);
                if (!WorkbinUtility.Instance().IsContactServerActive)
                {

                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private System.Windows.Controls.ContextMenu GetAttachmentMenus(UIElement uiElement)
        {
            System.Windows.Controls.ContextMenu conMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
            open.Header = "Open";
            open.Click += new RoutedEventHandler(MenuItem_Click);
            conMenu.Items.Add(open);
            conMenu.PlacementTarget = uiElement;

            return conMenu;
        }

        private string GetFullNameOfAgent(string empId)
        {
            CfgPersonQuery personQuery = new CfgPersonQuery();
            personQuery.EmployeeId = empId;
            personQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
            CfgPerson person = (CfgPerson)ConfigContainer.Instance().ConfServiceObject.RetrieveObject(personQuery);
            string name = string.Empty;
            if (person == null) return string.Empty;
            if (!string.IsNullOrEmpty(person.FirstName))
                name = person.FirstName;
            if (!string.IsNullOrEmpty(person.LastName))
                name = (string.IsNullOrEmpty(person.LastName) ? "" : name + " ") + person.LastName;
            return name;
        }

        private void GetLastAccessWorkbin()
        {
            if (ConfigContainer.Instance().AllKeys.Contains("CfgPerson"))
            {
                CfgPerson person = ConfigContainer.Instance().GetValue("CfgPerson") as CfgPerson;
                if (person != null)
                {
                    if (person.UserProperties.ContainsKey("agent.ixn.desktop"))
                    {
                        KeyValueCollection option = person.UserProperties["agent.ixn.desktop"] as KeyValueCollection;
                        if (option.ContainsKey("workbins.workbin-selected"))
                        {
                            WorkbinUtility.Instance().SelectedWorkbinName = option["workbins.workbin-selected"].ToString();
                        }
                    }
                }
                person = null;
            }
        }

        private void HideLoading()
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Collapsed;
                ImgPreload.Visibility = System.Windows.Visibility.Collapsed;
                Panel.SetZIndex(BrdOpacity, 0);
                Panel.SetZIndex(ImgPreload, 0);
            }));
        }

        private void HideMainMenu()
        {
            //Dispatcher.Invoke(new Action(() =>
            //{
            txtNoItem.Visibility = Visibility.Visible;
            StandardEmailImage.Visibility = Middle.Visibility = Visibility.Collapsed;
            //}));

            //gcWorkbin.Width = gcOpenMail.Width = gcReply.Width = gcReplyAll.Width = gcMarkdone.Width = gcDelete.Width=gcInteractionQueue.Width =new GridLength(0);
        }

        void InteractionService_InteractionServerNotifier(bool isOpen)
        {
            Dispatcher.Invoke(
                new Action(() =>
                {
                    try
                    {
                        if (isOpen)
                        {
                            txtNoItem.Visibility = Visibility.Collapsed;
                            tvWorkbin.Visibility = gridWorkbinExplorer.Visibility = Visibility.Visible;
                            if (dgWorkbin.SelectedItem != null)
                                dgWorkbin_SelectedCellsChanged(null, null);
                        }
                        else
                        {
                            txtNoItem.Visibility = Visibility.Visible;
                            tvWorkbin.Visibility = gridWorkbinExplorer.Visibility = Visibility.Collapsed;
                            ShowCurrentWorkbin.Header = "No Workbin Selected";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occurred as " + ex.Message);
                    }

                }));
        }

        private bool IsChildInOpen(string interactionId)
        {
            List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
            List<Window> childWindow = emailWindows.Where(x => interactionId.Equals(((Pointel.Interactions.IPlugins.IEmailAttribute)x).ParentInteractionID)).ToList();
            return childWindow != null && childWindow.Count > 0;
        }

        private bool IsEmailReachMaximumCount()
        {
            int maximumEmailCount = 5;

            if (ConfigContainer.Instance().AllKeys.Contains("email.max.intstance-count"))
                int.TryParse(((string)ConfigContainer.Instance().GetValue("email.max.intstance-count")), out maximumEmailCount);
            List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email") && (x as Pointel.Interactions.IPlugins.IEmailAttribute).InteractionID != null).ToList();
            if (emailWindows.Count == maximumEmailCount)
            {
                starttimerforerror("Email reached maximum count. Please close opened mail and then try to open.");
                return true;
            }
            return false;
        }

        private UIElement LoadAttachments(string Name, string size, string docId)
        {
            System.Windows.Controls.Button btnAttachment = new System.Windows.Controls.Button();
            btnAttachment.Style = (Style)FindResource("AttachmentButton");
            btnAttachment.Click += new RoutedEventHandler(btnAttachment_Click);
            btnAttachment.Margin = new Thickness(3);
            btnAttachment.Height = 20;

            btnAttachment.Content = "  " + Name + " (" + GetFileSize(Convert.ToInt64(size)) + ")  ";
            btnAttachment.Tag = docId;
            btnAttachment.ContextMenu = GetAttachmentMenus(btnAttachment);
            btnAttachment.HorizontalAlignment = HorizontalAlignment.Left;
            btnAttachment.Margin = new Thickness(5);

            return btnAttachment;
        }

        private void LoadMail(string workbinName, string agentId)
        {
            if (kvpInteractions != null)
            {

                foreach (string interactionId in kvpInteractions.AllKeys)
                    RetriveAndStore(interactionId, workbinName, agentId);
            }
        }

        private void LoadMailDisabledImages()
        {
            try
            {
                ////Common Button
                //imgQueueMail.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Email-01.png", UriKind.Relative));
                imgWorkbinMail.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyMail-disabled.png", UriKind.Relative));
                imgOpen.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Email-01.png", UriKind.Relative));

                //Inbound Button
                imgReply.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyMail-disabled.png", UriKind.Relative));
                imgReplyAll.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyAllMail-disabled.png", UriKind.Relative));
                imgMarkDone.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Done-disabled.png", UriKind.Relative));

                //Outbound Button
                imgDelete.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Delete Mail-disabled.png", UriKind.Relative));

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);

            }
        }

        private void LoadMailImages()
        {
            try
            {
                ////Common Button
                //imgQueueMail.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Email-01.png", UriKind.Relative));
                ///Pointel.Interactions.Email;component/Images/E-Mail/Transfer.png
                imgWorkbinMail.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Transfer.png", UriKind.Relative));
                //  mnuOpenimage.Source=
                imgOpen.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyMail-01.png", UriKind.Relative));
                //ReplyMail.png

                //Inbound Button
                imgReply.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyMail.png", UriKind.Relative));
                imgReplyAll.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\ReplyAllMail-01.png", UriKind.Relative));
                // imgMarkDone.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Done-01.png", UriKind.Relative));
                imgMarkDone.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Mark Done.png", UriKind.Relative));
                //Mark Done.png

                //Outbound Button
                imgDelete.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Delete Mail-01.png", UriKind.Relative));

                //Expand Collapse
                imgExpand.Source = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\HideDetails.png", UriKind.Relative));

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);

            }
        }

        private void LoadMyTeamlWorkbin()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ShowLoading();
                    if (workbinData.SelectedWorkbinDetails != null)
                        workbinData.SelectedWorkbinDetails.Clear();
                    if (WorkbinUtility.Instance().TeamWorkbinList != null)
                    {
                        if (workbinData.MyTeamWorkbinMenu.Count == 0 && WorkbinUtility.Instance().AgentGroup != null && WorkbinUtility.Instance().AgentGroup.Count > 0)
                        {
                            WorkbinMenuItem teamWorkbinRoot = new WorkbinMenuItem() { Title = "Team Workbins" };
                            List<string> agentName = new List<string>();
                            foreach (CfgAgentGroup agentGroup in WorkbinUtility.Instance().AgentGroup)
                            {
                                WorkbinMenuItem agentRoot = new WorkbinMenuItem() { Title = agentGroup.GroupInfo.Name };
                                foreach (CfgPerson person in agentGroup.Agents)
                                {
                                    if (!agentName.Contains(person.UserName))
                                    {
                                        if (person.EmployeeID != ConfigContainer.Instance().AgentId)
                                        {
                                            WorkbinMenuItem personRoot = new WorkbinMenuItem() { Title = person.UserName, AgentId = person.EmployeeID };
                                            foreach (string item in WorkbinUtility.Instance().TeamWorkbinList.Keys)
                                            {
                                                LoadWorkbinMail(item, person.EmployeeID);
                                                if (kvpInteractions != null)
                                                {
                                                    WorkbinMenuItem menuItem = new WorkbinMenuItem() { Title = item, Count = " (" + kvpInteractions.Count + ")", Visible = Visibility.Visible, AgentId = person.EmployeeID };
                                                    personRoot.Items.Add(menuItem);
                                                    LoadMail(item, person.EmployeeID);
                                                    agentName.Add(person.UserName);
                                                }
                                                else
                                                {
                                                    personRoot.Items.Add(new WorkbinMenuItem() { Title = item, Count = " (0)", Visible = Visibility.Visible, AgentId = person.EmployeeID });
                                                }

                                            }
                                            agentRoot.Items.Add(personRoot);
                                        }
                                    }
                                }
                                teamWorkbinRoot.Items.Add(agentRoot);
                            }
                            agentName = null;
                            workbinData.MyTeamWorkbinMenu.Add(teamWorkbinRoot);
                        }
                        if (tvWorkbin.ItemsSource == null)
                            tvWorkbin_SelectedItemChanged(null, null);
                        tvWorkbin.ItemsSource = workbinData.MyTeamWorkbinMenu;


                        //if there is no my team workbin configured to agent means, it will show "No workbin configured message".
                        if (workbinData.MyTeamWorkbinMenu.Count > 0)
                        {
                            txtErrorinConfig.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            txtErrorinConfig.Visibility = Visibility.Visible;
                            txtErrorinConfig.Text = "Workbin not configured";
                        }
                    }
                    HideLoading();
                }));
        }

        private void LoadPersonalWorkbin()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ShowLoading();
                    workbinData.WorkbinDetails = new System.Collections.ObjectModel.ObservableCollection<WorkbinMailDetails>();
                    txtErrorinConfig.Visibility = Visibility.Collapsed;
                    if (WorkbinUtility.Instance().PersonalWorkbinList != null && WorkbinUtility.Instance().PersonalWorkbinList.Count > 0)
                    {
                        if (workbinData.WorkbinMenu.Count == 0)
                        {
                            bool selected = false;
                            WorkbinMenuItem personalWorkbinRoot = new WorkbinMenuItem() { Title = "Personal Workbins" };
                            List<WorkbinMenuItem> workbin = new List<WorkbinMenuItem>();
                            foreach (string item in WorkbinUtility.Instance().PersonalWorkbinList.Keys)
                            {
                                LoadWorkbinMail(item, ConfigContainer.Instance().AgentId);
                                if (kvpInteractions != null)
                                {
                                    WorkbinMenuItem menuItem = new WorkbinMenuItem() { Title = item, Count = " (" + kvpInteractions.Count + ")", Visible = Visibility.Visible, AgentId = ConfigContainer.Instance().AgentId };
                                    if (menuItem.Title == WorkbinUtility.Instance().SelectedWorkbinName)
                                    {
                                        menuItem.IsSelected = true;
                                        selected = true;
                                    }
                                    personalWorkbinRoot.Items.Add(menuItem);
                                    LoadMail(item, ConfigContainer.Instance().AgentId);
                                }
                                else
                                {
                                    personalWorkbinRoot.Items.Add(new WorkbinMenuItem() { Title = item, Count = " (0)", Visible = Visibility.Visible, AgentId = ConfigContainer.Instance().AgentId });
                                }

                            }

                            workbinData.WorkbinMenu.Add(personalWorkbinRoot);
                            personalWorkbinRoot.IsExpanded = true;
                            if (!selected && workbinData.WorkbinMenu[0].Items.Count > 0)
                            {
                                workbinData.WorkbinMenu[0].Items[0].IsSelected = true;
                            }
                        }
                        tvWorkbin.ItemsSource = workbinData.WorkbinMenu;
                        txtLoading.Visibility = Visibility.Collapsed;
                    }
                    else
                    {

                    }
                    HideLoading();
                }));
        }

        private System.Windows.Controls.UserControl LoadTeamCommunicatorWorkbin()
        {
            System.Windows.Controls.UserControl teamCommunicatorWorkbin = null;
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.WorkBin);

                if (!_isInteractionQueue)
                {
                    WorkbinMailDetails mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;
                    if (mailDetails != null)
                    {
                        if (mailDetails.AgentId.Equals(ConfigContainer.Instance().AgentId))
                            container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Workbin);
                        else
                            //if (mailDetails.AgentId.Equals(ConfigContainer.Instance().AgentId))
                            container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Supervisor);
                    }
                }
                else
                    container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Workbin);


                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                Importer ImportClass = new Importer();
                container.ComposeParts(ImportClass);
                teamCommunicatorWorkbin = (from d in ImportClass.win
                                           where d.Name == "TeamCommunicator"
                                           select d).FirstOrDefault() as System.Windows.Controls.UserControl;
            }
            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return teamCommunicatorWorkbin;
        }

        private void LoadWorkbinMail(string workbinName, string agentId)
        {
            if (!string.IsNullOrEmpty(workbinName))
            {
                InteractionService service = new InteractionService();

                Pointel.Interactions.Core.Common.OutputValues result = service.GetWorkbinContent(agentId,
                    WorkbinUtility.Instance().PlaceID, workbinName, WorkbinUtility.Instance().IxnProxyID);

                if (result != null && result.MessageCode.Equals("200"))
                {
                    EventWorkbinContent content = (EventWorkbinContent)result.IMessage;
                    kvpInteractions = content.Interactions;
                }
            }
        }

        private void MarkDoneAllSelectedMails()
        {
            List<string> mailToOpen = new List<string>();
            List<string> mailToMarkDone = new List<string>();
            bool needToMarkdone = true;
            bool isDispositionMaditory = (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory") &&
                "true".Equals(ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")));

            //if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory") &&
            //    "true".Equals(ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")))
            //{
            for (int index = 0; index < dgWorkbin.SelectedItems.Count; index++)
            {
                WorkbinMailDetails mailDetails = dgWorkbin.SelectedItems[index] as WorkbinMailDetails;
                if (mailDetails != null)
                {
                    mailToMarkDone.Add(mailDetails.InteractionId);
                    if (isDispositionMaditory && !CheckDispositionCode(mailDetails))
                    {
                        mailToOpen.Add(mailDetails.InteractionId);
                        mailToMarkDone.Remove(mailDetails.InteractionId);
                    }
                }
            }

            if (mailToOpen.Count > 0)
            {
                PopUpWindow popupWindow = new PopUpWindow();
                if (mailToOpen.Count == dgWorkbin.SelectedItems.Count)
                {
                    popupWindow.Message = "Disposition code is mandatory.";
                    popupWindow.btnOK.Content = "OK";
                    popupWindow.btnCancel.Visibility = System.Windows.Visibility.Collapsed;
                    popupWindow.ShowDialog();
                }
                else
                {
                    if (mailToOpen.Count == 1)
                        popupWindow.Message = "Disposition code not selected for one mail, Are you sure that want to mark done remaining mails?";
                    else
                        popupWindow.Message = "Disposition code not selected for some mails, Are you sure that want to mark done remaining mails?";
                    popupWindow.btnOK.Content = "Yes";
                    popupWindow.btnCancel.Visibility = System.Windows.Visibility.Visible;
                    popupWindow.ShowDialog();
                    if (popupWindow.DialogResult == false)
                    {
                        mailToOpen.Clear();
                        needToMarkdone = false;
                    }
                }

                foreach (string interactionId in mailToOpen)
                {
                    OpenMailInWindow(interactionId);
                }
            }
            //}

            if (ConfigContainer.Instance().AllKeys.Contains("email.enable.prompt-for-done")
                && "true".Equals(ConfigContainer.Instance().GetValue("email.enable.prompt-for-done").ToString())
                && needToMarkdone && mailToOpen.Count == 0)
            {
                PopUpWindow popupWindow = new PopUpWindow();
                if (mailToMarkDone.Count > 1)
                    popupWindow.Message = "Are you sure want to mark done the interactions?";
                else
                    popupWindow.Message = "Are you sure want to mark done the interaction?";
                popupWindow.btnOK.Content = "Yes";
                popupWindow.btnCancel.Visibility = System.Windows.Visibility.Visible;
                bool? result = popupWindow.ShowDialog();
                if (result == false)
                {
                    return;
                }
            }
            else if (!needToMarkdone)
                return;

            int failedInteraction = 0;
            int successInteraction = 0;
            for (int index = 0; index < mailToMarkDone.Count; index++)
            {
                InteractionService interactionService = new InteractionService();
                KeyValueCollection keyValue = new KeyValueCollection();
                keyValue.Add("id", mailToMarkDone[index]);
                Pointel.Interactions.Core.Common.OutputValues output = interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, keyValue);
                if (output.MessageCode.Equals("200"))
                {
                    output = interactionService.StopProcessingInteraction(WorkbinUtility.Instance().IxnProxyID, mailToMarkDone[index]);
                    if (output.MessageCode == "200")
                    {
                        WorkbinMailDetails mailDetails = workbinData.SelectedWorkbinDetails.Where(x => x.InteractionId.Equals(mailToMarkDone[index])).SingleOrDefault();
                        ContactServerHelper.UpdateInteraction(mailToMarkDone[index], ConfigContainer.Instance().PersonDbId, mailDetails.EmailNotes, mailDetails.UserData, 3, DateTime.UtcNow.ToString());
                        successInteraction++;
                        NotifyHistoryRefresh(mailDetails.InteractionId, false);
                    }
                    else
                        failedInteraction++;
                }
                else
                    failedInteraction++;
            }

            if (mailToMarkDone.Count > 0)
            {
                if (failedInteraction > 0)
                    starttimerforerror(successInteraction + " interaction got success and " + failedInteraction + " got failure.");
                else if (successInteraction > 0)
                    starttimerforerror("The operation completed successfully.");
            }
        }

        private void MarkDoneSelectedMail()
        {
            WorkbinMailDetails mailDetails = null;
            if (_isInteractionQueue)
                mailDetails = tbCallBack.DataContext as WorkbinMailDetails;
            else
                mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;

            if (mailDetails != null)
            {
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory") &&
                "true".Equals(ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")))
                {
                    if (_isInteractionQueue && ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.check-on-iq")
                        && !ConfigContainer.Instance().GetAsBoolean("interaction.disposition.check-on-iq"))
                    {
                        // This code added for feature use.
                    }
                    else if (!CheckDispositionCode(mailDetails))
                    {
                        ShowMandatoryMessage();
                        return;
                    }
                }

                if (ConfigContainer.Instance().AllKeys.Contains("email.enable.prompt-for-done") && "true".Equals(ConfigContainer.Instance().GetValue("email.enable.prompt-for-done")))
                {
                    PopUpWindow popupWindow = new PopUpWindow();
                    popupWindow.Message = "Are you sure want to mark done the interaction?";
                    popupWindow.btnOK.Content = "Yes";
                    popupWindow.btnCancel.Visibility = System.Windows.Visibility.Visible;
                    bool? result = popupWindow.ShowDialog();
                    if (result == false)
                    {
                        return;
                    }
                }
                InteractionService interactionService = new InteractionService();
                KeyValueCollection keyValue = new KeyValueCollection();
                keyValue.Add("id", mailDetails.InteractionId);
                Pointel.Interactions.Core.Common.OutputValues output = interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, keyValue);
                if (output.MessageCode.Equals("200"))
                {
                    output = interactionService.StopProcessingInteraction(WorkbinUtility.Instance().IxnProxyID, mailDetails.InteractionId);
                    if (output.MessageCode == "200")
                    {
                        ContactServerHelper.UpdateInteraction(mailDetails.InteractionId, ConfigContainer.Instance().PersonDbId, mailDetails.EmailNotes, mailDetails.UserData, 3, DateTime.UtcNow.ToString());

                        if (_isInteractionQueue)
                            IQRemoveNotification();

                        starttimerforerror("The operation completed successfully.");

                        NotifyHistoryRefresh(mailDetails.InteractionId, false);
                    }
                }

            }
        }

        private void MarkDoneMultipleMailIQ()
        {
            Dictionary<string, WorkbinMailDetails> dicMailToMarkdone = new Dictionary<string, WorkbinMailDetails>();
            List<string> lstMailToOpen = new List<string>();
            bool isDispositionMaditory = (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.is-mandatory")
                && "true".Equals(ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")));

            if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.check-on-iq")
                && !ConfigContainer.Instance().GetAsBoolean("interaction.disposition.check-on-iq"))
                isDispositionMaditory = false;



            if (WorkbinUtility.Instance().IsContactServerActive)
            {
                foreach (var item in dgIQ.SelectedItems)
                {
                    DataRowView selectedRow = item as DataRowView;
                    if (selectedRow != null)
                    {
                        string interactionIdToMarkdone = selectedRow["InteractionId"].ToString();

                        Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance()
                            .PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionContent(interactionIdToMarkdone, true);

                        if (result != null && result.Id == EventGetInteractionContent.MessageId)
                        {
                            EventGetInteractionContent interactionContent = result as EventGetInteractionContent;
                            //ParseAndStoreMail(interactionContent, ref selectedMailDetails);
                            WorkbinMailDetails mailDetails = new WorkbinMailDetails();
                            mailDetails.InteractionId = interactionIdToMarkdone;
                            ParseAndStoreMail(interactionContent, ref mailDetails);
                            if (isDispositionMaditory)
                            {
                                if (CheckDispositionCode(mailDetails))
                                    dicMailToMarkdone.Add(interactionIdToMarkdone, mailDetails);
                                else
                                    lstMailToOpen.Add(interactionIdToMarkdone);
                            }
                            else
                                dicMailToMarkdone.Add(interactionIdToMarkdone, mailDetails);
                        }
                        else
                            logger.Warn("The EventGetInteractionContent request failed for the interaction Id: " + interactionIdToMarkdone);

                    }
                    else
                        logger.Warn("The selected item is null after perform the conversion.");
                }

                if (dicMailToMarkdone.Count != dgIQ.SelectedItems.Count)
                {
                    if (dicMailToMarkdone.Count == 0)
                        ShowMessage("Disposition code is mandatory.");
                    else
                    {
                        PopUpWindow popupWindow = new PopUpWindow();
                        if (lstMailToOpen.Count == 1)
                            popupWindow.Message = "Disposition code not selected for one mail, Are you sure that want to mark done remaining mails?";
                        else
                            popupWindow.Message = "Disposition code not selected for some mails, Are you sure that want to mark done remaining mails?";
                        popupWindow.btnOK.Content = "Yes";
                        popupWindow.btnCancel.Visibility = System.Windows.Visibility.Visible;
                        popupWindow.ShowDialog();
                        if (popupWindow.DialogResult == false)
                            return;
                    }
                }


                if (dicMailToMarkdone.Count > 0 && ConfigContainer.Instance().AllKeys.Contains("email.enable.prompt-for-done")
               && ConfigContainer.Instance().GetAsBoolean("email.enable.prompt-for-done") && lstMailToOpen.Count == 0)
                {
                    PopUpWindow popupWindow = new PopUpWindow();
                    if (dicMailToMarkdone.Count > 1)
                        popupWindow.Message = "Are you sure want to mark done the interactions?";
                    else
                        popupWindow.Message = "Are you sure want to mark done the interaction?";
                    popupWindow.btnOK.Content = "Yes";
                    popupWindow.btnCancel.Visibility = System.Windows.Visibility.Visible;
                    bool? result = popupWindow.ShowDialog();
                    if (result == false)
                        return;
                }


                for (int index = 0; index < lstMailToOpen.Count; index++)
                    OpenMailInWindow(lstMailToOpen[index]);

                InteractionService interactionService = new InteractionService();
                int failedInteraction = 0;
                int successInteraction = 0;

                foreach (string keyName in dicMailToMarkdone.Keys)
                {
                    WorkbinMailDetails mailDetails = dicMailToMarkdone[keyName];

                    KeyValueCollection keyValue = new KeyValueCollection();
                    keyValue.Add("id", mailDetails.InteractionId);
                    Pointel.Interactions.Core.Common.OutputValues output = interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, keyValue);
                    if (output.MessageCode.Equals("200"))
                    {
                        output = interactionService.StopProcessingInteraction(WorkbinUtility.Instance().IxnProxyID, mailDetails.InteractionId);
                        if (output.MessageCode == "200")
                        {
                            ContactServerHelper.UpdateInteraction(mailDetails.InteractionId, ConfigContainer.Instance().PersonDbId, mailDetails.EmailNotes, mailDetails.UserData, 3, DateTime.UtcNow.ToString());
                            successInteraction++;
                            NotifyHistoryRefresh(mailDetails.InteractionId, false);
                        }
                        else
                            failedInteraction++;
                    }
                    else
                        failedInteraction++;
                }

                if (failedInteraction != dicMailToMarkdone.Count)
                    IQRemoveNotification();

                if (failedInteraction > 0)
                    starttimerforerror(successInteraction + " interaction got success and " + failedInteraction + " got failure.");
                else if (successInteraction > 0)
                    starttimerforerror("The operation completed successfully.");

            }
            else
            {
                logger.Warn("The contact server not active.");

                // TODO : Need to show message to user.

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (CheckWorkbinAvailability()) return;
            var menuitem = sender as System.Windows.Controls.MenuItem;
            try
            {

                FileStream fs = null;
                switch (menuitem.Header.ToString().ToLower())
                {
                    case "open":
                        var docId = ((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString();
                        IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Plugins.Contact]).GetDocument(docId);
                        if (response != null && response.Id.Equals(EventGetDocument.MessageId))
                        {
                            EventGetDocument getAttachDocument = response as EventGetDocument;
                            if (getAttachDocument != null)
                            {
                                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp\" + docId.ToString() + @"\";
                                logger.Info("Opening the file : " + getAttachDocument.TheName);
                                if (string.IsNullOrEmpty(Path.GetDirectoryName(getAttachDocument.TheName)))
                                    getAttachDocument.TheName = path + @"\" + getAttachDocument.TheName;
                                logger.Info("Creating the file : " + getAttachDocument.TheName);
                                if (!Directory.Exists(Path.GetDirectoryName(getAttachDocument.TheName)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(getAttachDocument.TheName));
                                using (fs = new FileStream(getAttachDocument.TheName, FileMode.Create)) { }
                                File.WriteAllBytes(getAttachDocument.TheName, getAttachDocument.Content);
                                var process = Process.Start(getAttachDocument.TheName);
                                process.EnableRaisingEvents = true;
                                process.Exited += new EventHandler(process_Exited);
                            }
                        }
                        else if (response != null)
                        {
                            logger.Warn(response.ToString());
                        }

                        break;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while " + menuitem.Header.ToString().ToLower() + " the attachment '" + menuitem.Header.ToString().Trim() + "'  as :" + generalException.ToString());
            }
        }

        private void MenuOpenMail_Click(object sender, RoutedEventArgs e)
        {
            btnOpenMail_Click(null, null);
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete_Click(null, null);
        }

        private void mnuMarkDone_Click(object sender, RoutedEventArgs e)
        {
            btnMarkDone_Click(null, null);
        }

        private void mnuReplyAll_Click(object sender, RoutedEventArgs e)
        {
            btnReplyAllMail_Click(null, null);
        }

        private void mnuReply_Click(object sender, RoutedEventArgs e)
        {
            btnReplyMail_Click(null, null);
        }

        private void MoveMailIntoAgentWorkbin(Dictionary<string, string> dictionaryValues)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                List<string> lstInteractionId = new List<string>();

                if (_isInteractionQueue)
                {
                    if (dgIQ.SelectedItems != null && dgIQ.SelectedItems.Count > 0)
                    {
                        foreach (var item in dgIQ.SelectedItems)
                        {
                            DataRowView row = item as DataRowView;
                            if (row != null)
                                lstInteractionId.Add(row["InteractionId"].ToString());
                        }
                    }
                }
                else
                {
                    if (dgWorkbin.SelectedItems != null && dgWorkbin.SelectedItems.Count > 0)
                    {
                        foreach (var item in dgWorkbin.SelectedItems)
                        {
                            WorkbinMailDetails mailDetail = item as WorkbinMailDetails;
                            if (mailDetail != null)
                                lstInteractionId.Add(mailDetail.InteractionId);
                        }
                    }
                }

                InteractionService interactionService = new InteractionService();
                string workbinName = dictionaryValues["WorkbinName"].ToString();
                string agentId = dictionaryValues["EmployeeId"].ToString();

                int failedInteraction = 0;
                int successInteraction = 0;

                if (lstInteractionId.Count > 0)
                {
                    foreach (string interactionId in lstInteractionId)
                    {
                        KeyValueCollection keyValue = new KeyValueCollection();
                        keyValue.Add("id", interactionId);
                        Pointel.Interactions.Core.Common.OutputValues result = interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, keyValue);
                        if (result != null && result.MessageCode.Equals("200"))
                        {
                            //  string queueName = dictionaryValues["UniqueIdentity"].ToString();
                            result = interactionService.PlaceInWorkbin(interactionId, agentId,
                                // placeId,
                                workbinName, WorkbinUtility.Instance().IxnProxyID);
                            if (result != null && result.MessageCode.Equals("200"))
                            {
                                logger.Info(interactionId + " Placed in " + workbinName + " Workbin");
                                successInteraction++;

                                // If it is IQ, Need to notify to Remove interaction
                                if (_isInteractionQueue)
                                    IQRemoveNotification();

                            }
                            else
                                failedInteraction++;
                        }
                        else
                            failedInteraction++;
                    }

                    if (failedInteraction > 0)
                        starttimerforerror(successInteraction + " interaction moved successfully and " + failedInteraction + " got failure.");
                    else if (successInteraction == 1)
                        starttimerforerror("The interaction moved successfully.");
                    else
                        starttimerforerror("The interactions moved successfully.");

                }
                else
                    starttimerforerror("Please the interaction to perform the operation.");


            }));
        }

        private void MoveMailIntoQueue(Dictionary<string, string> dictionaryValues)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    List<string> lstInteractionId = new List<string>();

                    if (_isInteractionQueue)
                    {
                        if (dgIQ.SelectedItems != null && dgIQ.SelectedItems.Count > 0)
                        {
                            foreach (var item in dgIQ.SelectedItems)
                            {
                                DataRowView row = item as DataRowView;
                                if (row != null)
                                    lstInteractionId.Add(row["InteractionId"].ToString());
                            }
                        }
                    }
                    else
                    {
                        if (dgWorkbin.SelectedItems != null && dgWorkbin.SelectedItems.Count > 0)
                        {
                            foreach (var item in dgWorkbin.SelectedItems)
                            {
                                WorkbinMailDetails mailDetail = item as WorkbinMailDetails;
                                if (mailDetail != null)
                                    lstInteractionId.Add(mailDetail.InteractionId);
                            }
                        }
                    }

                    InteractionService interactionService = new InteractionService();
                    if (lstInteractionId.Count > 0)
                    {
                        int failedInteraction = 0;
                        int successInteraction = 0;

                        foreach (string interactionId in lstInteractionId)
                        {
                            KeyValueCollection keyValue = new KeyValueCollection();
                            keyValue.Add("id", interactionId);

                            Pointel.Interactions.Core.Common.OutputValues result =
                                interactionService.PullInteraction(ConfigContainer.Instance().TenantDbId,
                                WorkbinUtility.Instance().IxnProxyID, keyValue);

                            if (result != null && result.MessageCode.Equals("200"))
                            {
                                string queueName = dictionaryValues["UniqueIdentity"].ToString();
                                result = interactionService.PlaceInQueue(interactionId, WorkbinUtility.Instance().IxnProxyID, queueName);
                                if (result != null && result.MessageCode.Equals("200"))
                                {
                                    logger.Info(interactionId + " Placed in " + queueName + " Queue");
                                    successInteraction++;
                                    // If It is IQ, Need to notify to remove the interaction.
                                    if (_isInteractionQueue)
                                        IQRemoveNotification();

                                }
                                else
                                    failedInteraction++;
                            }
                            else
                                failedInteraction++;
                        }

                        if (failedInteraction > 0)
                            starttimerforerror(successInteraction + " interaction moved successfully and " + failedInteraction + " got failure.");
                        else if (successInteraction == 1)
                            starttimerforerror("The interaction moved successfully.");
                        else
                            starttimerforerror("The interactions moved successfully.");
                    }
                    else
                        starttimerforerror("Please the interaction to perform the operation.");

                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred in 'MoveMailIntoQueue' as " + ex.Message);
                }

            }));
        }

        void MoveMailWithInAgent_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    MenuItem menuItem = sender as MenuItem;
                    if (menuItem != null)
                    {

                        Dictionary<string, string> dictionaryValue = new Dictionary<string, string>();
                        dictionaryValue.Add("WorkbinName", menuItem.Header.ToString());
                        WorkbinMenuItem workbinMenuItem = tvWorkbin.SelectedItem as WorkbinMenuItem;
                        dictionaryValue.Add("EmployeeId", workbinMenuItem.AgentId);
                        MoveMailIntoAgentWorkbin(dictionaryValue);

                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        private void NotifyHistoryRefresh(string interactionId, bool isDelete)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).RefreshContactHistory(interactionId, isDelete);
        }

        private void NotifyWorkbinChangedEvent(string agentId, string workbinName, string interactionId, WorkbinContentOperation operation)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(workbinName) && !string.IsNullOrEmpty(interactionId))
                    {
                        if (operation == WorkbinContentOperation.PlacedIn)
                        {
                            RetriveAndStore(interactionId, workbinName, agentId);
                        }
                        else if (operation == WorkbinContentOperation.TakenOut)
                        {
                            WorkbinMenuItem selectedWorkbin = tvWorkbin.SelectedItem as WorkbinMenuItem;
                            if (lstEmailStatusProgress.Contains(interactionId))
                                lstEmailStatusProgress.Remove(interactionId);
                            WorkbinMailDetails maildetails = workbinData.WorkbinDetails.Where(x => x.InteractionId.Equals(interactionId)).SingleOrDefault();
                            workbinData.WorkbinDetails.Remove(maildetails);
                        }

                        if (agentId.Equals(ConfigContainer.Instance().AgentId) && workbinData.WorkbinMenu.Count > 0) //Personal Workbin
                        {
                            workbinData.WorkbinMenu[0].Items.Where(x => x.Title.Equals(workbinName) && x.AgentId.Equals(agentId)).SingleOrDefault().Count = " (" +
                               workbinData.WorkbinDetails.Where(x => x.WorkbinName.Equals(workbinName) && x.AgentId.Equals(agentId)).ToList().Count + ")";
                        }
                        else if (workbinData.MyTeamWorkbinMenu.Count > 0)
                        {
                            foreach (WorkbinMenuItem agentMenuitem in workbinData.MyTeamWorkbinMenu[0].Items)
                            {
                                if (agentMenuitem.Items.Where(x => x.Items.Where(y => y.Title.Equals(workbinName) && y.AgentId.Equals(agentId)).ToList().Count > 0).ToList().Count > 0)
                                {
                                    agentMenuitem.Items.Where(x => x.Items.Where(y => y.Title.Equals(workbinName) && y.AgentId.Equals(agentId)).ToList().Count > 0).SingleOrDefault().Items.Where(x => x.Title.Equals(workbinName)).SingleOrDefault().Count = " (" +
                               workbinData.WorkbinDetails.Where(x => x.WorkbinName.Equals(workbinName) && x.AgentId.Equals(agentId)).ToList().Count + ")";
                                    break;
                                }
                            }
                        }

                        WorkbinMenuItem selectedMenu = tvWorkbin.SelectedItem as WorkbinMenuItem;
                        if (selectedMenu != null && agentId.Equals(selectedMenu.AgentId) && selectedMenu.Title.Equals(workbinName))
                        {
                            workbinData.SelectedWorkbinDetails = workbinData.WorkbinDetails.Where(x => x.WorkbinName.Equals(workbinName)
                                && x.AgentId.Equals(selectedMenu.AgentId)).OrderBy(x => x.ReceivedDate).Reverse().ToList();
                            DetermineButtonVisibility();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occcurred as " + ex.Message);
                }
            }));
        }

        private void OpenMailInWindow(string interactionId)
        {
            if (_isInteractionQueue || !lstEmailStatusProgress.Contains(interactionId))
            {
                if (!WorkbinUtility.Instance().IsWorkbinEnable) return;
                if (!WorkbinUtility.Instance().IsContactServerActive) return;
                if (IsEmailReachMaximumCount()) return;

                if (!string.IsNullOrEmpty(interactionId))
                {
                    string typeID = string.Empty;
                    string parentInteractionId = string.Empty;

                    if (_isInteractionQueue)
                    {
                        if (dicKVPIQInteractions.Keys.Contains(interactionId))
                        {
                            KeyValueCollection kvInteraction = dicKVPIQInteractions[interactionId];
                            if (kvInteraction.ContainsKey("InteractionType"))
                                typeID = kvInteraction.GetAsString("InteractionType");

                            if (kvInteraction.ContainsKey("ParentId"))
                                parentInteractionId = kvInteraction.GetAsString("ParentId");

                        }
                        else
                            logger.Warn("The interaction Id not found in interaction dictionary.");
                    }
                    else
                    {
                        WorkbinMailDetails workbinMail = workbinData.WorkbinDetails.
                                                   Where(x => x.InteractionId.Equals(interactionId)).SingleOrDefault();
                        typeID = workbinMail.TypeId;
                        parentInteractionId = workbinMail.ParentInteractionId;
                    }


                    if ("Inbound".Equals(typeID) && IsChildInOpen(interactionId))
                    {
                        starttimerforerror("Open action failed.");
                        return;
                    }
                    else
                    {
                        List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
                        List<Window> shouldCloseWindow = emailWindows.Where(x => ((Pointel.Interactions.IPlugins.IEmailAttribute)x).InteractionID == null).ToList();
                        if (shouldCloseWindow != null)
                            foreach (Window window in shouldCloseWindow)
                                window.Close();

                        List<Window> openedWindow = emailWindows.Where(x => ((Pointel.Interactions.IPlugins.IEmailAttribute)x).InteractionID.Equals(parentInteractionId)).ToList();
                        if (openedWindow != null)
                            foreach (Window window in openedWindow)
                                window.Close();
                    }
                    KeyValueCollection kvpInteractionId = new KeyValueCollection();
                    kvpInteractionId.Add("id", interactionId);
                    InteractionService interactionServices = new InteractionService();
                    Pointel.Interactions.Core.Common.OutputValues result = interactionServices.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, kvpInteractionId);
                    if (result.MessageCode.Equals("200"))
                    {
                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                        {
                            if (!_isInteractionQueue)
                                lstEmailStatusProgress.Add(interactionId);
                            ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(result.IMessage);
                            starttimerforerror("The interaction opened successfully.");
                            if (_isInteractionQueue)
                                NotifyEmailOpenedToIQ(interactionId);
                        }
                    }
                    else
                        starttimerforerror("The operation failed whileopen interaction.");
                }
            }
        }

        private string GetInteractionStatus(string interactionId)
        {
            Pointel.Interactions.Core.InteractionService interactionServer = new Interactions.Core.InteractionService();
            Pointel.Interactions.Core.Common.OutputValues interactionResult = interactionServer.GetInteractionProperties(WorkbinUtility.Instance().IxnProxyID, interactionId);
            string state = string.Empty;
            if ("200".Equals(interactionResult.MessageCode))
            {
                //RowVoiceState.Height = RowChatState.Height = RowState.Height = GridLength.Auto;
                EventInteractionProperties eventinteractionProperties = interactionResult.IMessage as EventInteractionProperties;
                if (eventinteractionProperties != null)
                {
                    _isInteractionActive = false;
                    switch (eventinteractionProperties.Interaction.InteractionState)
                    {
                        case InteractionState.Cached:
                            state = "In a Queue.";
                            break;
                        case InteractionState.Routing:
                            state = "Delivery in progress.";
                            break;
                        case InteractionState.Queued:
                            if (string.IsNullOrEmpty(eventinteractionProperties.Interaction.InteractionWorkbinTypeId))
                                state = "In a Queue.";
                            else
                                state = "Assigned to " + GetFullNameOfAgent(eventinteractionProperties.Interaction.InteractionAssignedTo) + " - " + eventinteractionProperties.Interaction.InteractionWorkbinTypeId;
                            break;
                        case InteractionState.Handling:
                            state = "Assigned to " + GetFullNameOfAgent(eventinteractionProperties.Interaction.InteractionAssignedTo) + " - Actively handling.";
                            _isInteractionActive = true;
                            break;
                    }

                }
            }
            else
            {
                // TODO : Need to handle the emailinteraction Properties.

                // RowState.Height = RowChatState.Height = RowVoiceState.Height = new GridLength(0);
            }

            return state;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = btnOpenMail.Visibility == Visibility.Visible;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnOpenMail_Click(null, null);
        }

        private void ParseAndStoreMail(EventInteractionProperties interactionProperties, string workbinName, string interactionId, string agentId)
        {
            WorkbinMailDetails mailDetails = new WorkbinMailDetails();
            mailDetails.InteractionId = interactionId;
            mailDetails.AgentId = agentId;
            mailDetails.WorkbinName = workbinName;
            if (interactionProperties != null && interactionProperties.Interaction != null)
            {
                if (interactionProperties.Interaction.InteractionUserData != null)
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name"))
                        mailDetails.DispositionKey = interactionProperties.Interaction.InteractionUserData[ConfigContainer.Instance().GetValue("interaction.disposition.key-name")] as string;
                    if (interactionProperties.Interaction.InteractionUserData.ContainsKey("FromAddress"))
                        mailDetails.From = interactionProperties.Interaction.InteractionUserData["FromAddress"].ToString();
                    if (interactionProperties.Interaction.InteractionUserData.ContainsKey("Subject"))
                        mailDetails.Subject = interactionProperties.Interaction.InteractionUserData["Subject"].ToString();
                    if (interactionProperties.Interaction.InteractionUserData.ContainsKey("To"))
                        mailDetails.To = interactionProperties.Interaction.InteractionUserData["To"].ToString();
                    if (interactionProperties.Interaction.InteractionUserData.ContainsKey("Cc"))
                        mailDetails.CC = interactionProperties.Interaction.InteractionUserData["Cc"].ToString();
                    if (interactionProperties.Interaction.InteractionUserData.ContainsKey("Bcc"))
                        mailDetails.BCC = interactionProperties.Interaction.InteractionUserData["Bcc"].ToString();
                    string workbinFields = "";
                    if (isPersonalWorkbinSelected)
                        workbinFields = WorkbinUtility.Instance().PersonalWorkbinList[workbinName];
                    else
                        workbinFields = WorkbinUtility.Instance().TeamWorkbinList[workbinName];

                    foreach (string fields in workbinFields.Split(','))
                        if (interactionProperties.Interaction.InteractionUserData.ContainsKey(fields))
                            mailDetails.DisplayValue.Add(interactionProperties.Interaction.InteractionUserData[fields].ToString());
                        else if (fields.ToLower().Equals("from") && interactionProperties.Interaction.InteractionUserData.ContainsKey("FromAddress"))
                            mailDetails.DisplayValue.Add(interactionProperties.Interaction.InteractionUserData["FromAddress"].ToString());
                        else if (fields.ToLower().Equals("received"))
                            mailDetails.DisplayValue.Add(interactionProperties.Interaction.InteractionReceivedAt.ToString());
                        else if (fields.ToLower().Equals("submitted"))
                            mailDetails.DisplayValue.Add(interactionProperties.Interaction.InteractionSubmittedAt.ToString());

                }
                if (interactionProperties.Interaction.InteractionReceivedAt != null)
                    mailDetails.ReceivedDate = interactionProperties.Interaction.InteractionReceivedAt.ToString();
                if (interactionProperties.Interaction.InteractionSubmittedAt != null)
                    mailDetails.StartDate = interactionProperties.Interaction.InteractionSubmittedAt.ToString();

                if (interactionProperties.Interaction.Contains("InteractionType"))
                    mailDetails.TypeId = interactionProperties.Interaction["InteractionType"].ToString();
                string AgentName = "unknown";
                if (!string.IsNullOrEmpty(interactionProperties.Interaction.InteractionAssignedTo))
                    AgentName = GetFullNameOfAgent(interactionProperties.Interaction.InteractionAssignedTo);
                if (string.IsNullOrEmpty(AgentName))
                    AgentName = "unknown";
                mailDetails.State = "Assigned to " + AgentName + " - " + workbinName + " workbin";
                mailDetails.ParentInteractionId = interactionProperties.Interaction.InteractionParentInteractionId;
                List<string> CaseDataFilter = null;
                //Case data with filtering
                //Case data with filtering
                if (ConfigContainer.Instance().AllKeys.Contains("email.enable.case-data-filter")
                    && ((string)ConfigContainer.Instance().GetValue("email.enable.case-data-filter")).ToLower().Equals("true")
                     && ((ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataFilterKey")
                        && ConfigContainer.Instance().GetValue("EmailAttachDataFilterKey") != null)
                        || (ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataKey")
                        && ((List<string>)ConfigContainer.Instance().GetValue("EmailAttachDataKey")) != null)))
                {
                    CaseDataFilter = ((ConfigContainer.Instance().GetValue("EmailAttachDataFilterKey")) as List<string>).ToList();
                    if (CaseDataFilter != null)
                        CaseDataFilter.AddRange((List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey")));
                    else
                        CaseDataFilter = (List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey"));
                    if (CaseDataFilter != null)
                        CaseDataFilter = CaseDataFilter.Distinct().ToList();
                }
                else
                {
                    CaseDataFilter = interactionProperties.Interaction.InteractionUserData.AllKeys.ToList();
                }
                if (CaseDataFilter != null && CaseDataFilter.Count > 0)
                {
                    foreach (string keyName in CaseDataFilter)
                    {
                        if (interactionProperties.Interaction.InteractionUserData.AllKeys.Contains(keyName))
                            mailDetails.CaseData.Add(new EmailCaseData(keyName, interactionProperties.Interaction.InteractionUserData[keyName].ToString()));
                    }
                }
            }
            mailDetails.MailImage = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\" +
                (mailDetails.TypeId.Equals("Inbound") ? "Inbound1.png" : "Outbound2.png"), UriKind.Relative));

            //Added for remove duplicate interactions, date: 23/feb/2016, fixer : rajkumar
            //start
            if (workbinData.WorkbinDetails.Any(x => x.InteractionId == mailDetails.InteractionId))
                workbinData.WorkbinDetails.Remove(workbinData.WorkbinDetails.Where(x => x.InteractionId == mailDetails.InteractionId).FirstOrDefault());
            //end

            workbinData.WorkbinDetails.Add(mailDetails);
        }

        private void ParseAndStoreMail(EventGetInteractionContent interactionContent, ref WorkbinMailDetails mailDetails)
        {
            if (interactionContent.InteractionAttributes != null)
            {
                mailDetails.TypeId = interactionContent.InteractionAttributes.TypeId;
                mailDetails.SubTypeId = interactionContent.InteractionAttributes.SubtypeId;
                mailDetails.Subject = interactionContent.InteractionAttributes.Subject;
                mailDetails.ThreadId = interactionContent.InteractionAttributes.ThreadId;


                if (interactionContent.InteractionAttributes != null)
                {
                    mailDetails.EmailNotes = interactionContent.InteractionAttributes.TheComment;

                    if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name"))
                    {
                        string dispositionKeyName = ConfigContainer.Instance().GetAsString("interaction.disposition.key-name");
                        if (!string.IsNullOrEmpty(dispositionKeyName))
                        {
                            if (interactionContent.InteractionAttributes.AllAttributes.ContainsKey(dispositionKeyName))
                                mailDetails.DispositionKey = interactionContent.InteractionAttributes.AllAttributes.GetAsString(dispositionKeyName);
                        }
                        else
                            logger.Warn("The disposition key is empty.");

                    }

                }
            }
            if (!String.IsNullOrEmpty(interactionContent.InteractionContent.StructuredText))
                mailDetails.MailBody = interactionContent.InteractionContent.StructuredText;
            else
            {
                mailDetails.MailBody = interactionContent.InteractionContent.Text;
                if (!string.IsNullOrEmpty(mailDetails.MailBody))
                {
                    mailDetails.MailBody = mailDetails.MailBody.Replace("\r\n", "<br />");
                    mailDetails.MailBody = mailDetails.MailBody.Replace("\n", "<br />");
                }
            }
            mailDetails.Attachment.Clear();
            if (interactionContent.Attachments != null && interactionContent.Attachments.Count > 0)
                foreach (Attachment attachment in interactionContent.Attachments)
                {
                    mailDetails.Attachment.Add(new AttachmentDetails() { DocumentId = attachment.DocumentId, FileName = attachment.TheName, Size = attachment.TheSize.Value.ToString() });
                }
            mailDetails.EmailNotes = interactionContent.InteractionAttributes.TheComment;
            //Last property read copy: 21-04-2015

            if (mailDetails.TypeId.Equals("Inbound"))
            {
                mailDetails.MailImage = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Inbound1.png", UriKind.Relative));
                EmailInEntityAttributes entityAttribute = interactionContent.EntityAttributes as EmailInEntityAttributes;
                if (entityAttribute != null)
                {
                    mailDetails.From = entityAttribute.FromAddress;
                    mailDetails.To = entityAttribute.ToAddresses;
                    mailDetails.CC = entityAttribute.CcAddresses;
                    mailDetails.BCC = entityAttribute.BccAddresses;
                    mailDetails.ReceivedDate = (interactionContent.InteractionAttributes.StartDate != null) ? interactionContent.InteractionAttributes.StartDate.Value.ToString() : null;
                }
            }
            else
            {
                mailDetails.MailImage = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Email\\Outbound2.png", UriKind.Relative));
                EmailOutEntityAttributes entityAttribute = interactionContent.EntityAttributes as EmailOutEntityAttributes;
                if (entityAttribute != null)
                {
                    mailDetails.From = entityAttribute.FromAddress;
                    mailDetails.To = entityAttribute.ToAddresses;
                    mailDetails.CC = entityAttribute.CcAddresses;
                    mailDetails.BCC = entityAttribute.BccAddresses;
                    mailDetails.ReceivedDate = (interactionContent.InteractionAttributes.StartDate != null) ? interactionContent.InteractionAttributes.StartDate.Value.ToString() : null;
                }
            }
        }

        private void PerformLoadMail()
        {
            workbinData.WorkbinDetails = new System.Collections.ObjectModel.ObservableCollection<WorkbinMailDetails>();
            workbinData.SelectedWorkbinDetails = new List<WorkbinMailDetails>();
            //  LoadMailImages();
            LoadPersonalWorkbin();
            txtUnavailableError.Visibility = Visibility.Collapsed;
            gridCompleteContainer.Visibility = Visibility.Visible;
        }

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

        private void RetriveAndStore(string interactionId, string workbinName, string agentId)
        {
            if (string.Compare(interactionId, string.Empty) == 0)
                return;
            InteractionService service = new InteractionService();
            Pointel.Interactions.Core.Common.OutputValues result = service.GetInteractionProperties(WorkbinUtility.Instance().IxnProxyID, interactionId);
            if (result.MessageCode == "200")
            {
                EventInteractionProperties interactionProperties = result.IMessage as EventInteractionProperties;
                if (interactionProperties != null)
                {
                    ParseAndStoreMail(interactionProperties, workbinName, interactionId, agentId);
                }
            }
        }

        private void SelectAll(TextBox tb)
        {
            Keyboard.Focus(tb);
            tb.SelectAll();
        }

        private void SetPersonalWorkbinBackground()
        {
            BrushConverter brushConverter = new BrushConverter();
            btnMyWorkbin.Background = (Brush)brushConverter.ConvertFrom("#A7BFD6");
            btnMyTeamWorkbin.Background = (Brush)brushConverter.ConvertFrom("#D9EBFC");
        }

        private void ShowLoading()
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Visible;
                ImgPreload.Visibility = System.Windows.Visibility.Visible;
                Panel.SetZIndex(BrdOpacity, 1);
                Panel.SetZIndex(ImgPreload, 2);

            }));
        }

        private void ShowMainMenu()
        {
            txtNoItem.Visibility = Visibility.Collapsed;
            StandardEmailImage.Visibility = Middle.Visibility = Visibility.Visible;
        }

        private void ShowMessage(string message)
        {
            PopUpWindow popupWindow = new PopUpWindow();
            popupWindow.Message = message;
            popupWindow.btnOK.Content = "OK";
            popupWindow.btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            popupWindow.ShowDialog();
        }

        private void ShowMandatoryMessage()
        {
            ShowMessage("Disposition code is mandatory.");

            WorkbinMailDetails mailDetails = null;
            if (_isInteractionQueue)
                mailDetails = tbCallBack.DataContext as WorkbinMailDetails;
            else
                mailDetails = dgWorkbin.SelectedItem as WorkbinMailDetails;

            if (mailDetails != null)
            {
                OpenMailInWindow(mailDetails.InteractionId);
            }
        }

        void ShowTeamCommunicator(System.Windows.Controls.UserControl teamCommunicatorWorkbin, UIElement uiElement)
        {
            if (teamCommunicatorWorkbin != null)
            {
                WorkbinUtility.Instance().contextMenuTransfer = new ContextMenu();
                WorkbinUtility.Instance().contextMenuTransfer.Style = (Style)FindResource("ContextMenu");
                var parent = FindAncestor<Grid>(teamCommunicatorWorkbin);
                if (parent != null)
                    parent.Children.Clear();

                Grid grid1 = new Grid();
                grid1.Background = Brushes.White;
                grid1.Children.Add(teamCommunicatorWorkbin);
                var menuConsultItem = new System.Windows.Controls.MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = Brushes.Transparent;
                menuConsultItem.Header = grid1;
                menuConsultItem.Width = teamCommunicatorWorkbin.Width;
                menuConsultItem.Height = teamCommunicatorWorkbin.Height;
                menuConsultItem.Margin = new Thickness(-13, -6, -18, -5);
                WorkbinUtility.Instance().contextMenuTransfer.Items.Clear();
                WorkbinUtility.Instance().contextMenuTransfer.Items.Add(menuConsultItem);
                WorkbinUtility.Instance().contextMenuTransfer.Placement = PlacementMode.Bottom;
                WorkbinUtility.Instance().contextMenuTransfer.PlacementTarget = uiElement;
                WorkbinUtility.Instance().contextMenuTransfer.IsOpen = true;
                WorkbinUtility.Instance().contextMenuTransfer.StaysOpen = true;
                WorkbinUtility.Instance().contextMenuTransfer.Focus();
            }
        }

        private void starttimerforerror(string message)
        {
            try
            {
                if (_timerforcloseError != null)
                    _timerforcloseError.Stop();

                _timerforcloseError = new DispatcherTimer();
                grMessage.Height = GridLength.Auto;
                txtInteractionMessage.Text = message;
                _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                _timerforcloseError.Tick += new EventHandler(CloseError);
                _timerforcloseError.Start();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }
        }

        private void tbCallBack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private string TeamCommunicatorEventNotify(Dictionary<string, string> dictionaryValues)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                if (WorkbinUtility.Instance().contextMenuTransfer.IsOpen)
                    WorkbinUtility.Instance().contextMenuTransfer.IsOpen = false;
                if (WorkbinUtility.Instance().contextMenuTransfer.StaysOpen)
                    WorkbinUtility.Instance().contextMenuTransfer.StaysOpen = false;
            }));
            if (dictionaryValues != null && dictionaryValues.Count > 0)
            {
                if (dictionaryValues.ContainsKey("SearchedType"))
                {
                    if (dictionaryValues["SearchedType"].ToString().Equals("InteractionQueue"))
                    {
                        MoveMailIntoQueue(dictionaryValues);

                    }
                    else if (dictionaryValues["SearchedType"].ToString().Equals("Agent"))
                    {
                        MoveMailIntoAgentWorkbin(dictionaryValues);
                    }
                }
            }
            return string.Empty;
        }

        private void tvWorkbin_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    _isInteractionQueue = false;
                    dkpState.Visibility = Visibility.Collapsed;
                    gridDetails.Visibility = Visibility.Visible;
                    gridOtherMediaDetails.Visibility = Visibility.Collapsed;

                    workbinData.IQVisiblity = Visibility.Collapsed;
                    gridWorkbin.Visibility = Visibility.Visible;
                    ShowLoading();
                    WorkbinMenuItem item = tvWorkbin.SelectedItem as WorkbinMenuItem;
                    tbCallBack.DataContext = null;
                    if (item != null && item.Visible != Visibility.Collapsed)
                    {
                        WorkbinUtility.Instance().SelectedWorkbinName = item.Title;
                        if (item.AgentId.Equals(ConfigContainer.Instance().AgentId))
                            ShowCurrentWorkbin.Header = item.Title;
                        else
                        {
                            foreach (WorkbinMenuItem workbinMenu in workbinData.MyTeamWorkbinMenu[0].Items)
                            {
                                if (workbinMenu.Items.Where(x => x.AgentId.Equals(item.AgentId)).ToList().Count > 0)
                                {
                                    WorkbinMenuItem agentMenu = workbinMenu.Items.Where(x => x.AgentId.Equals(item.AgentId)).SingleOrDefault();
                                    ShowCurrentWorkbin.Header = agentMenu.Title + " - " + item.Title;
                                    break;
                                }
                            }
                        }
                        workbinData.SelectedWorkbinDetails = workbinData.WorkbinDetails.Where(x => x.WorkbinName.Equals(item.Title)
                                                              && x.AgentId.Equals(item.AgentId)).OrderBy(x => x.ReceivedDate).Reverse().ToList();
                    }
                    DetermineButtonVisibility();
                    HideLoading();
                    SetWidthandHeightforIQ();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occcurred as " + ex.Message);
                }
            }));
        }


        private void tvInteractionQueue_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    TakeNewSnapShot();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occcurred as " + ex.Message);
                }

            }));
        }

        private void TakeNewSnapShot()
        {
            IQMenuItem iqMenuItem = tvInteractionQueue.SelectedItem as IQMenuItem;
            if (!string.IsNullOrEmpty(iqMenuItem.Condition))
            {
                _selectedIQCondition = iqMenuItem;
                _isInteractionQueue = true;
                dkpState.Visibility = Visibility.Visible;

                ShowLoading();
                ShowCurrentWorkbin.Header = iqMenuItem.DisplayName;


                gridWorkbin.Visibility = Visibility.Collapsed;


                //Release the snapshot if already exists
                if (iqMenuItem.SnapShotID != 0)
                    ReleaseSnapShotID(iqMenuItem.SnapShotID);

                iqMenuItem.SnapShotID = GetSnapshotID(iqMenuItem.Condition);
                iqSnapshotID = iqMenuItem.SnapShotID;

                if (iqMenuItem.SnapShotID != 0 && totalIQInteractions > 0)
                {
                    ShowMainMenu();
                    workbinData.IQVisiblity = Visibility.Visible;
                    SnapShotContent.Text = totalIQInteractions + " interactions in snapshot, taken at " + DateTime.Now.ToString("HH:mm:ss");
                    startIndex = 1;
                    endIndex = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    if (endIndex > totalIQInteractions)
                        endIndex = totalIQInteractions;


                    int numberOfInteractions = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    if (numberOfInteractions > totalIQInteractions)
                        numberOfInteractions = totalIQInteractions;

                    EventSnapshotInteractions eventSnapshotInteractions;
                    GetSnapshotInteractions(iqMenuItem.SnapShotID, 0, numberOfInteractions, out eventSnapshotInteractions);

                    if (eventSnapshotInteractions != null && eventSnapshotInteractions.Interactions != null &&
                        eventSnapshotInteractions.Interactions.Count > 0)
                    {
                        dicKVPIQInteractions = new Dictionary<string, KeyValueCollection>();

                        AddColoumnstoIQDatatable(iqMenuItem.DisplayedColoumns);
                        FillInteractionsinDatatable(eventSnapshotInteractions);
                        SetPaging();
                        dgIQ.DataContext = dtIQInteractions;
                        dgIQ.SelectedIndex = 0;

                        if (ExpandMailHeading.Text == "Hide Details")
                        {
                            if (_isInteractionQueue)
                            {
                                dgIQ.Height = 150;
                                rowdefHistory.Height = new GridLength(165 + 26);
                            }
                            else
                            {
                                dgWorkbin.Height = 150;
                                rowdefHistory.Height = new GridLength(160);
                            }
                        }
                        //SetWidthandHeightforIQ();
                        //SetWidthandHeightforIQ();
                    }
                    else
                    {
                        dtIQInteractions = null;
                        HideIQ();
                    }
                }
                else
                {
                    dtIQInteractions = null;
                    HideIQ();
                }
                HideLoading();
            }
            else
                return;
        }
        private void SetWidthandHeightforIQ()
        {
            if (brdIQ.ActualWidth > 0)
                dgIQ.Width = brdIQ.ActualWidth - 10;
            if (ExpandMailHeading.Text == "Hide Details")
            {
                if (_isInteractionQueue)
                {
                    dgIQ.Height = 150;
                    rowdefHistory.Height = new GridLength(165 + brdPaging.ActualHeight);
                }
                else
                {
                    dgWorkbin.Height = 150;
                    rowdefHistory.Height = new GridLength(160);
                }
            }
        }
        private void cmbItemsPerPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IQMenuItem iqMenuItem = tvInteractionQueue.SelectedItem as IQMenuItem;
                startIndex = 1;
                endIndex = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                if (endIndex > totalIQInteractions)
                    endIndex = totalIQInteractions;


                int numberOfInteractions = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                if (numberOfInteractions > totalIQInteractions)
                    numberOfInteractions = totalIQInteractions;

                EventSnapshotInteractions eventSnapshotInteractions;
                GetSnapshotInteractions(iqMenuItem.SnapShotID, 0, numberOfInteractions, out eventSnapshotInteractions);

                if (eventSnapshotInteractions != null && eventSnapshotInteractions.Interactions != null &&
                    eventSnapshotInteractions.Interactions.Count > 0)
                {
                    dicKVPIQInteractions = new Dictionary<string, KeyValueCollection>();
                    AddColoumnstoIQDatatable(iqMenuItem.DisplayedColoumns);
                    FillInteractionsinDatatable(eventSnapshotInteractions);
                    SetPaging();
                    dgIQ.DataContext = dtIQInteractions;
                    dgIQ.SelectedIndex = 0;
                    SetWidthandHeightforIQ();
                }
                else
                {
                    HideIQ();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }
        private void FillInteractionsinDatatable(EventSnapshotInteractions eventSnapshotInteractions)
        {
            try
            {
                //Change Coloumn name before adding
                if (dtIQInteractions.Columns.Contains("From"))
                    dtIQInteractions.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "From").SingleOrDefault().ColumnName = "FromAddress";

                if (dtIQInteractions.Columns.Contains("Received"))
                    dtIQInteractions.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "Received").SingleOrDefault().ColumnName = "ReceivedAt";

                foreach (string interactionId in eventSnapshotInteractions.Interactions.AllKeys)
                {
                    KeyValueCollection kvpInteractionProperties = eventSnapshotInteractions.Interactions[interactionId] as KeyValueCollection;
                    dicKVPIQInteractions.Add(interactionId, kvpInteractionProperties);
                    DataRow iqRow = dtIQInteractions.NewRow();
                    for (int columnIndex = 0; columnIndex < dtIQInteractions.Columns.Count; columnIndex++)
                    {
                        if (kvpInteractionProperties.AllKeys.Contains(dtIQInteractions.Columns[columnIndex].ColumnName))
                        {
                            string value = kvpInteractionProperties[dtIQInteractions.Columns[columnIndex].ColumnName].ToString();
                            DateTime dateTimeIQ;
                            if (DateTime.TryParse(value, out dateTimeIQ))
                                value = GetLocalDateTime(value);
                            iqRow[dtIQInteractions.Columns[columnIndex].ColumnName] = value;
                        }
                    }
                    dtIQInteractions.Rows.Add(iqRow);
                }

                if (dtIQInteractions.Columns.Contains("FromAddress"))
                    dtIQInteractions.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "FromAddress").SingleOrDefault().ColumnName = "From";

                if (dtIQInteractions.Columns.Contains("ReceivedAt"))
                    dtIQInteractions.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "ReceivedAt").SingleOrDefault().ColumnName = "Received";

            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }

        }

        private void SetPaging()
        {
            btnFirstPage.IsEnabled = startIndex > 1;
            btnPreviousPage.IsEnabled = startIndex > 1;
            txtStart.Text = "" + startIndex;
            txtEnd.Text = "" + endIndex;
            if (endIndex >= totalIQInteractions)
                btnNextPage.IsEnabled = false;
            else
                btnNextPage.IsEnabled = true;
        }

        private void HideIQ()
        {
            txtNoItem.Visibility = Visibility.Visible;
            StandardEmailImage.Visibility = Middle.Visibility = Visibility.Collapsed;
            gridDetails.Visibility = dockCaseData.Visibility = Visibility.Collapsed;
        }

        private void AddColoumnstoIQDatatable(string displayedColoumns)
        {
            dtIQInteractions = new DataTable();
            dtIQInteractions.Columns.Clear();
            List<string> lstAttribute;
            if (!string.IsNullOrEmpty(displayedColoumns))
            {
                lstAttribute = displayedColoumns.Split(',').ToList();
                iqDisplayedColoumns = displayedColoumns;
            }
            else
            {
                lstAttribute = (ConfigContainer.Instance().AllKeys.Contains("interaction-management.interactions-filter.displayed-columns") ?
                                  iqDisplayedColoumns = (string)ConfigContainer.Instance().GetValue("interaction-management.interactions-filter.displayed-columns") :
                                    iqDisplayedColoumns = "From,To,Subject,Received").Split(',').ToList();

            }
            lstAttribute.Add("MediaType");
            lstAttribute.Add("InteractionType");
            lstAttribute.Add("InteractionSubtype");
            lstAttribute.Add("InteractionId");
            lstAttribute = lstAttribute.Distinct().ToList();
            for (int index = 0; index < lstAttribute.Count; index++)
                dtIQInteractions.Columns.Add(lstAttribute[index]);
        }

        public void CreateMediaColumnWithImages()
        {
            Microsoft.Windows.Controls.DataGridTemplateColumn statusImageColumn = new Microsoft.Windows.Controls.DataGridTemplateColumn { CanUserReorder = false, Width = 30, CanUserSort = false }; ;
            statusImageColumn.Header = "";
            statusImageColumn.CellTemplateSelector = new MediaImageDataTemplateSelector();
            statusImageColumn.MinWidth = 30;
            if (!isAddedMediaIcon)
            {
                dgIQ.Columns.Insert(0, statusImageColumn);
                isAddedMediaIcon = true;
            }
            else
            {
                dgIQ.Columns.RemoveAt(0);
                dgIQ.Columns.Insert(0, statusImageColumn);
            }
        }

        private void ReleaseSnapShotID(int snapshotID)
        {
            try
            {
                InteractionService objInteractionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = objInteractionService.ReleaseSnapshotID(WorkbinUtility.Instance().IxnProxyID, snapshotID);
                if (result.MessageCode.Equals("200"))
                {
                    logger.Info("Snapshot ID Released successfully" + snapshotID);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private int GetSnapshotID(string condition)
        {
            int snapshotID = 0;
            totalIQInteractions = 0;
            try
            {
                InteractionService objInteractionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = objInteractionService.TakeSnapshotID(WorkbinUtility.Instance().IxnProxyID, condition);
                if (result.MessageCode.Equals("200") && result.IMessage.Id == EventSnapshotTaken.MessageId)
                {
                    EventSnapshotTaken eventSnapshotTaken = (EventSnapshotTaken)result.IMessage;
                    snapshotID = eventSnapshotTaken.SnapshotId;
                    totalIQInteractions = eventSnapshotTaken.NumberOfInteractions;
                    logger.Info("Snapshot ID takened successfully" + snapshotID);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
            return snapshotID;
        }

        private void GetSnapshotInteractions(int snapshotID, int start, int numberOfInteractions, out EventSnapshotInteractions eventSnapshotInteractions)
        {
            eventSnapshotInteractions = null;
            try
            {
                InteractionService objInteractionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = objInteractionService.GetSnapshotInteractionsbyID(WorkbinUtility.Instance().IxnProxyID,
                    snapshotID, start, numberOfInteractions);
                if (result.MessageCode.Equals("200") && result.IMessage.Id == EventSnapshotInteractions.MessageId)
                {
                    eventSnapshotInteractions = (EventSnapshotInteractions)result.IMessage;
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private void btnSnapShot_Click(object sender, RoutedEventArgs e)
        {
            TakeNewSnapShot();
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = 1;
            endIndex = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
            SetPaging();
            FilterDatatable();
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startIndex = int.Parse(txtStart.Text) - int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                endIndex = int.Parse(txtStart.Text) - 1;
                SetPaging();
                FilterDatatable();
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startIndex = int.Parse(txtEnd.Text) + 1;
                endIndex = int.Parse(txtEnd.Text) + int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                if (endIndex > totalIQInteractions)
                    endIndex = totalIQInteractions;

                if (dtIQInteractions.Rows.Count < endIndex)
                {
                    int numberOfInteractions = int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    if (numberOfInteractions > totalIQInteractions)
                        numberOfInteractions = totalIQInteractions;

                    EventSnapshotInteractions eventSnapshotInteractions;
                    GetSnapshotInteractions(iqSnapshotID, int.Parse(txtEnd.Text), numberOfInteractions, out eventSnapshotInteractions);

                    if (eventSnapshotInteractions != null && eventSnapshotInteractions.Interactions != null &&
                        eventSnapshotInteractions.Interactions.Count > 0)
                    {
                        FillInteractionsinDatatable(eventSnapshotInteractions);
                    }
                }

                SetPaging();
                FilterDatatable();
            }
            catch (Exception ex)
            {
                logger.Error("Error occcurred as " + ex.Message);
            }
        }
        private void FilterDatatable()
        {
            DataTable temp = dtIQInteractions.Clone();
            for (int index = startIndex - 1; index < endIndex; index++)
            {
                temp.ImportRow(dtIQInteractions.Rows[index]);
            }
            temp = temp.DefaultView.ToTable();
            dgIQ.DataContext = temp;
            if (temp.Rows.Count > 0)
                dgIQ.SelectedIndex = 0;
            SetWidthandHeightforIQ();
        }
        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgIQ_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dgIQ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgIQ.SelectedIndex >= 0)
            {
                var selectedIQ = dgIQ.SelectedItem as DataRowView;
                if (selectedIQ != null)
                {
                    string interationId = selectedIQ["InteractionId"].ToString();
                    if (dicKVPIQInteractions.Keys.Contains(interationId))
                    {
                        KeyValueCollection kvInteractionDetails = dicKVPIQInteractions[interationId];
                        txtNoDetails.Visibility = Visibility.Collapsed;
                        switch (selectedIQ["MediaType"].ToString())
                        {
                            case "email": ShowEmailInteractionDetail(kvInteractionDetails, interationId); break;
                            case "chat": ShowChatInteractionDetail(kvInteractionDetails, interationId); break;
                            case "voice": ShowVoiceInteractionDetail(kvInteractionDetails, interationId); break;
                            default: ShowOtherInteractionDetail(kvInteractionDetails, interationId); break;
                        }
                    }
                    else
                        logger.Warn("Interaction detail not found in the interaction dictionary.");

                }
                else
                    logger.Warn("Selected item is null.");


            }
            else
                logger.Warn("Item not selected in grid.");
        }

        private void HideEmailButton()
        {
            gcMarkdone.Width = gcReplyAll.Width = gcPrint.Width = gcOpenMail.Width = gcReply.Width = gcDelete.Width = new GridLength(0);
        }

        private void ShowEmailInteractionDetail(KeyValueCollection kvEmailDetail, string interactionId)
        {
            gridDetails.Visibility = Visibility.Visible;
            gridOtherMediaDetails.Visibility = Visibility.Collapsed;

            if (kvEmailDetail != null)
            {
                WorkbinMailDetails mailDetails = new WorkbinMailDetails();
                mailDetails.InteractionId = interactionId;

                if (kvEmailDetail.ContainsKey("Subject"))
                    mailDetails.Subject = kvEmailDetail.GetAsString("Subject");
                var selectedIQ = dgIQ.SelectedItem as DataRowView;

                if (selectedIQ["InteractionType"].ToString().Equals("Inbound"))
                {
                    if (kvEmailDetail.ContainsKey("ReceivedAt"))
                        mailDetails.ReceivedDate = kvEmailDetail.GetAsString("ReceivedAt");
                }
                else
                {
                    if (kvEmailDetail.ContainsKey("StartDate"))
                        mailDetails.ReceivedDate = kvEmailDetail.GetAsString("StartDate");
                }

                AssignStateAndCaseData(kvEmailDetail, "email", ref mailDetails);

                if (dgIQ.SelectedItems != null && dgIQ.SelectedItems.Count > 1)
                {
                    isShowReply = isShowReplyAll = isShowOpen = false;

                    isShowMarkDone = (dgIQ.SelectedItems.Cast<DataRowView>().Where(x => x["InteractionType"].ToString().ToLower().Equals("inbound")
                        && x["MediaType"].ToString().ToLower().Equals("email")).ToList().Count == dgIQ.SelectedItems.Count);

                    isShowDelete = (dgIQ.SelectedItems.Cast<DataRowView>().Where(x => x["InteractionType"].ToString().ToLower().Equals("outbound")
                        && x["MediaType"].ToString().ToLower().Equals("email")).ToList().Count == dgIQ.SelectedItems.Count);

                }
                else
                {

                    if (selectedIQ != null)
                    {
                        if (!_isActivelyHandling)
                        {
                            if (selectedIQ["InteractionType"].ToString().Equals("Inbound"))
                            {
                                isShowDelete = false;
                                isShowMarkDone = isShowReply = isShowOpen = true;
                            }
                            else
                            {
                                isShowMarkDone = isShowReply = isShowReplyAll = false;
                                isShowOpen = isShowDelete = true;
                            }
                        }
                    }
                }

                BindSelectedMailDetails(mailDetails);

            }
        }

        private void ShowChatInteractionDetail(KeyValueCollection kvChatDetail, string interactionId)
        {
            HideEmailButton();
            gridDetails.Visibility = Visibility.Collapsed;
            gridOtherMediaDetails.Visibility = Visibility.Visible;
            if (ExpandMailHeading.Text == "Hide Details")
                rowdefDetails.MinHeight = gridOtherMediaDetails.ActualHeight;
            WorkbinMailDetails chatDetails = new WorkbinMailDetails();
            chatDetails.InteractionId = interactionId;
            if (kvChatDetail != null)
            {
                AssignStateAndCaseData(kvChatDetail, "chat", ref chatDetails);
                tbCallBack.DataContext = chatDetails;
            }
            gcWorkbin.Width = (_isActivelyHandling) ? new GridLength(0) : GridLength.Auto;
        }

        private void ShowVoiceInteractionDetail(KeyValueCollection kvVoiceDetail, string interactionId)
        {
            HideEmailButton();
            gridDetails.Visibility = Visibility.Collapsed;
            gridOtherMediaDetails.Visibility = Visibility.Visible;
            WorkbinMailDetails voiceDetails = new WorkbinMailDetails();
            voiceDetails.InteractionId = interactionId;
            if (kvVoiceDetail != null)
            {
                AssignStateAndCaseData(kvVoiceDetail, "voice", ref voiceDetails);
                tbCallBack.DataContext = voiceDetails;
            }
            gcWorkbin.Width = (_isActivelyHandling) ? new GridLength(0) : GridLength.Auto;
        }

        private void ShowOtherInteractionDetail(KeyValueCollection kvOtherDetail, string interactionId)
        {
            HideEmailButton();
            gridDetails.Visibility = Visibility.Collapsed;
            gridOtherMediaDetails.Visibility = Visibility.Visible;
            WorkbinMailDetails otherDetails = new WorkbinMailDetails();
            otherDetails.InteractionId = interactionId;
            if (kvOtherDetail != null)
            {
                AssignStateAndCaseData(kvOtherDetail, "other", ref otherDetails);
                tbCallBack.DataContext = otherDetails;
            }
            gcWorkbin.Width = (_isActivelyHandling) ? new GridLength(0) : GridLength.Auto;
        }

        private void AssignStateAndCaseData(KeyValueCollection kvEmailDetail, string media, ref WorkbinMailDetails details)
        {
            IQMenuItem iqMenuItem = tvInteractionQueue.SelectedItem as IQMenuItem;
            if (iqMenuItem != null && iqMenuItem.CaseDataToFilter != null)
            {
                details.CaseData = new List<EmailCaseData>();
                foreach (string keyName in iqMenuItem.CaseDataToFilter)
                    if (kvEmailDetail.ContainsKey(keyName) && !string.IsNullOrEmpty(kvEmailDetail.GetAsString(keyName)))
                        details.CaseData.Add(new EmailCaseData(keyName, kvEmailDetail.GetAsString(keyName)));
            }
            else
            {
                // TODO : Need to Filter case data if it is not configured.
            }
            //if (details.CaseData == null || details.CaseData.Count == 0)
            //{
            //    dgUserData.Visibility = Visibility.Collapsed;
            //    txtNoCaseData.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    dgUserData.Visibility = Visibility.Visible;
            //    txtNoCaseData.Visibility = Visibility.Collapsed;
            //}

            if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name")
                && kvEmailDetail.ContainsKey(ConfigContainer.Instance().GetAsString("interaction.disposition.key-name")))
                details.DispositionKey = kvEmailDetail.GetAsString(ConfigContainer.Instance().GetAsString("interaction.disposition.key-name"));

            details.InterationState = GetInteractionStatus(details.InteractionId);

            _isActivelyHandling = (details.InterationState != null && details.InterationState.Contains("- Actively handling."));

        }

        private void IQRemoveNotification()
        {
            TakeNewSnapShot();

        }

        void InteractionClosedEvent(string interactionId)
        {
            if (_isInteractionQueue && dgIQ.SelectedItem != null)
            {
                DataRowView row = dgIQ.SelectedItem as DataRowView;
                if (row != null && row["InteractionId"].ToString() == interactionId)
                    dgIQ_SelectionChanged(null, null);
            }

            //if (lstOpenedIQInteraction.Contains(interactionId))
            //{
            //    lstOpenedIQInteraction.Remove(interactionId);

            //    if (dgIQ.SelectedItem != null)
            //        dgIQ_SelectionChanged(null, null);
            //}
        }

        private void NotifyEmailOpenedToIQ(string interactionId)
        {
            if (_isInteractionQueue && dgIQ.SelectedItem != null)
            {
                DataRowView row = dgIQ.SelectedItem as DataRowView;
                if (row != null && row["InteractionId"].ToString() == interactionId)
                    dgIQ_SelectionChanged(null, null);
            }
            //{
            //    lstOpenedIQInteraction.Add(interactionId);

            //    if (dgIQ.SelectedItem != null)
            //        dgIQ_SelectionChanged(null, null);
            //}


        }

        private void dgIQ_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {

        }

        private void dgIQ_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                if (!iqDisplayedColoumns.Split(',').ToList().Contains(e.Column.Header.ToString())
               || e.Column.Header.ToString() == "State" || string.IsNullOrEmpty(e.Column.Header.ToString()))
                {
                    e.Column.Width = new Microsoft.Windows.Controls.DataGridLength(0.0);
                    e.Column.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    if (e.Column.Header.ToString() == "Subject")
                    {
                        e.Column.MinWidth = 250;
                        e.Column.Width = new Microsoft.Windows.Controls.DataGridLength(1, Microsoft.Windows.Controls.DataGridLengthUnitType.Star);
                    }
                    else
                    {
                        e.Column.MinWidth = 150;
                        //e.Column.Width = new Microsoft.Windows.Controls.DataGridLength()
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void txt_GotMouseCapture(object sender, MouseEventArgs e)
        {
            SelectAll(sender as TextBox);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    GetLastAccessWorkbin();
                    if (WorkbinUtility.Instance().IsSuperadmin)
                        btnMyTeamWorkbin.Visibility = Visibility.Visible;

                    if (ConfigContainer.Instance().AllKeys.Contains("interaction-management.filters")
                                  && !string.IsNullOrEmpty(ConfigContainer.Instance().GetAsString("interaction-management.filters")))
                    {
                        btnMyInteractionQueue.Visibility = Visibility.Visible;
                        CreateMediaColumnWithImages();
                    }

                    if (workbinLoaded == false && WorkbinUtility.Instance().IsAgentLoginIXN)
                    {
                        CreateWorkbinFieldsContextMenuItem("From", "Subject", "Received");
                        try
                        {
                            InteractionService.InteractionServerNotifier -= InteractionService_InteractionServerNotifier;
                            WorkbinService.WorkbinChangedEvent -= NotifyWorkbinChangedEvent;
                            WorkbinService.ContactServerNotificationHandler -= ContactServerNotification;
                            WorkbinService.InteractionServerLoginNotification -= WorkbinService_InteractionServerLoginNotification;
                        }
                        catch { }

                        InteractionService.InteractionServerNotifier += InteractionService_InteractionServerNotifier;
                        WorkbinService.WorkbinChangedEvent += NotifyWorkbinChangedEvent;
                        WorkbinService.ContactServerNotificationHandler += ContactServerNotification;
                        WorkbinService.InteractionServerLoginNotification += WorkbinService_InteractionServerLoginNotification;
                        PerformLoadMail();
                        SetPersonalWorkbinBackground();
                        workbinLoaded = true;
                    }
                    else if (!WorkbinUtility.Instance().IsAgentLoginIXN)
                    {
                        txtUnavailableError.Text = "Agent not logged in interaction server";
                        txtUnavailableError.Visibility = Visibility.Visible;
                        gridCompleteContainer.Visibility = Visibility.Collapsed;
                        ImgPreload.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        dgWorkbin_SelectedCellsChanged(null, null);
                    }
                    dgWorkbin.Focus();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occured as " + ex.Message);
                }
                finally { GC.SuppressFinalize(this); GC.Collect(); }
            }));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void ViewMailButton()
        {

            ShowMainMenu();
            if (WorkbinUtility.Instance().IsWorkbinEnable)
            {
                gcMarkdone.Width = (isShowMarkDone && WorkbinUtility.Instance().IsContactServerActive) ? GridLength.Auto : new GridLength(0);
                gcReplyAll.Width = (isShowReplyAll && WorkbinUtility.Instance().IsContactServerActive) ? GridLength.Auto : new GridLength(0);
                gcPrint.Width = gcOpenMail.Width = (isShowOpen && WorkbinUtility.Instance().IsContactServerActive) ? GridLength.Auto : new GridLength(0);
                gcReply.Width = (isShowReply && WorkbinUtility.Instance().IsContactServerActive) ? GridLength.Auto : new GridLength(0);
                gcDelete.Width = (isShowDelete && WorkbinUtility.Instance().IsContactServerActive) ? GridLength.Auto : new GridLength(0);

                //Code to show tooltip according to selected mail count.
                if (dgWorkbin.SelectedItems != null && dgWorkbin.SelectedItems.Count > 1)
                {
                    MarkDoneMailContent.Text = "Mark done selected mails.";
                    txtDeleteToolTip.Text = "Delete the selected mails.";
                    WorkbinMailContent.Text = "Transfer the selected mails.";
                }
                else
                {
                    MarkDoneMailContent.Text = "Mark done selected mail.";
                    txtDeleteToolTip.Text = "Delete the selected mail.";
                    WorkbinMailContent.Text = "Transfer the selected mail.";
                }

                // If the mail is actively handling, Need to hide all the button.
                if (_isInteractionQueue && _isActivelyHandling)
                    gcMarkdone.Width = gcReplyAll.Width = gcPrint.Width = gcOpenMail.Width = gcReply.Width = gcDelete.Width = gcWorkbin.Width = new GridLength(0);
                else
                    gcWorkbin.Width = GridLength.Auto;
            }
            else
            {
                gcReplyAll.Width = gcPrint.Width = gcOpenMail.Width = gcDelete.Width = gcReply.Width = gcMarkdone.Width = new GridLength(0);

            }
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
                        if (dgWorkbin.Columns.Where(x => x.Header.Equals(selectedItem.Header)).ToList().Count > 0)
                        {
                            selectedItem.Icon = new Image
                            {
                                Height = 15,
                                Width = 15,
                                Source =
                                    new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\TickBlack.png", UriKind.Relative))
                            };
                            dgWorkbin.Columns.Where(x => x.Header.Equals(selectedItem.Header)).SingleOrDefault().Visibility = Visibility.Visible;
                        }

                    }
                    else
                    {
                        if (dgWorkbin.Columns.Where(x => x.Header.Equals(selectedItem.Header)).ToList().Count > 0)
                        {
                            selectedItem.Icon = null;
                            dgWorkbin.Columns.Where(x => x.Header.Equals(selectedItem.Header)).SingleOrDefault().Visibility = Visibility.Collapsed;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        void WorkbinService_InteractionServerLoginNotification()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    workbinData.WorkbinMenu.Clear();
                    if (WorkbinUtility.Instance().IsAgentLoginIXN)
                    {
                        InteractionService.InteractionServerNotifier -= InteractionService_InteractionServerNotifier;
                        WorkbinService.WorkbinChangedEvent -= NotifyWorkbinChangedEvent;
                        WorkbinService.ContactServerNotificationHandler -= ContactServerNotification;
                        WorkbinService.InteractionServerLoginNotification -= WorkbinService_InteractionServerLoginNotification;
                        workbinLoaded = false;
                        UserControl_Loaded(null, null);
                        if (!workbinLoaded)
                        {
                            if (isPersonalWorkbinSelected)
                                LoadPersonalWorkbin();
                            else
                                LoadMyTeamlWorkbin();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        #endregion Methods

        private void userControlWorkbin_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dgIQ.Visibility == Visibility.Visible && brdIQ.ActualWidth > 0)
                dgIQ.Width = brdIQ.ActualWidth - 10;
        }

        public string GetLocalDateTime(string datetime)
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = enUS.DateTimeFormat;
            DateTime _time;
            if (DateTime.TryParse(datetime, null, System.Globalization.DateTimeStyles.AssumeLocal, out _time))
                return _time.ToString(dtfi);
            else
                return datetime;
        }
    }
}