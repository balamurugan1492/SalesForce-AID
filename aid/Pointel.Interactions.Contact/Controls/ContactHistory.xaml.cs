/*
 * Implemented hidden keys:
 * ------------------------
 * 1.contact.enable.show-handled-agent
 * 2.contact.history.enable.export
 *
 * */
namespace Pointel.Interactions.Contact.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

    using Microsoft.Win32;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Contact.ApplicationReader;
    using Pointel.Interactions.Contact.Core;
    using Pointel.Interactions.Contact.Core.Common;
    using Pointel.Interactions.Contact.Helpers;
    using Pointel.Interactions.Contact.Settings;
    using Pointel.Interactions.IPlugins;
    using Pointel.Windows.Views.Common.Editor.Controls;

    /// <summary>
    /// Interaction logic for ContactHistory.xaml
    /// </summary>
    public partial class ContactHistory : UserControl
    {
        #region Fields

        private string contactID;
        private DataTable dtContactHistory = null;
        private int end = 0;
        string filteredMediaType = "all";
        private EventGetDocument getAttachDocument = null;
        private HistoryType historyType;
        private bool isAddedMediaIcon = false;
        private bool isExpand = false;
        private bool isHasNext = false;
        private bool isNeedShowHadledAgent = true;
        private bool IsOpenVisible;
        private List<HistoryCriteria> lastSearchingCriteria = null;
        private List<string> listHistoryDisplayedColoumns;
        private Pointel.Logger.Core.ILog logger = null;
        private string ownerID;
        private string selectedContactID = null;
        private string selectedinteractionId = null;
        private string selectedMailState = null;

        //paging variables
        private int start = 0;
        private Dictionary<string, double> timeFilterValue = null;
        private AdvanceSearch _advanceSearch = null;
        private ObservableCollection<Helpers.CaseData> _casedataList = null;
        private ConfigContainer _configContainer = ConfigContainer.Instance();
        private string _fileExtension = string.Empty;
        private ContextMenu _mediaFilterContextMenu = null;

        #endregion Fields

        #region Constructors

        public ContactHistory(string contactId, string mediaType)
        {
            this.contactID = contactId;
            //this.mediaType = mediaType;
            historyType = HistoryType.ContactHistory;
            InitializeComponent();
            if (_configContainer.AllKeys.Contains("contact.enable.show-handled-agent") && !string.IsNullOrEmpty(_configContainer.GetValue("contact.enable.show-handled-agent"))
                && ((string)_configContainer.GetValue("contact.enable.show-handled-agent")).ToLower() == "false")
                isNeedShowHadledAgent = false;
            ContactHandler.EmailStateNotificationEvent += new EmailStateNotifier(ContactHandler_EmailStateNotificationEvent);
            listHistoryDisplayedColoumns = ReadKey.ReadConfigKeys("contact.history-displayed-columns",
                                                        new string[] { "Status", "Subject", "StartDate", "EndDate", "OwnerId" },
                                                        (new InteractionAttributes()).GetType().GetProperties().Select(x => x.Name).ToList());
        }

        public ContactHistory(string ownerId)
        {
            this.ownerID = ownerId;
            historyType = HistoryType.MyHistory;
            isNeedShowHadledAgent = false;
            InitializeComponent();
            ContactHandler.EmailStateNotificationEvent += new EmailStateNotifier(ContactHandler_EmailStateNotificationEvent);
            listHistoryDisplayedColoumns = ReadKey.ReadConfigKeys("contact.history-displayed-columns",
                                                        new string[] { "Status", "Subject", "StartDate", "EndDate", "OwnerId" },
                                                        (new InteractionAttributes()).GetType().GetProperties().Select(x => x.Name).ToList());
        }

        ~ContactHistory()
        {
            timeFilterValue = null;
            //mediaType =
            contactID = ownerID = selectedMailState = selectedinteractionId = filteredMediaType = null;
            _advanceSearch = null;
            _mediaFilterContextMenu = null;
            logger = null;
            lastSearchingCriteria = null;
            UnSubcribeNotificationEvent();
            listHistoryDisplayedColoumns = null;
        }

        #endregion Constructors

        #region Enumerations

        private enum HistoryType
        {
            None, MyHistory, ContactHistory
        }

        #endregion Enumerations

        #region Methods

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

        public void CreateMediaColumnWithImages()
        {
            Microsoft.Windows.Controls.DataGridTemplateColumn statusImageColumn = new Microsoft.Windows.Controls.DataGridTemplateColumn { CanUserReorder = false, Width = 30, CanUserSort = false }; ;
            statusImageColumn.Header = "";
            statusImageColumn.CellTemplateSelector = new MediaImageDataTemplateSelector();
            if (!isAddedMediaIcon)
            {
                dgContactHistory.Columns.Insert(0, statusImageColumn);
                isAddedMediaIcon = true;
            }
            else
            {
                dgContactHistory.Columns.RemoveAt(0);
                dgContactHistory.Columns.Insert(0, statusImageColumn);
            }
        }

        public EventGetDocument GetDocumentContent(string documentId)
        {
            try
            {
                ContactService contactService = new ContactService();
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestToGetDocument.GetAttachDocument(documentId);
                if (output.IContactMessage != null)
                {
                    if (output.IContactMessage.Id == EventGetDocument.MessageId)
                    {
                        logger.Info(documentId + " : Document returned from UCs Successfully........");
                        return (EventGetDocument)output.IContactMessage;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as : " + generalException.Message);
            }
            return null;
        }

        private void AdvanceSearch_eventReceivedFilteredData(List<Criteria> searchCriteria, MatchCondition matchCondition)
        {
            try
            {
                List<HistoryCriteria> historyCondition = new List<HistoryCriteria>();
                List<HistoryCriteria> basicHistoryCondition = new List<HistoryCriteria>();
                if (searchCriteria.Where(x => !string.IsNullOrEmpty(x.Value)).ToList().Count == 0)
                {
                    //txtMessage.Text = "Please fill some search criteria and then try.";
                    //starttimerforerror();
                    //return;
                }
                basicHistoryCondition.Add(GetBasicIDCriteria());
                if (CustomSlider.Value != 25 && searchCriteria.Where(x => x.Field == "StartDate").ToList().Count == 0)
                {
                    basicHistoryCondition.Add(GetCustomTimeSlider());
                }
                if (filteredMediaType != "all")
                    basicHistoryCondition.Add(GetMediaFilter());

                for (int index = 0; index < searchCriteria.Count; index++)
                {
                    if (string.IsNullOrEmpty(searchCriteria[index].Value)) continue;
                    Operators comparisonOperator = Operators.Equal;
                    string attributeValue = searchCriteria[index].Value;

                    if ("status".Equals(searchCriteria[index].Field.ToLower()))
                    {
                        if ("all".Equals(searchCriteria[index].Value.ToLower()))
                            continue;
                        else
                        {
                            if ("in progress".Equals(attributeValue.ToLower()))
                                attributeValue = "2";
                            else if ("done".Equals(attributeValue.ToLower()))
                                attributeValue = "3";
                        }
                    }
                    else
                    {
                        //Code to convert the Search Condition.
                        if (searchCriteria[index].Condition == SearchCondition.Contains)
                        {
                            comparisonOperator = Operators.Like;
                            attributeValue = "*" + searchCriteria[index].Value + "*";
                        }
                        else if (searchCriteria[index].Condition == SearchCondition.Before)
                        {
                            comparisonOperator = Operators.Lesser;
                        }
                        else if (searchCriteria[index].Condition == SearchCondition.OnorAfter)
                        {
                            comparisonOperator = Operators.GreaterOrEqual;
                        }
                        else if (searchCriteria[index].Condition == SearchCondition.On)
                        {
                            comparisonOperator = Operators.Equal;
                        }

                        //Code to convert the search value.
                        if (searchCriteria[index].Condition == SearchCondition.Before || searchCriteria[index].Condition == SearchCondition.OnorAfter
                            || searchCriteria[index].Condition == SearchCondition.On)
                        {
                            DateTime dt = DateTime.Parse(searchCriteria[index].Value);
                            attributeValue = dt.Year + "-" + dt.Month + "-" + dt.Day + "T00:00:00.0000000Z";
                        }
                    }
                    if (searchCriteria[index].Condition == SearchCondition.On)
                    {
                        historyCondition.Add(new HistoryCriteria() { AttributeName = searchCriteria[index].Field, AttributeValue = attributeValue, ComparisonOperator = Operators.GreaterOrEqual, Prefixes = Prefixes.And });
                        DateTime dt = DateTime.Parse(searchCriteria[index].Value);
                        dt = dt.AddDays(1);
                        attributeValue = dt.Year + "-" + dt.Month + "-" + dt.Day + "T00:00:00.0000000Z";
                        historyCondition.Add(new HistoryCriteria() { AttributeName = searchCriteria[index].Field, AttributeValue = attributeValue, ComparisonOperator = Operators.Lesser, Prefixes = Prefixes.And });
                        continue;
                    }
                    historyCondition.Add(new HistoryCriteria() { AttributeName = searchCriteria[index].Field, AttributeValue = attributeValue, ComparisonOperator = comparisonOperator, Prefixes = (matchCondition == MatchCondition.MatchAll) ? Prefixes.And : Prefixes.Or });

                }
                SearchHistory(basicHistoryCondition, CustomSlider.Value == 0, true, historyCondition, matchCondition == MatchCondition.MatchAll);
                //GetSelectedHistory();
                btnFirstIndex_Click(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void attacScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void BindNeccessaryValues()
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

            _mediaFilterContextMenu = new ContextMenu();
            foreach (MenuItem item in HistoryMediaFilters())
                _mediaFilterContextMenu.Items.Add(item);

            foreach (MenuItem menuItem in _mediaFilterContextMenu.Items)
            {
                if (filteredMediaType.ToLower() == menuItem.Tag.ToString().ToLower())
                {
                    menuItem.Icon = new Image
                    {
                        Height = 15,
                        Width = 15,
                        Source =
                            new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Contact\\TickBlack.png", UriKind.Relative))
                    };
                    break;
                }
                else
                {
                    menuItem.Icon = null;
                }
            }
        }

        private void brdContactHistory_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnAdvanceSearchResponse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PerformBasicSearch();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnAdvanceSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSearch.Text = string.Empty;
                if (grsearch.Height != GridLength.Auto)
                {
                    if (_advanceSearch == null)
                    {

                        List<string> listSearchItem = ReadKey.ReadConfigKeys("contact.history-search-attributes",
                                                                             new string[] { "Status", "StartDate", "EndDate", "Subject" },
                                                                             (new InteractionAttributes()).GetType().GetProperties().Select(x => x.Name).ToList());

                        List<string> listAdvancedDefault = ReadKey.ReadConfigKeys("contact.history-advanced-default",
                                                                                  new string[] { "Status", "StartDate" },
                                                                                  (new InteractionAttributes()).GetType().GetProperties().Select(x => x.Name).ToList());

                        listSearchItem.AddRange(listAdvancedDefault);
                        listSearchItem = listSearchItem.Distinct().ToList();

                        string historyAdvancedDefault = string.Join<string>(",", listAdvancedDefault);

                        List<ComboBoxItem> cmbList = new List<ComboBoxItem>();

                        foreach (string item in listSearchItem.Distinct().Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList())
                        {
                            ComboBoxItem cmbItem = new ComboBoxItem() { Content = item };
                            if (historyAdvancedDefault.Contains(item))
                                cmbItem.Tag = "Default";
                            cmbList.Add(cmbItem);
                        }

                        _advanceSearch = new AdvanceSearch(cmbList) { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
                        _advanceSearch.eventReceivedFilteredData += new AdvanceSearch.EventReceivedFilteredData(AdvanceSearch_eventReceivedFilteredData);
                    }
                    grbAdvanceSearch.Content = _advanceSearch;
                    grsearch.Height = GridLength.Auto;
                    ExpandGrid.Height = new GridLength(1, GridUnitType.Star);
                    DetailsGrid.Height = new GridLength(1, GridUnitType.Star);
                    ExpandGrid.MinHeight = 130;
                    DetailsGrid.MinHeight = 120;

                    txtSearch.IsEnabled = false;
                    AdvanceSearchImage.Source = new BitmapImage(new Uri("\\Agent.Interaction.Desktop;component\\Images\\Contact\\HideAdvanceSearchOption-01.png", UriKind.Relative));
                    ShowAdvanceSearchContent.Text = "Hide advanced search option.";
                    brdSearchBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ExpandGrid.MinHeight = 0;
                    DetailsGrid.MinHeight = 0;
                    grbAdvanceSearch.Content = null;
                    grsearch.Height = new GridLength(0);
                    if (isExpand)
                    {
                        ExpandGrid.Height = new GridLength(1, GridUnitType.Star);
                        DetailsGrid.Height = new GridLength(0);
                    }
                    else
                    {
                        ExpandGrid.Height = new GridLength(145);
                        DetailsGrid.Height = new GridLength(1, GridUnitType.Star);
                    }

                    txtSearch.IsEnabled = true;
                    AdvanceSearchImage.Source = new BitmapImage(new Uri("\\Agent.Interaction.Desktop;component\\Images\\Contact\\DoubleArrow.png", UriKind.Relative));
                    ShowAdvanceSearchContent.Text = "Show advanced search option.";
                    brdSearchBar.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("Error occcured as " + ex.Message);
            }
        }

        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseError(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnContactExpand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExpandGrid.MinHeight = 0;
                DetailsGrid.MinHeight = 0;
                if (isExpand)
                {
                    isExpand = false;
                    imgContact.Source = null;
                    imgContact.Source = new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Contact\\Limited.png", UriKind.Relative));
                    tbShowInteraction.Visibility = System.Windows.Visibility.Visible;
                    ExpandGrid.Height = new GridLength(145);
                    DetailsGrid.Height = new GridLength(1, GridUnitType.Star);
                    ExpandContent.Text = "Hide details for selected interation.";
                    ExpandHeading.Text = "Hide Details";
                }
                else
                {
                    isExpand = true;
                    imgContact.Source = null;
                    imgContact.Source = new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Contact\\Detailed.png", UriKind.Relative));
                    tbShowInteraction.Visibility = System.Windows.Visibility.Collapsed;
                    ExpandGrid.Height = new GridLength(1, GridUnitType.Star);
                    DetailsGrid.Height = new GridLength(0);
                    ExpandContent.Text = "Show details for selected interation.";
                    ExpandHeading.Text = "Show Details";

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnDownArrowFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mediaFilterContextMenu.Style = (Style)FindResource("ContextMenu");
                _mediaFilterContextMenu.PlacementTarget = btnDownArrowFilter;
                _mediaFilterContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                _mediaFilterContextMenu.IsOpen = true;
                _mediaFilterContextMenu.Focus();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnFirstIndex_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //  btnFirst.IsEnabled = false;
                try
                {
                    start = 0;
                    end = start + int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    GetSelectedHistory();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while move the history item to first index as " + ex.Message);
                }
            }));
        }

        private void btnLastIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    btnNextPage.IsEnabled = false;
                    btnFirst.IsEnabled = false;
                    //This code used to retrieve the remaining 50 data.
                    if (!(dtContactHistory.Rows.Count > (start + int.Parse(cmbItemsPerPage.SelectedValue.ToString())))
                        && isHasNext && !(dtContactHistory.Rows.Count > end))
                    {
                        SearchHistory(lastSearchingCriteria, CustomSlider.Value == 0, false);
                    }
                    else if (!(dtContactHistory.Rows.Count > (start + int.Parse(cmbItemsPerPage.SelectedValue.ToString())))
                        && !(dtContactHistory.Rows.Count > end))
                        return;
                    start += int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    end = start + int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    GetSelectedHistory();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utility.EmailUtility.IsEmailReachMaximumCount())
                {
                    lblError.Text = "Email reached maximum count. Please close opened mail and then try to open.";
                    StartTimerForError();
                    return;
                }

                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Workbin))
                {
                    if (!((IWorkbinPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Workbin]).NotifyEmailOpen(selectedinteractionId))
                    {
                        ShoWMessage("Unable to open interaction.", true);
                    }
                    dgContactHistory_SelectionChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void btnPreviousIndex_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    btnPreviousPage.IsEnabled = false;
                    start -= int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    end = start + int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    if (start < 0)
                        start = 0;
                    GetSelectedHistory();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        private void btnReplyAll_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (Utility.EmailUtility.IsEmailReachMaximumCount())
                    {
                        lblError.Text = "Email reached maximum count. Please close opened mail and then try to open.";
                        StartTimerForError();
                        return;
                    }
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                    {
                        ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailReply(selectedinteractionId, true);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while replyAll as " + ex.Message);
                }
            }));
        }

        private void btnReply_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (Utility.EmailUtility.IsEmailReachMaximumCount())
                    {
                        lblError.Text = "Email reached maximum count. Please close opened mail and then try to open.";
                        StartTimerForError();
                        return;
                    }
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                    {
                        ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailReply(selectedinteractionId);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while reply mail as " + ex.Message);
                }
            }));
        }

        private void btnReSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utility.EmailUtility.IsEmailReachMaximumCount())
                {
                    lblError.Text = "Email reached maximum count. Please close opened mail and then try to open.";
                    StartTimerForError();
                    return;
                }
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                {
                    ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyNewEmail(txtTo.Text, selectedContactID, selectedinteractionId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void Btn_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Btn_Export.ContextMenu != null)
                    Btn_Export.ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while View export data context menu as " + ex.Message);
            }
        }

        private void CloseError(object sender, EventArgs e)
        {
            try
            {
                grMessage.Height = new GridLength(0);
                DispatcherTimer _timerforcloseError = sender as DispatcherTimer;
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as  " + ex.Message);
            }
        }

        private void cmbItemsPerPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtContactHistory.Rows.Count < (start + int.Parse(cmbItemsPerPage.SelectedValue.ToString()))
                       && isHasNext && dtContactHistory.Rows.Count > end)
                {
                    SearchHistory(lastSearchingCriteria, CustomSlider.Value == 0, false);
                }
                btnFirstIndex_Click(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        void ContactHandler_EmailStateNotificationEvent()
        {
            if (dgContactHistory.Items.Count > 0)
                dgContactHistory_SelectionChanged(null, null);
        }

        void ContactHandler_InteractionServerNotificationHandler()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ContactDataContext.GetInstance().IsInteractionServerActive)
                    HideEmailButton();
                else
                    dgContactHistory_SelectionChanged(null, null);
            }));
        }

        private void ContactServerNotification()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (ContactDataContext.GetInstance().IsContactServerActive)
                    {
                        HideEmailButton();
                        UserControl_Loaded(null, null);
                        CustomSlider_ValueChanged(null, null);
                        HideMessage();
                    }
                    else
                    {
                        ShoWMessage("The contact server is not active. Please conact your administrator.", false);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        private void ConvertData(EventInteractionListGet eventInteractionListGet, bool needCreateTable)
        {
            List<string> lstAttribute = new List<string>(listHistoryDisplayedColoumns);
            lstAttribute.Add("MediaTypeId");
            lstAttribute.Add("TypeId");
            lstAttribute.Add("SubtypeId");
            lstAttribute.Add("InteractionId");

            lstAttribute = lstAttribute.Distinct().ToList();

            int startingIndex = 0;
            if (needCreateTable)
            {
                dtContactHistory = new DataTable();
                dtContactHistory.Columns.Clear();
                start = 0;
                end += int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                for (int index = 0; index < lstAttribute.Count; index++)
                    dtContactHistory.Columns.Add(lstAttribute[index]);
            }
            else
            {
                if (dtContactHistory != null)
                    startingIndex = dtContactHistory.Rows.Count;
            }

            if (eventInteractionListGet != null)
            {
                isHasNext = (bool)eventInteractionListGet.HasNext;

                if (eventInteractionListGet.InteractionData != null)
                    for (int index = startingIndex; index < eventInteractionListGet.InteractionData.Count; index++)
                    {
                        //  Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute[] attribute=eventInteractionListGet.InteractionData[index].Attributes.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>()
                        //    .Where(x=> x in )
                        DataRow historyRow = dtContactHistory.NewRow();
                        historyRow["InteractionId"] = eventInteractionListGet.InteractionData[index].Id;
                        for (int attrIndex = 0; attrIndex < eventInteractionListGet.InteractionData[index].Attributes.Count; attrIndex++)
                            if (dtContactHistory.Columns.Cast<DataColumn>().Where(x => x.ColumnName == eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName).ToList().Count > 0)
                                if (eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName == "Status")
                                {
                                    var status = eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeValue.ToString();
                                    if (Convert.ToInt32(status) <= 2)
                                        historyRow["Status"] = "In Progress";
                                    else if ("3" == status)
                                        historyRow["Status"] = "Done";

                                }
                                else if (eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName.ToString() == "StartDate"
                                    || eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName.ToString() == "EndDate")
                                {
                                    historyRow[dtContactHistory.Columns[eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName.ToString()]] = eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeValue.ToString().GetLocalDateTime();
                                }
                                else
                                    historyRow[eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeName.ToString()] = eventInteractionListGet.InteractionData[index].Attributes[attrIndex].AttributeValue;
                        dtContactHistory.Rows.Add(historyRow);
                    }
            }

            //GetSelectedHistory();
        }

        private void ConvertData(EventGetInteractionsForContact eventGetInteractionsForContact, bool needCreateTable)
        {
            List<string> lstAttribute = new List<string>(listHistoryDisplayedColoumns);
            lstAttribute.Add("MediaTypeId");
            lstAttribute.Add("TypeId");
            lstAttribute.Add("SubtypeId");
            lstAttribute.Add("InteractionId");
            lstAttribute = lstAttribute.Distinct().ToList();
            int startingIndex = 0;
            if (needCreateTable)
            {
                dtContactHistory = new DataTable();
                dtContactHistory.Columns.Clear();
                start = 0;
                end += int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                for (int index = 0; index < lstAttribute.Count; index++)
                    dtContactHistory.Columns.Add(lstAttribute[index]);
            }
            else
            {
                if (dtContactHistory != null)
                    startingIndex = dtContactHistory.Rows.Count;
            }

            if (eventGetInteractionsForContact != null)
            {
                if (eventGetInteractionsForContact.ContactInteractions != null)
                {
                    for (int index = 0; index < eventGetInteractionsForContact.ContactInteractions.Count; index++)
                    {
                        DataRow historyRow = dtContactHistory.NewRow();
                        historyRow["InteractionId"] = eventGetInteractionsForContact.ContactInteractions[index].InteractionId;
                        for (int columnIndex = 0; columnIndex < dtContactHistory.Columns.Count; columnIndex++)
                            if (eventGetInteractionsForContact.ContactInteractions[index].InteractionAttributes.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x => x.AttributeName
                                == dtContactHistory.Columns[columnIndex].ColumnName).ToList().Count > 0)
                            {
                                string attributeValue = eventGetInteractionsForContact.ContactInteractions[index].InteractionAttributes.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x => x.AttributeName
                                == dtContactHistory.Columns[columnIndex].ColumnName).SingleOrDefault().AttributeValue.ToString();

                                if (dtContactHistory.Columns[columnIndex].ColumnName == "Status")
                                {
                                    if (Convert.ToInt32(attributeValue) <= 2)
                                        historyRow["Status"] = "In Progress";
                                    else if ("3" == attributeValue)
                                        historyRow["Status"] = "Done";

                                }
                                else if (dtContactHistory.Columns[columnIndex].ColumnName == "StartDate" || dtContactHistory.Columns[columnIndex].ColumnName == "EndDate")
                                {
                                    historyRow[dtContactHistory.Columns[columnIndex].ColumnName] = attributeValue.GetLocalDateTime();
                                }
                                else
                                    historyRow[dtContactHistory.Columns[columnIndex].ColumnName] = attributeValue;
                            }
                        dtContactHistory.Rows.Add(historyRow);
                    }
                }
            }
            //GetSelectedHistory();
        }

        private string ConvertSecondsToHHMMSS(int seconds)
        {
            string duration = string.Empty;
            try
            {
                TimeSpan t = TimeSpan.FromSeconds(seconds);
                if (t.TotalMinutes < 1.0)
                {
                    duration = String.Format("{0}s", t.Seconds);
                }
                else if (t.TotalHours < 1.0)
                {
                    duration = String.Format("{0}m {1}s", t.Minutes, t.Seconds);
                }
                else // more than 1 hour
                {
                    duration = String.Format("{0}h {1:D2}m {2:D2}s", (int)t.TotalHours, t.Minutes, t.Seconds);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
            return duration;
        }

        private void CreateTimeSlideValue()
        {
            timeFilterValue = new Dictionary<string, double>();
            timeFilterValue.Add("Arch", 0);
            timeFilterValue.Add("All", 25);
            timeFilterValue.Add("1M", 50);
            timeFilterValue.Add("1W", 75);
            timeFilterValue.Add("1D", 100);
            string key = "All";
            if (historyType == HistoryType.ContactHistory)
            {
                if (_configContainer.AllKeys.Contains("contact.history-default-time-filter-main")
                    && !string.IsNullOrEmpty(((string)_configContainer.GetValue("contact.history-default-time-filter-main")))
                    && timeFilterValue.ContainsKey(_configContainer.GetValue("contact.history-default-time-filter-main"))
                    )
                    key = (string)_configContainer.GetValue("contact.history-default-time-filter-main");
                else
                    key = "1M";
            }
            else
            {
                if (_configContainer.AllKeys.Contains("contact.myhistory-default-time-filter-main")
                    && !string.IsNullOrEmpty(((string)_configContainer.GetValue("contact.myhistory-default-time-filter-main")))
                    && timeFilterValue.ContainsKey(_configContainer.GetValue("contact.myhistory-default-time-filter-main"))
                    )
                    key = (string)_configContainer.GetValue("contact.myhistory-default-time-filter-main");
                else
                    key = "1W";

            }
            if (timeFilterValue.ContainsKey(key))
                CustomSlider.Value = (double)timeFilterValue[key];
            else
                CustomSlider.Value = 0;
        }

        private void CustomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //PerformHistoryIntitalLoad();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //if (txtSearch.IsEnabled)
                    if (grsearch.Height != GridLength.Auto)
                        PerformBasicSearch();
                    else if (_advanceSearch != null)
                        _advanceSearch.Search_Click(null, null);

                }));
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void dgContactHistory_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                if (!(listHistoryDisplayedColoumns.Contains(e.Column.Header.ToString()))
               || e.Column.Header.ToString() == "State" || string.IsNullOrEmpty(e.Column.Header.ToString()))
                {
                    e.Column.Width = new Microsoft.Windows.Controls.DataGridLength(0.0);
                    e.Column.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    if (e.Column.Header.ToString() == "Subject")
                        e.Column.Width = new Microsoft.Windows.Controls.DataGridLength(1, Microsoft.Windows.Controls.DataGridLengthUnitType.Star);
                    else
                    {
                        e.Column.MinWidth = 65.0;
                        //e.Column.Width = new Microsoft.Windows.Controls.DataGridLength()
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void dgContactHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedContactID = null;
                txtChatState.Text = string.Empty;
                RowState.Height = RowChatState.Height = RowVoiceState.Height = new GridLength(0);
                if (dgContactHistory.SelectedItem == null || (dgContactHistory.SelectedItems != null && dgContactHistory.SelectedItems.Count > 1))
                {
                    HideEmailButton();
                    txtDetails.Visibility = Visibility.Visible;
                    pnlDetails.Visibility = gridNote.Visibility = CaseWhite.Visibility = Visibility.Collapsed;

                    return;
                }
                var selectedContactHistory = dgContactHistory.SelectedItem as DataRowView;
                if (selectedContactHistory == null) return;

                txtDetails.Visibility = Visibility.Collapsed;
                pnlDetails.Visibility = gridNote.Visibility = CaseWhite.Visibility = Visibility.Visible;
                DataRow row = selectedContactHistory.Row;
                selectedMailState = row["Status"] as string;
                if (row["InteractionId"].ToString() != null)
                {
                    selectedinteractionId = row["InteractionId"].ToString();
                    //  RowState.Height = new GridLength(0);
                    KeyValueCollection caseData = null;
                    if (selectedMailState.ToLower() != "done")
                    {
                        lblChatState.Text = lblVoiceState.Text = lblState.Text = "State: ";
                        //Updated by sakthi to get Attachment details in Email
                        Pointel.Interactions.Core.InteractionService interactionServer = new Interactions.Core.InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues interactionResult = interactionServer.GetInteractionProperties(ContactDataContext.GetInstance().IxnProxyId, selectedinteractionId);

                        if ("200".Equals(interactionResult.MessageCode))
                        {
                            RowVoiceState.Height = RowChatState.Height = RowState.Height = GridLength.Auto;
                            EventInteractionProperties eventinteractionProperties = interactionResult.IMessage as EventInteractionProperties;
                            if (eventinteractionProperties != null)
                            {
                                caseData = eventinteractionProperties.Interaction.InteractionUserData;
                                switch (eventinteractionProperties.Interaction.InteractionState)
                                {
                                    case InteractionState.Cached:
                                        txtState.Text = "In a Queue.";
                                        break;
                                    case InteractionState.Routing:
                                        txtState.Text = "Delivery in progress.";
                                        break;
                                    case InteractionState.Queued:
                                        if (string.IsNullOrEmpty(eventinteractionProperties.Interaction.InteractionWorkbinTypeId))
                                            txtState.Text = "In a Queue.";
                                        else
                                            txtState.Text = "Assigned to " + GetFullNameOfAgent(eventinteractionProperties.Interaction.InteractionAssignedTo) + " - " + eventinteractionProperties.Interaction.InteractionWorkbinTypeId;
                                        break;
                                    case InteractionState.Handling:
                                        txtChatState.Text = txtState.Text = "Assigned to " + GetFullNameOfAgent(eventinteractionProperties.Interaction.InteractionAssignedTo) + " - Actively handling.";
                                        break;
                                    default:
                                        txtState.Text = string.Empty;
                                        break;
                                }
                                IsOpenVisible = eventinteractionProperties.Interaction.InteractionState != InteractionState.Handling;
                            }
                        }
                        else
                        {
                            RowState.Height = RowChatState.Height = RowVoiceState.Height = new GridLength(0);
                        }
                    }

                    IMessage response = Pointel.Interactions.Contact.Core.Request.RequestToGetInteractionContent.GetInteractionContent(row["InteractionId"].ToString(), row["Media"].ToString().ToLower().Equals("email"));
                    if (response == null) return;
                    if (response.Name.Equals("EventGetInteractionContent"))
                    {
                        CaseWhite.Visibility = gridNote.Visibility = pnlDetails.Visibility = Visibility.Visible;
                        EventGetInteractionContent interactionContent = (EventGetInteractionContent)response;
                        if (caseData == null)
                            caseData = interactionContent.InteractionAttributes.AllAttributes;
                        List<string> casedataFilterKeys = new List<string>();
                        detailEmail.Visibility = Visibility.Collapsed;
                        detailChat.Visibility = Visibility.Collapsed;
                        detailVoice.Visibility = Visibility.Collapsed;
                        //Get Details view
                        if (selectedMailState.ToLower() == "done")
                        {
                            if (!string.IsNullOrEmpty(contactID) && isNeedShowHadledAgent && interactionContent != null && interactionContent.InteractionAttributes != null)
                            {
                                RowState.Height = RowChatState.Height = RowVoiceState.Height = GridLength.Auto;
                                int agentDbId = 0;
                                int tenantDbId = 0;
                                if (interactionContent.InteractionAttributes.OwnerId != null)
                                    agentDbId = (int)interactionContent.InteractionAttributes.OwnerId;
                                if (interactionContent.InteractionAttributes.TenantId != null)
                                    tenantDbId = (int)interactionContent.InteractionAttributes.TenantId;
                                txtState.Text = txtVoiceState.Text = txtChatState.Text = Utility.GenesysUtitlity.GetAgentFullName(agentDbId, tenantDbId);
                                lblChatState.Text = lblVoiceState.Text = lblState.Text = "Handled by: ";
                            }
                            else
                            {
                                RowState.Height = RowChatState.Height = RowVoiceState.Height = new GridLength(0);
                            }
                        }

                        switch (row["Media"].ToString().ToLower())
                        {
                            case "email":
                                GetDetailContentforEmail(interactionContent);
                                txtEmailStartDate.Text = row["StartDate"].ToString();
                                if (_configContainer.AllKeys.Contains("email.enable.case-data-filter")
                        && ((string)_configContainer.GetValue("email.enable.case-data-filter")).ToLower().Equals("true")
                         && _configContainer.AllKeys.Contains("EmailAttachDataFilterKey")
                            && _configContainer.GetValue("EmailAttachDataFilterKey") != null)
                                {
                                    casedataFilterKeys = (List<string>)_configContainer.GetValue("EmailAttachDataFilterKey");
                                    if (_configContainer.AllKeys.Contains("EmailAttachDataKey")
                                        && _configContainer.GetValue("EmailAttachDataKey") != null)
                                    {
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys.AddRange((List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey")));
                                        else
                                            casedataFilterKeys = (List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey"));
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys = casedataFilterKeys.Distinct().ToList();
                                    }
                                }
                                detailEmail.Visibility = Visibility.Visible;
                                gcCall.Width = new GridLength(0);
                                break;

                            case "chat":
                                detailChat.Visibility = Visibility.Visible;
                                mainRTB.Visibility = Visibility.Collapsed;
                                if (selectedMailState.ToLower() == "done")
                                    GetDetailContentforChat(interactionContent);

                                BaseEntityAttributes baseEntityAttributes = interactionContent.EntityAttributes;
                                ChatEntityAttributes chatEntityAttributes = new ChatEntityAttributes();
                                chatEntityAttributes = (ChatEntityAttributes)baseEntityAttributes;

                                double duration = ((DateTime)chatEntityAttributes.ReleasedDate - (DateTime)chatEntityAttributes.EstablishedDate).TotalSeconds;

                                lblSubjectBold.Content = "Chat session with " +
                                    GetContactFirstNameLastName(interactionContent.InteractionAttributes.ContactId) +
                                    (duration > 0 ? ("Duration: " + ConvertSecondsToHHMMSS((int)(duration))) : string.Empty);

                                if (_configContainer.AllKeys.Contains("chat.enable.case-data-filter")
                           && ((string)_configContainer.GetValue("chat.enable.case-data-filter")).ToLower().Equals("true")
                            && _configContainer.AllKeys.Contains("ChatAttachDataFilterKey")
                               && _configContainer.GetValue("ChatAttachDataFilterKey") != null)
                                {
                                    casedataFilterKeys = (List<string>)_configContainer.GetValue("ChatAttachDataFilterKey");
                                    if (_configContainer.AllKeys.Contains("ChatAttachDataKey")
                                       && _configContainer.GetValue("ChatAttachDataKey") != null)
                                    {
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys.AddRange((List<string>)(ConfigContainer.Instance().GetValue("ChatAttachDataKey")));
                                        else
                                            casedataFilterKeys = (List<string>)(ConfigContainer.Instance().GetValue("ChatAttachDataKey"));
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys = casedataFilterKeys.Distinct().ToList();
                                    }
                                }
                                //ContactDataContext.GetInstance().LoadChatCaseDataFilterKeys != null ? ContactDataContext.GetInstance().LoadChatCaseDataFilterKeys : null;
                                HideEmailButton();
                                gcCall.Width = new GridLength(0);

                                break;

                            case "voice":
                                gcCall.Width = GridLength.Auto;
                                GetDetailContentforVoice(interactionContent);
                                if (_configContainer.AllKeys.Contains("voice.enable.case-data-filter")
                           && ((string)_configContainer.GetValue("voice.enable.case-data-filter")).ToLower().Equals("true")
                            && _configContainer.AllKeys.Contains("VoiceAttachDataFilterKey")
                               && _configContainer.GetValue("VoiceAttachDataFilterKey") != null)
                                {
                                    casedataFilterKeys = (List<string>)_configContainer.GetValue("VoiceAttachDataFilterKey");
                                    if (_configContainer.AllKeys.Contains("VoiceAttachDataKey")
                                      && _configContainer.GetValue("VoiceAttachDataKey") != null)
                                    {
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys.AddRange((List<string>)(ConfigContainer.Instance().GetValue("VoiceAttachDataKey")));
                                        else
                                            casedataFilterKeys = (List<string>)(ConfigContainer.Instance().GetValue("VoiceAttachDataKey"));
                                        if (casedataFilterKeys != null)
                                            casedataFilterKeys = casedataFilterKeys.Distinct().ToList();
                                    }
                                }

                                //casedataFilterKeys = ContactDataContext.GetInstance().LoadVoiceCaseDataFilterKeys != null ? ContactDataContext.GetInstance().LoadVoiceCaseDataFilterKeys : null;
                                detailVoice.Visibility = Visibility.Visible;
                                HideEmailButton();
                                break;
                            default: break;
                        }

                        //Get Notes
                        txtNotes.Text = interactionContent.InteractionAttributes.TheComment != null ? interactionContent.InteractionAttributes.TheComment.ToString() : (interactionContent.InteractionAttributes.AllAttributes.Contains("Notes") ? interactionContent.InteractionAttributes.AllAttributes["Notes"].ToString() : string.Empty);

                        //Get CaseData
                        _casedataList = new ObservableCollection<Helpers.CaseData>();
                        if (caseData != null && caseData.AllKeys.Length > 0)
                        {
                            if (casedataFilterKeys != null && casedataFilterKeys.Count > 0)
                            {
                                foreach (var key in casedataFilterKeys)
                                {
                                    if (caseData.ContainsKey(key))
                                    {
                                        Helpers.CaseData attribute = new Helpers.CaseData { Key = key.ToString(), Value = caseData[key.ToString()].ToString() };
                                        _casedataList.Add(attribute);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var attr in caseData.AllKeys)
                                {
                                    Helpers.CaseData attribute = new Helpers.CaseData { Key = attr.ToString(), Value = caseData[attr.ToString()].ToString() };
                                    _casedataList.Add(attribute);
                                }
                            }
                        }

                        dgCaseInfo.ItemsSource = null;
                        dgCaseInfo.ItemsSource = _casedataList;
                        txtNoCaseData.Visibility = (_casedataList.Count == 0) ? Visibility.Visible : Visibility.Collapsed;

                        txtDisposition.Text = "None";
                        if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name")
              && caseData.ContainsKey(ConfigContainer.Instance().GetAsString("interaction.disposition.key-name")))
                            txtDisposition.Text = caseData.GetAsString(ConfigContainer.Instance().GetAsString("interaction.disposition.key-name"));
                    }
                    else
                    {
                        lblError.Text = "Can not retrieve the details to this interaction.";
                        StartTimerForError();
                        CaseWhite.Visibility = gridNote.Visibility = pnlDetails.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void FillDataToExport(ref List<ExportData> lstExportData)
        {
            if (lstExportData != null)
            {
                List<string> lstAttribute = new List<string>();
                List<string> lstCaseData = new List<string>();
                if (_configContainer.AllKeys.Contains("contact.history.export-attribute") &&
                    !string.IsNullOrEmpty(_configContainer.GetAsString("contact.history.export-attribute").Trim()))
                    lstAttribute = _configContainer.GetAsString("contact.history.export-attribute").Split(',').Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();

                if (lstAttribute.Count == 0)
                    lstAttribute = new List<string>(listHistoryDisplayedColoumns);

                if (_configContainer.AllKeys.Contains("contact.history.export-case-data") &&
                    !string.IsNullOrEmpty(_configContainer.GetAsString("contact.history.export-case-data").Trim()))
                    lstCaseData = _configContainer.GetAsString("contact.history.export-case-data").Split(',').Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();

                for (int index = 0; index < lstExportData.Count; index++)
                {

                    if (lstAttribute.Count > 0)
                    {
                        IMessage response = Pointel.Interactions.Contact.Core.Request.RequestToGetInteractionContent.GetInteractionContent(lstExportData[index].InteractionID, false);
                        EventGetInteractionContent interactionContent = null;
                        if (response != null && response.Name.Equals("EventGetInteractionContent"))
                        {
                            interactionContent = (EventGetInteractionContent)response;
                            var interactionAttributes = interactionContent.InteractionAttributes;
                            for (int attributeIndex = 0; attributeIndex < lstAttribute.Count; attributeIndex++)
                            {
                                var regexColumnName = Regex.Match(lstAttribute[attributeIndex], @"\$([^$]*)\$");
                                var columnName = string.IsNullOrEmpty(regexColumnName.Groups[1].Value) ? lstAttribute[attributeIndex] : regexColumnName.Groups[1].Value;
                                string[] temparray = lstAttribute[attributeIndex]
                                    .Replace((regexColumnName.Groups[0].Value == string.Empty ? "####$$####" : regexColumnName.Groups[0].Value), string.Empty)
                                    .Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                string value = null;
                                foreach (var item in temparray)
                                {
                                    var attValue = GetPropertyValue<InteractionAttributes>(item, interactionAttributes);
                                    if (attValue == null) continue;
                                    value += " " + attValue;
                                    value = (item.ToLower().Contains("date") && value.Trim() != null) ? value.Trim().GetLocalDateTime() : value.Trim();
                                }
                                lstExportData[index][columnName] = value != null ? value : string.Empty;
                                #region Old Code
                                //if (interactionattribute.ContainsKey(lstAttribute[attributeIndex]))
                                //    objExportData[index][lstAttribute[attributeIndex]] = interactionattribute[lstAttribute[attributeIndex]] as string;
                                //else
                                //    objExportData[index][lstAttribute[attributeIndex]] = string.Empty;
                                #endregion Old Code
                            }
                        }
                        if (lstCaseData.Count == 0) continue;
                        KeyValueCollection caseDataAttributes = null;
                        if (lstExportData[index].IsDone)
                            caseDataAttributes = interactionContent.InteractionAttributes.AllAttributes;
                        else
                        {
                            Pointel.Interactions.Core.InteractionService interactionServer = new Interactions.Core.InteractionService();
                            Pointel.Interactions.Core.Common.OutputValues interactionResult = interactionServer.GetInteractionProperties(ContactDataContext.GetInstance().IxnProxyId, lstExportData[index].InteractionID);
                            if (interactionResult != null && "200".Equals(interactionResult.MessageCode))
                            {
                                EventInteractionProperties eventinteractionProperties = interactionResult.IMessage as EventInteractionProperties;
                                if (eventinteractionProperties != null)
                                    caseDataAttributes = eventinteractionProperties.Interaction.InteractionUserData;
                            }
                        }
                        if (caseDataAttributes != null)
                        {
                            for (int attributeIndex = 0; attributeIndex < lstCaseData.Count; attributeIndex++)
                            {
                                var regexColumnName = Regex.Match(lstCaseData[attributeIndex], @"\$([^$]*)\$");
                                var columnName = string.IsNullOrEmpty(regexColumnName.Groups[1].Value) ? lstCaseData[attributeIndex] : regexColumnName.Groups[1].Value;
                                string[] temparray = lstCaseData[attributeIndex]
                                    .Replace((regexColumnName.Groups[0].Value == string.Empty ? "####$$####" : regexColumnName.Groups[0].Value), string.Empty)
                                    .Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                string value = null;
                                foreach (var item in temparray)
                                {
                                    value += " " + (caseDataAttributes.ContainsKey(item) ? caseDataAttributes[item] as string : string.Empty);
                                    value = (lstCaseData[attributeIndex].ToLower().Contains("date") && value.Trim() != null) ? value.Trim().GetLocalDateTime() : value.Trim();
                                }
                                lstExportData[index][columnName] = value != null ? value : string.Empty;
                                #region Old Code
                                //if (caseDataAttributes.ContainsKey(lstCaseData[attributeIndex]))
                                //    objExportData[index][lstCaseData[attributeIndex]] = caseDataAttributes[lstCaseData[attributeIndex]] as string;
                                //else
                                //    objExportData[index][lstCaseData[attributeIndex]] = string.Empty;
                                #endregion Old Code
                            }
                        }
                    }
                }
            }
        }

        private SearchCriteriaCollection FormSearchCriteria(List<HistoryCriteria> historyCriteria, bool isArchieve = false)
        {
            SearchCriteriaCollection SearchCriteria = new SearchCriteriaCollection();
            if (historyCriteria != null)
            {
                if (isArchieve)
                {
                    List<HistoryCriteria> objHistorySearch = historyCriteria.Where(x => x.AttributeName != "ContactId" && x.AttributeName != "MediaTypeId"
                        && x.AttributeName != "OwnerId").ToList();
                    for (int index = 0; index < objHistorySearch.Count; index++)
                        historyCriteria.RemoveAt(historyCriteria.IndexOf(objHistorySearch[index]));
                }

                for (int index = 0; index < historyCriteria.Count; index++)
                {
                    SimpleSearchCriteria searchCritiria = new SimpleSearchCriteria() { AttrName = historyCriteria[index].AttributeName, AttrValue = historyCriteria[index].AttributeValue, Operator = new NullableOperators(historyCriteria[index].ComparisonOperator) };
                    if (historyCriteria.Count > 1)
                    {
                        ComplexSearchCriteria complexSearchCriteria = new ComplexSearchCriteria() { Prefix = historyCriteria[index].Prefixes };
                        complexSearchCriteria.Criterias = new SearchCriteriaCollection();
                        complexSearchCriteria.Criterias.Add(searchCritiria);
                        SearchCriteria.Add(complexSearchCriteria);
                    }
                    else
                    {
                        SearchCriteria.Add(searchCritiria);
                    }
                }
                lastSearchingCriteria = historyCriteria;
            }
            return SearchCriteria;
        }

        /// <summary>
        /// The method used to form the interaction search query based on the given history criteria.
        /// </summary>
        /// <param name="historyCriteria">The history criteria.</param>
        /// <returns></returns>
        private string FormSearchQuery(List<HistoryCriteria> historyCriteria, List<HistoryCriteria> advanceSearchCriteria, bool isMatchAll)
        {
            string _query = string.Empty;
            string _queryContainer = string.Empty;
            string filter = string.Empty;
            if (historyCriteria.Where(x => x.AttributeName == "OwnerId").ToList().Count > 0 || historyCriteria.Where(x => x.AttributeName == "ContactId").ToList().Count > 0)
            {
                foreach (HistoryCriteria criteria in historyCriteria.Where(x => x.AttributeName == "ContactId" || x.AttributeName == "OwnerId"))
                    _queryContainer = criteria.AttributeName + ":" + criteria.AttributeValue;
            }

            if (historyCriteria != null && historyCriteria.Count > 0)
            {

                if (historyCriteria.Where(x => x.AttributeName == "MediaTypeId").ToList().Count > 0)
                {
                    foreach (HistoryCriteria criteria in historyCriteria.Where(x => x.AttributeName == "MediaTypeId"))
                        filter += (!string.IsNullOrEmpty(filter) ? " OR " : "") + criteria.AttributeName + ":" + criteria.AttributeValue;
                    _queryContainer += "::Sep" + filter;
                }
                //   if (historyCriteria.Where(x => x.AttributeName == "StartDate").ToList().Count > 0 || historyCriteria.Where(x => x.AttributeName == "EndDate").ToList().Count > 0)
                if (historyCriteria.Where(x => x.IsDate).ToList().Count > 0)
                {
                    filter = string.Empty;
                    //string mediaFilter = string.Empty;
                    foreach (HistoryCriteria criteria in historyCriteria.Where(x => x.AttributeName == "StartDate" || x.AttributeName == "EndDate"))
                        // filter += (!string.IsNullOrEmpty(filter) ? " OR " : "") + criteria.AttributeName + ":" + criteria.AttributeValue;
                        filter += (!string.IsNullOrEmpty(filter) ? " AND " : "") + criteria.AttributeName + ":[" + criteria.AttributeValue + " TO " + criteria.AttributeSubValues + "]";

                    _queryContainer += "::Sep" + filter;
                }
                if (historyCriteria.Where(x => x.AttributeName == "Status").ToList().Count > 0)
                {
                    filter = string.Empty;
                    foreach (HistoryCriteria criteria in historyCriteria.Where(x => x.AttributeName == "Status"))
                        filter += (!string.IsNullOrEmpty(filter) ? " OR " : "") + criteria.AttributeName + ":" + criteria.AttributeValue;
                    _queryContainer += "::Sep" + filter;
                }
                if (historyCriteria.Where(x => x.AttributeName == "Subject").ToList().Count > 0 || historyCriteria.Where(x => x.AttributeName == "ParentId").ToList().Count > 0)
                {
                    filter = string.Empty;
                    foreach (HistoryCriteria criteria in historyCriteria.Where(x => x.AttributeName == "Subject" || x.AttributeName == "ParentId"))
                        filter += (!string.IsNullOrEmpty(filter) ? " OR " : "") + criteria.AttributeName + ":" + criteria.AttributeValue;
                    _queryContainer += "::Sep" + filter;
                }

                foreach (string _queryItem in _queryContainer.Split(new string[] { "::Sep" }, StringSplitOptions.None))
                    _query = _query + (string.IsNullOrEmpty(_query) ? string.Empty : " AND ") + (string.IsNullOrEmpty(_query) ? string.Empty : "(") + _queryItem + (string.IsNullOrEmpty(_query) ? string.Empty : ")");
            }
            if (advanceSearchCriteria != null && advanceSearchCriteria.Count > 0)
            {
                filter = string.Empty;
                string _operator = " OR ";
                if (isMatchAll) _operator = " AND ";
                for (int index = 0; index < advanceSearchCriteria.Count; index++)
                    filter += (!string.IsNullOrEmpty(filter) ? _operator : "") + advanceSearchCriteria[index].AttributeName + ":" + advanceSearchCriteria[index].AttributeValue;
                _query = _query + " AND (" + filter + ")";
            }
            return _query;
        }

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

        private HistoryCriteria GetBasicIDCriteria()
        {
            string id = "OwnerId";
            string value = ownerID;
            start = end = 0;
            if (historyType == HistoryType.ContactHistory)
            {
                id = "ContactId";
                value = contactID;
            }
            HistoryCriteria historyCriteria = new HistoryCriteria() { AttributeName = id, AttributeValue = value, ComparisonOperator = Operators.Equal, Prefixes = Prefixes.And };
            return historyCriteria;
        }

        private string GetContactFirstNameLastName(string contactID)
        {
            string contactName = string.Empty;
            try
            {
                OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestGetAllAttributes.GetAttributeValues(contactID, new List<string>(new string[] { "FirstName", "LastName" }));
                if (result != null && result.IContactMessage.Name.Equals("EventGetAttributes"))
                {
                    logger.Info("BindContactDetails() : contact details retrieved");
                    EventGetAttributes attribute = result.IContactMessage as EventGetAttributes;
                    if (attribute.Attributes != null && attribute.Attributes.Count > 0)
                    {
                        foreach (AttributesHeader attr in attribute.Attributes)
                        {
                            contactName += !string.IsNullOrEmpty(attr.AttributesInfoList[0].AttrValue.ToString()) ? attr.AttributesInfoList[0].AttrValue.ToString() + " " : string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
            return contactName;
        }

        private void getContactHistory(SearchCriteriaCollection SearchCriteria, bool needToRecreateTable = true, bool isArchive = false)
        {
            try
            {
                btnAdvanceSearch.IsEnabled = brdSearchBar.IsEnabled = false;

                List<string> lstAttribute = new List<string>(listHistoryDisplayedColoumns);

                lstAttribute.Add("SubtypeId");
                lstAttribute.Add("Status");
                lstAttribute.Add("Subject");
                lstAttribute.Add("StartDate");
                lstAttribute.Add("EndDate");
                lstAttribute.Add("OwnerId");
                lstAttribute.Add("ContactId");
                lstAttribute.Add("ParentId");
                lstAttribute.Add("MediaTypeId");
                lstAttribute.Add("TypeId");

                lstAttribute = lstAttribute.Distinct().ToList();

                int currentIndex = 0;
                if (dtContactHistory != null)
                {
                    if (Convert.ToInt32(cmbItemsPerPage.SelectedValue) > 50)
                        currentIndex = Convert.ToInt32(cmbItemsPerPage.SelectedValue);
                    else
                        currentIndex = dtContactHistory.Rows.Count + 50;
                }

                if (string.IsNullOrEmpty(contactID))
                {

                    OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestGetInteractionList.GetInteractionList(
                        SearchCriteria, _configContainer.TenantDbId, currentIndex, lstAttribute.Distinct().ToList(),
                        (isArchive ? DataSourceType.Archive : DataSourceType.Main));
                    IMessage response = result.IContactMessage;
                    if (response != null)
                    {
                        Dictionary<string, InteractionAttributeList> _listInteractionAttributeList = new Dictionary<string, InteractionAttributeList>();
                        switch (response.Id)
                        {
                            case EventInteractionListGet.MessageId:
                                EventInteractionListGet eventInteractionListGet = response as EventInteractionListGet;
                                ConvertData(eventInteractionListGet, needToRecreateTable);
                                break;
                            case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                                logger.Trace(response.ToString());
                                lblError.Text = "Your request was not completed because the Universal Contact Server return an error. Please contact your administrator.";
                                StartTimerForError();
                                if (dtContactHistory != null)
                                {
                                    dtContactHistory.Rows.Clear();
                                    start = end = 0;
                                }
                                GetSelectedHistory();
                                break;
                        }
                    }
                }
                else
                {
                    OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestGetContactInteractionList.GetContacts(SearchCriteria, contactID, _configContainer.TenantDbId, currentIndex, lstAttribute);
                    IMessage response = result.IContactMessage;
                    if (response != null)
                    {
                        Dictionary<string, InteractionAttributeList> _listInteractionAttributeList = new Dictionary<string, InteractionAttributeList>();
                        switch (response.Id)
                        {
                            case EventGetInteractionsForContact.MessageId:
                                EventGetInteractionsForContact eventGetInteractionsForContact = response as EventGetInteractionsForContact;
                                ConvertData(eventGetInteractionsForContact, needToRecreateTable);
                                break;
                            case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError.MessageId:
                                logger.Trace(response.ToString());
                                lblError.Text = "Your request was not completed because the Universal Contact Server return an error. Please contact your administrator.";
                                StartTimerForError();
                                if (dtContactHistory != null)
                                {
                                    dtContactHistory.Rows.Clear();
                                    start = end = 0;
                                }
                                GetSelectedHistory();
                                GetSelectedHistory();
                                break;
                        }
                    }

                }

            }
            catch (Exception generalException)
            {
                string d = generalException.Message;
            }
        }

        private HistoryCriteria GetCustomTimeSlider()
        {
            DateTime dt = DateTime.Now; // Today
            if (CustomSlider.Value == 50) //Month
                dt = dt.AddMonths(-1);
            else if (CustomSlider.Value == 75) //Weak
                dt = dt.AddDays(-7);
            string UTCDateTime1 = string.Empty;
            string UTCDateTime2 = string.Empty;
            if (ContactDataContext.GetInstance().IsInteractionIndexFound)
            {
                UTCDateTime1 = dt.Year + "-" + (dt.Month < 10 ? "0" : "") + dt.Month + "-" + (dt.Day < 10 ? "0" : "") + dt.Day + "T00\\:00\\:00.000Z";
                dt = DateTime.Now;
                dt = dt.AddDays(1);
                UTCDateTime2 = dt.Year + "-" + (dt.Month < 10 ? "0" : "") + dt.Month + "-" + (dt.Day < 10 ? "0" : "") + dt.Day + "T00\\:00\\:00.000Z";
            }
            else
            {
                UTCDateTime1 = dt.Year + "-" + dt.Month + "-" + dt.Day + "T00:00:00.0000000Z";
                dt = DateTime.Now;
                dt = dt.AddDays(1);
                UTCDateTime2 = dt.Year + "-" + dt.Month + "-" + dt.Day + "T00:00:00.0000000Z";
            }

            return new HistoryCriteria()
            {
                AttributeName = "StartDate",
                AttributeValue = UTCDateTime1,
                AttributeSubValues = UTCDateTime2,
                ComparisonOperator = Operators.GreaterOrEqual,
                Prefixes = Prefixes.And,
                IsDate = true
            };
        }

        private List<ExportData> GetDataToExport()
        {
            ContactHandler contactHandler = new ContactHandler();
            List<ExportData> dataToExport = new List<ExportData>();
            List<HistoryCriteria> lstSearchHistory = null;
            Dispatcher.Invoke(new Action(delegate()
            {
                lstSearchHistory = GetSearchCriteria();
            }));
            //if (ContactDataContext.GetInstance().IsInteractionIndexFound)
            //{

            //    GetSearchCriteria();
            //    IMessage result = contactHandler.SearchContact(FormSearchQuery(lstSearchHistory), 2000, "interaction");
            //    if (result != null && result.Id == EventSearch.MessageId)
            //    {
            //        EventSearch objEventSearch = result as EventSearch;
            //    }
            //}
            //else
            //{
            List<string> lstAttribute = new List<string>();
            lstAttribute.Add("Status");
            lstAttribute.Add("InteractionId");
            OutputValues result = Pointel.Interactions.Contact.Core.Request.RequestGetInteractionList.GetInteractionList(FormSearchCriteria(lstSearchHistory), _configContainer.TenantDbId, 2000, lstAttribute);
            IMessage response = result.IContactMessage;
            if (response != null)
            {
                if (response.Id == EventInteractionListGet.MessageId)
                {
                    EventInteractionListGet objEventInteractionList = response as EventInteractionListGet;
                    if (objEventInteractionList.InteractionData != null)
                    {
                        for (int index = 0; index < objEventInteractionList.InteractionData.Count; index++)
                        {
                            ExportData objExportData = new ExportData();
                            objExportData.InteractionID = objEventInteractionList.InteractionData[index].Id;
                            if (objEventInteractionList.InteractionData[index].Attributes.Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>()
                                .Where(x => x.AttributeName == "Status").ToList().Count > 0)
                            {
                                Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attribute = objEventInteractionList.InteractionData[index].Attributes.
                                   Cast<Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute>().Where(x => x.AttributeName == "Status")
                                   .SingleOrDefault();
                                objExportData.IsDone = (attribute.AttributeValue.ToString() == "3");
                            }

                            dataToExport.Add(objExportData);
                        }
                    }
                    FillDataToExport(ref dataToExport);
                }

            }

            //}
            return dataToExport;
        }

        private void GetDetailContentforChat(EventGetInteractionContent interactionContent)
        {
            try
            {
                //ContactDataContext.GetInstance().RTBDocument.Blocks.Clear();
                //mainRTB.Document = new FlowDocument();
                //mainRTB.Documents = new FlowDocument();

                string content = string.Empty;
                if (!String.IsNullOrEmpty(interactionContent.InteractionContent.StructuredText))
                {
                    content = interactionContent.InteractionContent.StructuredText;

                }
                else
                {
                    content = interactionContent.InteractionContent.Text;
                }
                if (!string.IsNullOrEmpty(content))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);
                    ChatTranscriptParser chatTranscriptParser = new ChatTranscriptParser();
                    chatTranscriptParser.GetChatSession(xmlDoc, mainRTB.Document);
                    mainRTB.Visibility = Visibility.Visible;
                }
                else
                {
                    if (mainRTB.Document != null && mainRTB.Document.Blocks != null)
                        mainRTB.Document.Blocks.Clear();
                }
            }
            catch (Exception ex)
            {
                if (mainRTB.Document != null && mainRTB.Document.Blocks != null)
                    mainRTB.Document.Blocks.Clear();
            }
        }

        private void GetDetailContentforEmail(EventGetInteractionContent eventGetInteractionContent)
        {
            try
            {
                RowFrom.Height = GridLength.Auto;
                RowTo.Height = GridLength.Auto;
                RowCC.Height = GridLength.Auto;
                RowBCC.Height = GridLength.Auto;
                dockEmailContent.Children.Clear();
                wpAttachments.Children.Clear();
                lblSubjectBold.Content = !string.IsNullOrEmpty(eventGetInteractionContent.InteractionAttributes.Subject) ? eventGetInteractionContent.InteractionAttributes.Subject : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("Subject") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["Subject"].ToString() : "(No Subject)";

                //txtCc.Text = (interactionContent.InteractionAttributes.AllAttributes.Contains("CcAddresses") && !string.IsNullOrEmpty(interactionContent.InteractionAttributes.AllAttributes["CcAddresses"].ToString()))? interactionContent.InteractionAttributes.AllAttributes["CcAddresses"].ToString() : hidedetails(RowCC);
                //txtBCc.Text = (interactionContent.InteractionAttributes.AllAttributes.Contains("BccAddresses") && !string.IsNullOrEmpty(interactionContent.InteractionAttributes.AllAttributes["BccAddresses"].ToString())) ? interactionContent.InteractionAttributes.AllAttributes["BccAddresses"].ToString() : hidedetails(RowBCC);

                txtEmailStartDate.Text = eventGetInteractionContent.InteractionAttributes.StartDate != null ? eventGetInteractionContent.InteractionAttributes.StartDate.ToString() : "";
                DateTime _time;
                if (DateTime.TryParse(txtEmailStartDate.Text, out _time))
                    txtEmailStartDate.Text = _time.ToString();

                BaseEntityAttributes baseEntityAttributes = eventGetInteractionContent.EntityAttributes;
                bool isEmailEnable = (ContactDataContext.GetInstance().IsEmailLogon && _configContainer.AllKeys.Contains("email.enable.plugin") && "true".Equals(((string)_configContainer.GetValue("email.enable.plugin")).ToLower())
                  && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email));
                switch (eventGetInteractionContent.InteractionAttributes.EntityTypeId.ToString().ToLower())
                {
                    case "emailin":
                        EmailInEntityAttributes emailInEntityAttributes = new EmailInEntityAttributes();
                        emailInEntityAttributes = (EmailInEntityAttributes)baseEntityAttributes;
                        txtFrom.Text = !string.IsNullOrEmpty(emailInEntityAttributes.FromAddress) ? emailInEntityAttributes.FromAddress : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("FromAddress") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["FromAddress"].ToString() : hidedetails(RowFrom);
                        txtTo.Text = !string.IsNullOrEmpty(emailInEntityAttributes.ToAddresses) ? emailInEntityAttributes.ToAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("To") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["To"].ToString() : hidedetails(RowTo);
                        txtCc.Text = !string.IsNullOrEmpty(emailInEntityAttributes.CcAddresses) ? emailInEntityAttributes.CcAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("CcAddresses") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["CcAddresses"].ToString() : hidedetails(RowCC);
                        txtBCc.Text = !string.IsNullOrEmpty(emailInEntityAttributes.BccAddresses) ? emailInEntityAttributes.BccAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("BccAddresses") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["BccAddresses"].ToString() : hidedetails(RowBCC);
                        if (isEmailEnable)
                        {
                            if (ContactDataContext.GetInstance().IsInteractionServerActive && ContactDataContext.GetInstance().IsContactServerActive)
                            {
                                gcOpen.Width = gcReply.Width = gcReplyAll.Width = GridLength.Auto;
                                if (selectedMailState.Equals("Done"))
                                    ShowInboundProgress(txtCc.Text != "");
                                else
                                    ShowInboundProgress(txtCc.Text != "");
                                gcResend.Width = new GridLength(0);
                            }
                            else
                                gcOpen.Width = gcReply.Width = gcReplyAll.Width = gcResend.Width = new GridLength(0);
                        }
                        break;
                    case "emailout":
                        EmailOutEntityAttributes emailOutEntityAttributes = new EmailOutEntityAttributes();
                        emailOutEntityAttributes = (EmailOutEntityAttributes)baseEntityAttributes;
                        txtFrom.Text = !string.IsNullOrEmpty(emailOutEntityAttributes.FromAddress) ? emailOutEntityAttributes.FromAddress : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("FromAddress") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["FromAddress"].ToString() : hidedetails(RowFrom);
                        txtTo.Text = !string.IsNullOrEmpty(emailOutEntityAttributes.FromAddress) ? emailOutEntityAttributes.ToAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("To") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["To"].ToString() : hidedetails(RowTo);
                        txtCc.Text = !string.IsNullOrEmpty(emailOutEntityAttributes.CcAddresses) ? emailOutEntityAttributes.CcAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("CcAddresses") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["CcAddresses"].ToString() : hidedetails(RowCC);
                        txtBCc.Text = !string.IsNullOrEmpty(emailOutEntityAttributes.BccAddresses) ? emailOutEntityAttributes.BccAddresses : eventGetInteractionContent.InteractionAttributes.AllAttributes.Contains("BccAddresses") ? eventGetInteractionContent.InteractionAttributes.AllAttributes["BccAddresses"].ToString() : hidedetails(RowBCC);
                        if (isEmailEnable)
                        {
                            if (ContactDataContext.GetInstance().IsInteractionServerActive && ContactDataContext.GetInstance().IsContactServerActive)
                            {
                                gcOpen.Width = gcReply.Width = gcReplyAll.Width = gcResend.Width = GridLength.Auto;
                                selectedContactID = eventGetInteractionContent.InteractionAttributes.AllAttributes["ContactId"] as string;
                                ShowOutbound();
                            }
                            else
                                gcOpen.Width = gcReply.Width = gcReplyAll.Width = gcResend.Width = new GridLength(0);
                        }
                        break;
                    default: break;
                }

                //Get Text
                string content = string.Empty;
                if (!String.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText))
                {
                    content = eventGetInteractionContent.InteractionContent.StructuredText;
                }
                else if (!String.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.Text))
                {
                    content = eventGetInteractionContent.InteractionContent.Text;
                    content = content.Replace("\r\n", "<br />");
                    content = content.Replace("\n", "<br />");
                }
                if (!String.IsNullOrEmpty(content))
                {
                    RowContent.MinHeight = 150;
                    HTMLEditor htmlEditor = new HTMLEditor(false, !String.IsNullOrEmpty(eventGetInteractionContent.InteractionContent.StructuredText), content);
                    dockEmailContent.Children.Add(htmlEditor);
                }
                else
                {
                    RowContent.MinHeight = 0;
                    dockEmailContent.Children.Clear();
                }

                if (eventGetInteractionContent.Attachments != null && eventGetInteractionContent.Attachments.Count > 0)
                {
                    foreach (Attachment attachment in eventGetInteractionContent.Attachments)
                    {
                        wpAttachments.Children.Add(LoadAttachments(attachment.TheName, attachment.TheSize.ToString(), attachment.DocumentId));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
        }

        private void GetDetailContentforVoice(EventGetInteractionContent interactionContent)
        {
            try
            {
                RowDate.Height = GridLength.Auto;
                RowPhoneNumber.Height = GridLength.Auto;
                RowDuration.Height = GridLength.Auto;
                RowConatct.Height = GridLength.Auto;
                BaseEntityAttributes baseEntityAttributes = interactionContent.EntityAttributes;
                PhoneCallEntityAttributes phoneCallEntityAttributes = new PhoneCallEntityAttributes();
                phoneCallEntityAttributes = (PhoneCallEntityAttributes)baseEntityAttributes;
                txtVoiceDate.Text = interactionContent.InteractionAttributes.StartDate != null ? interactionContent.InteractionAttributes.StartDate.ToString().GetLocalDateTime() : hidedetails(RowDate);
                txtVoiceContact.Text = interactionContent.InteractionAttributes.ContactId == null ? hidedetails(RowConatct) : !string.IsNullOrEmpty(GetContactFirstNameLastName(interactionContent.InteractionAttributes.ContactId)) ? GetContactFirstNameLastName(interactionContent.InteractionAttributes.ContactId) : hidedetails(RowConatct);
                txtVoicePhoneNo.Text = !string.IsNullOrEmpty(phoneCallEntityAttributes.PhoneNumber) ? phoneCallEntityAttributes.PhoneNumber : hidedetails(RowPhoneNumber);
                //if (!string.IsNullOrEmpty(txtVoicePhoneNo.Text))
                //    txtToolTipCall.Text = "Make call to " + txtVoicePhoneNo.Text;
                //else
                //    gcCall.Width = new GridLength(0);
                txtVoiceDuration.Text = phoneCallEntityAttributes.Duration != null ? ConvertSecondsToHHMMSS(Convert.ToInt16(phoneCallEntityAttributes.Duration.ToString())) : hidedetails(RowDuration);

                lblSubjectBold.Content = "Phone call from ";
                lblSubjectBold.Content += !string.IsNullOrEmpty(txtVoiceContact.Text) ? txtVoiceContact.Text : string.Empty;
                lblSubjectBold.Content += " (" + (phoneCallEntityAttributes.PhoneNumber != null ? phoneCallEntityAttributes.PhoneNumber : string.Empty) + ")";

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
        }

        private string GetFullNameOfAgent(string empId)
        {
            string name = string.Empty;
            CfgPerson person = null;
            try
            {
                CfgPersonQuery personQuery = new CfgPersonQuery();
                personQuery.EmployeeId = empId;
                personQuery.TenantDbid = _configContainer.TenantDbId;
                person = (CfgPerson)_configContainer.ConfServiceObject.RetrieveObject(personQuery);
                if (person != null)
                {
                    if (!string.IsNullOrEmpty(person.FirstName))
                        name = person.FirstName;
                    if (!string.IsNullOrEmpty(person.LastName))
                        name = (string.IsNullOrEmpty(person.LastName) ? "" : name + " ") + person.LastName;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as : " + ex.Message);
            }
            if (string.IsNullOrEmpty(name))
                name = "Unidentified Agent";
            return name;
        }

        private HistoryCriteria GetMediaFilter()
        {
            return new HistoryCriteria() { AttributeName = "MediaTypeId", AttributeValue = filteredMediaType, ComparisonOperator = Operators.Equal, Prefixes = Prefixes.And };
        }

        private string GetPropertyValue<T>(string propertyName, T obj)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propertyInfo == null) goto end;
            if (propertyInfo.CanRead)
                return Convert.ToString(propertyInfo.GetValue(obj, null));
        end:
            return null;
        }

        private List<HistoryCriteria> GetSearchCriteria()
        {
            List<HistoryCriteria> searchCriteria = new List<HistoryCriteria>();
            bool iscontainDateFilter = false;
            searchCriteria.Add(GetBasicIDCriteria());
            if (CustomSlider.Value > 25)
            {
                searchCriteria.Add(GetCustomTimeSlider());
                iscontainDateFilter = true;
            }
            if (filteredMediaType != "all")
                searchCriteria.Add(GetMediaFilter());
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                DateTime dt = DateTime.Now;
                if (DateTime.TryParse(txtSearch.Text, out dt) && !iscontainDateFilter)
                {
                    string UTCDateTime = dt.Year + "-" + dt.Month + "-" + dt.Day + "T00:00:00.0000000Z";
                    searchCriteria.Add(new HistoryCriteria() { AttributeName = "StartDate", AttributeValue = UTCDateTime, ComparisonOperator = Operators.GreaterOrEqual, Prefixes = Prefixes.And });
                }

                if ("in progress".Equals(txtSearch.Text.ToLower()))
                    searchCriteria.Add(new HistoryCriteria() { AttributeName = "Status", AttributeValue = "2", ComparisonOperator = Operators.Equal, Prefixes = Prefixes.And });
                else if ("done".Equals(txtSearch.Text.ToLower()))
                    searchCriteria.Add(new HistoryCriteria() { AttributeName = "Status", AttributeValue = "3", ComparisonOperator = Operators.Equal, Prefixes = Prefixes.And });
                else
                {
                    searchCriteria.Add(new HistoryCriteria() { AttributeName = "Subject", AttributeValue = txtSearch.Text + "*", ComparisonOperator = Operators.Like, Prefixes = Prefixes.And });
                    //searchCriteria.Add(new HistoryCriteria() { AttributeName = "ParentId", AttributeValue = txtSearch.Text + "*", ComparisonOperator = Operators.Like, Prefixes = Prefixes.And });

                }

            }
            return searchCriteria;
        }

        private void GetSelectedHistory()
        {
            if (end > dtContactHistory.Rows.Count)
                end = dtContactHistory.Rows.Count;
            DataTable temp = dtContactHistory.Clone();

            for (int index = start; index < end; index++)
            {
                temp.ImportRow(dtContactHistory.Rows[index]);
            }

            temp = temp.DefaultView.ToTable();

            if (temp.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "MediaTypeId").ToList().Count > 0)
                temp.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "MediaTypeId").SingleOrDefault().ColumnName = "media";
            //TypeId
            if (temp.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "TypeId").ToList().Count > 0)
                temp.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "TypeId").SingleOrDefault().ColumnName = "State";

            txtStart.Text = dtContactHistory.Rows.Count > 0 ? "" + (start + 1) : "0";
            txtEnd.Text = dtContactHistory.Rows.Count > 0 ? "" + end : "0";
            btnNextPage.IsEnabled = dtContactHistory.Rows.Count > end || isHasNext;
            btnPreviousPage.IsEnabled = btnFirst.IsEnabled = start != 0;
            dgContactHistory.DataContext = temp;
            if (temp.Rows.Count > 0)
            {
                //if (temp.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "Subject").ToList().Count > 0)
                //    dgContactHistory.Columns.Where(x => "Subject".Equals((x.Header as string))).SingleOrDefault().Width =
                //        new Microsoft.Windows.Controls.DataGridLength(1, Microsoft.Windows.Controls.DataGridLengthUnitType.Star);

                dgContactHistory.SelectedIndex = 0;
                txtError.Visibility = Visibility.Collapsed;
                if (historyType == HistoryType.MyHistory && _configContainer.AllKeys.Contains("contact.history.enable.export")
                        && _configContainer.GetAsBoolean("contact.history.enable.export"))
                    Btn_Export.Visibility = Visibility.Visible;
            }
            else
            {
                Btn_Export.Visibility = Visibility.Collapsed;
                txtError.Visibility = Visibility.Visible;

            }
        }

        private string hidedetails(RowDefinition row)
        {
            row.Height = new GridLength(0);
            return "";
        }

        private void HideEmailButton()
        {
            gcOpen.Width = gcReply.Width = gcReplyAll.Width = gcResend.Width = new GridLength(0);
        }

        private void HideMessage()
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                grMessage.Height = new GridLength(0);
                this.IsEnabled = true;
                btnCloseError.Visibility = Visibility.Visible;
            }));
        }

        private void HideProgress()
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Collapsed;
                gridPreload.Visibility = System.Windows.Visibility.Collapsed;
                Panel.SetZIndex(BrdOpacity, 0);
                Panel.SetZIndex(gridPreload, 0);
                ContactDataContext.GetInstance().PreloadImage = null;
            }));
        }

        private List<MenuItem> HistoryMediaFilters()
        {
            var temp = new List<MenuItem>();
            List<string> items = ((string)_configContainer.GetValue("contact.history.media-filters")).Split(',').ToList();
            //availableMedia.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (string key in items)
                {
                    //temp.Add(menuItem(key));
                    switch (key.ToLower().Trim())
                    {
                        case "voice":
                            temp.Add(menuItem(key.ToLower().Trim()));
                            break;
                        case "email":
                            temp.Add(menuItem(key.ToLower().Trim()));
                            break;
                        case "chat":
                            temp.Add(menuItem(key.ToLower().Trim()));
                            break;
                        default:
                            break;
                    }
                }
            }

            if (temp.Count == 0)
            {
                temp.Add(menuItem("voice"));
                temp.Add(menuItem("email"));
                temp.Add(menuItem("chat"));
            }

            if (temp.Count > 1)
                temp.Insert(0, menuItem("all"));
            filteredMediaType = temp[0].Tag.ToString();

            return temp;
        }

        private void lblError_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
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

            return btnAttachment;
        }

        private void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            mainRTB.ScrollToEnd();
        }

        private MenuItem menuItem(string mnuName)
        {
            //if (!mnuName.Equals("all"))
            //    availableMedia.Add(mnuName);
            var menuMediaFilter = new MenuItem();
            menuMediaFilter.Margin = new Thickness(2);
            menuMediaFilter.Header = "Show " + mnuName + " interactions";
            menuMediaFilter.Tag = mnuName;
            menuMediaFilter.Click += new RoutedEventHandler(menuMediaFilter_Click);
            return menuMediaFilter;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as System.Windows.Controls.MenuItem;
            FileStream fs = null;
            try
            {
                switch (menuitem.Header.ToString().ToLower())
                {
                    case "open":
                        getAttachDocument = GetDocumentContent(((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString());
                        if (getAttachDocument != null)
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp\" + getAttachDocument.Id.ToString() + @"\";
                            logger.Info("Opening the file : " + getAttachDocument.TheName);
                            if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(getAttachDocument.TheName)))
                                getAttachDocument.TheName = path + @"\" + getAttachDocument.TheName;
                            logger.Info("Creating the file : " + getAttachDocument.TheName);
                            if (!Directory.Exists(System.IO.Path.GetDirectoryName(getAttachDocument.TheName)))
                                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(getAttachDocument.TheName));
                            using (fs = new FileStream(getAttachDocument.TheName, FileMode.Create)) { }
                            File.WriteAllBytes(getAttachDocument.TheName, getAttachDocument.Content);
                            var process = Process.Start(getAttachDocument.TheName);
                            process.EnableRaisingEvents = true;
                            process.Exited += new EventHandler(process_Exited);
                        }
                        break;
                    case "save":
                        getAttachDocument = GetDocumentContent(((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString());
                        if (getAttachDocument != null)
                        {
                            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                            saveDialog.FileName = getAttachDocument.TheName;
                            _fileExtension = System.IO.Path.GetExtension(getAttachDocument.TheName);
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
                logger.Error("Error occurred while " + menuitem.Header.ToString().ToLower() + " the attachment '" + menuitem.Header.ToString().Trim() + "'  as: " + generalException.ToString());
            }
            finally
            {
                fs = null;
                //GC.Collect();
            }
        }

        private void menuMediaFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                filteredMediaType = "all";
                if (menuitem.Header.ToString().Contains("chat"))
                    filteredMediaType = "chat";
                else if (menuitem.Header.ToString().Contains("email"))
                    filteredMediaType = "email";
                else if (menuitem.Header.ToString().Contains("voice"))
                    filteredMediaType = "voice";
                else if (menuitem.Header.ToString().Contains("sms"))
                    filteredMediaType = "sms";
                if (_mediaFilterContextMenu.Items.Cast<MenuItem>().Where(x => x.Icon != null).ToList().Count > 0)
                    _mediaFilterContextMenu.Items.Cast<MenuItem>().Where(x => x.Icon != null).SingleOrDefault().Icon = null;

                menuitem.Icon = new Image
                {
                    Height = 15,
                    Width = 15,
                    Source =
                        new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Contact\\TickBlack.png", UriKind.Relative))
                };
                CustomSlider_ValueChanged(null, null);
                //if (!txtSearch.IsEnabled && _advanceSearch != null)
                if (grsearch.Height != GridLength.Auto && _advanceSearch != null)
                    _advanceSearch.Search_Click(null, null);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void MnuExportCSV_Click(object sender, RoutedEventArgs e)
        {
            Thread exportThread = new Thread(new ThreadStart(delegate()
            {
                try
                {
                    if (!_configContainer.AllKeys.Contains("contact.history.export-attribute") &&
                        !_configContainer.AllKeys.Contains("contact.history.export-case-data"))
                    {
                        ShoWMessage("No configuration available to export data.", true);
                        return;
                    }
                    var firstname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).FirstName;
                    var lastname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).LastName;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = ".csv";
                    sfd.Filter = "CSV files (*.csv)|*.csv";
                    sfd.FileName = firstname + (string.IsNullOrEmpty(firstname) ? "" : "_") + lastname
                        + DateTime.Now.ToString("g").Replace(" ", "").Replace("/", "").Replace(":", "");
                    if (sfd.ShowDialog() == true)
                    {
                        string filePath = sfd.FileName;
                        ShowProgress("\\Agent.Interaction.Desktop;component\\Images\\Contact\\Build-csv2.gif", "Exporting data...");
                        List<ExportData> lstExportData = GetDataToExport();
                        DataExportHelper.ExportAndSaveCSV(lstExportData, sfd.FileName);
                        HideProgress();
                        Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            lblError.Text = "Data Exported Successfully.";
                            StartTimerForError(true);
                        }));
                    }
                }
                catch (Exception ex)
                {

                    ShoWMessage("Error occurred while export data.", true);
                    logger.Error("Error occurred while export data as " + ex.Message);
                    HideProgress();
                }
            }));
            exportThread.Start();
        }

        private void MnuExportExcel_Click(object sender, RoutedEventArgs e)
        {
            Thread exportThread = new Thread(new ThreadStart(delegate()
            {
                try
                {
                    if (!_configContainer.AllKeys.Contains("contact.history.export-attribute") &&
                        !_configContainer.AllKeys.Contains("contact.history.export-case-data"))
                    {
                        ShoWMessage("No configuration available to export data.", true);
                        return;
                    }
                    var firstname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).FirstName;
                    var lastname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).LastName;
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = ".xls";
                    sfd.Filter = "Excel 97-2003 (.xls)|*.xls";
                    sfd.FileName = firstname + (string.IsNullOrEmpty(firstname) ? "" : "_") + lastname
                        + DateTime.Now.ToString("g").Replace(" ", "").Replace("/", "").Replace(":", "");
                    if (sfd.ShowDialog() == true)
                    {
                        string filePath = sfd.FileName;
                        ShowProgress("\\Agent.Interaction.Desktop;component\\Images\\Contact\\Build-excel2.gif", "Exporting data...");
                        List<ExportData> lstExportData = GetDataToExport();
                        DataExportHelper.ExportAndSaveExcell(lstExportData, sfd.FileName);
                        HideProgress();
                        Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            lblError.Text = "Data Exported Successfully.";
                            StartTimerForError(true);
                        }));
                    }
                }
                catch (Exception ex)
                {

                    ShoWMessage("Error occurred while export data.", true);
                    logger.Error("Error occurred while export data as " + ex.Message);
                    HideProgress();
                }
            }));
            exportThread.Start();
        }

        private void MnuExportPDF_Click(object sender, RoutedEventArgs e)
        {
            //Thread exportThread = new Thread(new ThreadStart(delegate()
            //{
            try
            {
                if (!_configContainer.AllKeys.Contains("contact.history.export-attribute") &&
                     !_configContainer.AllKeys.Contains("contact.history.export-case-data"))
                {
                    ShoWMessage("No configuration available to export data.", true);
                    return;
                }
                var firstname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).FirstName;
                var lastname = ((CfgPerson)_configContainer.GetValue("CfgPerson")).LastName;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".pdf";
                sfd.Filter = "PDF document|*.pdf";
                sfd.FileName = firstname + (string.IsNullOrEmpty(firstname) ? "" : "_") + lastname
                    + DateTime.Now.ToString("g").Replace(" ", "").Replace("/", "").Replace(":", "");
                if (sfd.ShowDialog() == true)
                {
                    string filePath = sfd.FileName;
                    ShowProgress("\\Agent.Interaction.Desktop;component\\Images\\Contact\\Build-pdf2.gif", "Exporting data...");
                    List<ExportData> lstExportData = GetDataToExport();
                    DataExportHelper.ExportAndSavePDF(lstExportData, sfd.FileName);
                    HideProgress();
                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        lblError.Text = "Data Exported Successfully.";
                        StartTimerForError(true);
                    }));
                }

            }
            catch (Exception ex)
            {
                ShoWMessage("Error occurred while export data.", true);
                logger.Error("Error occurred while export data as " + ex.Message);
                HideProgress();
            }
            //}));

            //exportThread.Start();
        }

        private void NotifyWorkbinChangeEvent(string interactionId)
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    try
                    {
                        if (interactionId.Equals(selectedinteractionId))
                            dgContactHistory_SelectionChanged(null, null);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occcurred as " + ex.Message);
                    }
                }));
        }

        private void PerformBasicSearch()
        {
            ShowProgress();
            SearchHistory(GetSearchCriteria(), CustomSlider.Value == 0);
            GetSelectedHistory();
            HideProgress();
        }

        private void PerformHistoryIntitalLoad()
        {
            if (!ContactDataContext.GetInstance().IsContactServerActive)
            {
                ShoWMessage("Contact server is not active. Please contact your administrator.", false);
                return;
            }
            else
            {
                HideMessage();
            }
            if (historyType == HistoryType.None) return;
            ShowProgress();
            CreateMediaColumnWithImages();
            List<HistoryCriteria> searchCriteria = new List<HistoryCriteria>();
            searchCriteria.Add(GetBasicIDCriteria());
            if (CustomSlider.Value > 25) //!=
            {
                searchCriteria.Add(GetCustomTimeSlider());
            }
            if (filteredMediaType != "all")
                searchCriteria.Add(GetMediaFilter());
            SearchHistory(searchCriteria, CustomSlider.Value == 0);
            GetSelectedHistory();
            HideProgress();
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

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileName = (sender as System.Windows.Forms.SaveFileDialog).FileName;
                if (getAttachDocument != null)
                {
                    File.WriteAllBytes((string.IsNullOrEmpty(fileName) ? (getAttachDocument.TheName + (getAttachDocument.TheName.EndsWith(_fileExtension) ? "" : _fileExtension)) : fileName), getAttachDocument.Content);
                    getAttachDocument = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while save File Dialog as : " + ex.Message);
            }
        }

        private void SearchHistory(List<HistoryCriteria> searchCriteria, bool isArchive, bool needRecreateTable = true, List<HistoryCriteria> advanceSearchCriteria = null, bool isMatchAllCondition = false)
        {
            if (ContactDataContext.GetInstance().IsInteractionIndexFound && historyType == HistoryType.MyHistory && !isArchive)
            {
                btnAdvanceSearch.IsEnabled = brdSearchBar.IsEnabled = true;
                ContactHandler contactHandler = new ContactHandler();
                int maxcount = 50;
                if (Convert.ToInt32(cmbItemsPerPage.SelectedValue) > 50)
                    maxcount = Convert.ToInt32(cmbItemsPerPage.SelectedValue);

                List<string> lstAttribute = new List<string>(listHistoryDisplayedColoumns);
                lstAttribute.Add("MediaTypeId");
                lstAttribute.Add("TypeId");
                lstAttribute.Add("SubtypeId");
                lstAttribute.Add("InteractionId");
                int startingIndex = 0;
                lstAttribute = lstAttribute.Distinct().ToList();
                if (!needRecreateTable)
                {
                    maxcount += int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    startingIndex = dtContactHistory.Rows.Count;
                }
                else
                {
                    dtContactHistory = new DataTable();
                    dtContactHistory.Columns.Clear();
                    start = 0;
                    end += int.Parse(cmbItemsPerPage.SelectedValue.ToString());
                    for (int index = 0; index < lstAttribute.Count; index++)
                        dtContactHistory.Columns.Add(lstAttribute[index]);
                }

                IMessage result = contactHandler.SearchContact(FormSearchQuery(searchCriteria, advanceSearchCriteria, isMatchAllCondition), maxcount, "interaction");
                if (result != null)
                {
                    EventSearch eventSearch = result as EventSearch;
                    isHasNext = eventSearch.Documents != null && eventSearch.FoundDocuments.Value > eventSearch.Documents.Count;

                    if (eventSearch.Documents != null && eventSearch.Documents.Count > 0)
                    {
                        dtContactHistory.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "InteractionId").SingleOrDefault().ColumnName = "Id";
                        for (int index = startingIndex; index < eventSearch.Documents.Count && index < 100; index++)
                        {
                            DocumentData docData = eventSearch.Documents[index];
                            DataRow historyRow = dtContactHistory.NewRow();
                            for (int columnIndex = 0; columnIndex < dtContactHistory.Columns.Count; columnIndex++)
                            {
                                if (docData.Fields.AllKeys.Contains(dtContactHistory.Columns[columnIndex].ColumnName))
                                {

                                    if (dtContactHistory.Columns[columnIndex].ColumnName == "Status")
                                    {
                                        var status = docData.Fields[dtContactHistory.Columns[columnIndex].ColumnName].ToString();
                                        if (Convert.ToInt32(status) <= 2)
                                            historyRow["Status"] = "In Progress";
                                        else if ("3" == status)
                                            historyRow["Status"] = "Done";
                                    }
                                    else if ("StartDate".Equals(dtContactHistory.Columns[columnIndex].ColumnName) || "EndDate".Equals(dtContactHistory.Columns[columnIndex].ColumnName))
                                    {
                                        historyRow[dtContactHistory.Columns[columnIndex].ColumnName] = docData.Fields[dtContactHistory.Columns[columnIndex].ColumnName].ToString().GetLocalDateTime();
                                    }
                                    else
                                        historyRow[dtContactHistory.Columns[columnIndex].ColumnName] = docData.Fields[dtContactHistory.Columns[columnIndex].ColumnName].ToString();
                                }
                            }
                            dtContactHistory.Rows.Add(historyRow);
                        }
                        dtContactHistory.Columns.Cast<DataColumn>().Where(x => x.ColumnName == "Id").SingleOrDefault().ColumnName = "InteractionId";
                    }
                    else
                        GetSelectedHistory();
                    return;
                }
                else
                {
                    ContactDataContext.GetInstance().IsInteractionIndexFound = false;

                    // Added code by sakthikumar to hide if the advance search is visible.
                    if (isExpand)
                    {
                        ExpandGrid.Height = new GridLength(1, GridUnitType.Star);
                        DetailsGrid.Height = new GridLength(0);
                        isExpand = false;
                    }
                }
            }
            getContactHistory(FormSearchCriteria(searchCriteria, isArchive), needRecreateTable, isArchive);
        }

        private void SelectAll(TextBox tb)
        {
            Keyboard.Focus(tb);
            tb.SelectAll();
        }

        private void ShowInboundDone(bool isReplyALL)
        {
            gcResend.Width = gcOpen.Width = new GridLength(0);
            gcReply.Width = GridLength.Auto;
            gcReplyAll.Width = isReplyALL ? GridLength.Auto : new GridLength(0);
        }

        private void ShowInboundProgress(bool isReplyALL)
        {
            if (IsOpenVisible && dgContactHistory.SelectedItem != null &&
                _configContainer.AllKeys.Contains("email.enable.pull-from-history")
                && "true".Equals(((string)_configContainer.GetValue("email.enable.pull-from-history")).ToLower())
                && !ContactDataContext.GetInstance().OpenedMailIds.Contains(selectedinteractionId))
                gcOpen.Width = GridLength.Auto;
            else
                gcResend.Width = gcOpen.Width = new GridLength(0);

            gcReply.Width = GridLength.Auto;
            gcReplyAll.Width = isReplyALL ? GridLength.Auto : new GridLength(0);
        }

        private void ShoWMessage(string Message, bool iscontrolEnable)
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                grMessage.Height = GridLength.Auto;
                lblError.Text = Message;
                this.IsEnabled = iscontrolEnable;
                if (IsEnabled)
                    btnCloseError.Visibility = Visibility.Visible;
                else
                    btnCloseError.Visibility = Visibility.Collapsed;
            }));
        }

        private void ShowOutbound()
        {
            if (selectedMailState.Equals("Done"))
            {
                if (IsOpenVisible)
                    gcOpen.Width = new GridLength(0);
                gcResend.Width = GridLength.Auto;
            }
            else
            {
                gcResend.Width = new GridLength(0);
                if (IsOpenVisible && dgContactHistory.SelectedItem != null &&
                     _configContainer.AllKeys.Contains("email.enable.pull-from-history")
                && "true".Equals(((string)_configContainer.GetValue("email.enable.pull-from-history")).ToLower())
                    && !ContactDataContext.GetInstance().OpenedMailIds.Contains(selectedinteractionId))
                    gcOpen.Width = GridLength.Auto;
                else
                    gcOpen.Width = new GridLength(0);
            }
            gcReply.Width = gcReplyAll.Width = new GridLength(0);
        }

        private void ShowProgress(string imgPath = "\\Agent.Interaction.Desktop;component\\Images\\Loading.gif", string progressText = "Loading...")
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                BrdOpacity.Visibility = System.Windows.Visibility.Visible;
                gridPreload.Visibility = System.Windows.Visibility.Visible;
                Panel.SetZIndex(BrdOpacity, 1);
                Panel.SetZIndex(gridPreload, 2);
                ContactDataContext.GetInstance().PreloadImage = imgPath;
                txtLoadingProgress.Text = progressText;
            }));
        }

        void StartTimerForError(bool isSuccess = false)
        {
            try
            {
                if (isSuccess)
                    imgStatusImage.Source = new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Done 64x64.png", UriKind.Relative));
                else
                    imgStatusImage.Source = new BitmapImage(new Uri(_configContainer.GetValue("image-path") + "\\Error.png", UriKind.Relative));
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

        private void SubscribeNotificationEvent()
        {
            ContactHandler.WorkbinChangedEvent += new WorkbinChangedEvent(NotifyWorkbinChangeEvent);
            ContactHandler.ContactServerNotificationHandler += new ContactServerNotificationHandler(ContactServerNotification);
            ContactHandler.InteractionServerNotificationHandler += new InteractionServerNotificationHandler(ContactHandler_InteractionServerNotificationHandler);
        }

        /// <summary>
        /// Handles the Loaded event of the tapCaseInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void tapCaseInfo_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void txtFrom_GotMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {
                SelectAll(sender as TextBox);

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e != null && e.Key == Key.Enter)
                {
                    if (dtContactHistory != null)
                        dtContactHistory.Rows.Clear();
                    PerformBasicSearch();
                    GetSelectedHistory();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void UnSubcribeNotificationEvent()
        {
            ContactHandler.WorkbinChangedEvent -= NotifyWorkbinChangeEvent;
            ContactHandler.ContactServerNotificationHandler -= ContactServerNotification;
            ContactHandler.InteractionServerNotificationHandler -= ContactHandler_InteractionServerNotificationHandler;
            ContactHandler.EmailStateNotificationEvent -= ContactHandler_EmailStateNotificationEvent;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    this.DataContext = ContactDataContext.GetInstance();

                    ShowProgress();
                    logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
                    dtContactHistory = new DataTable();
                    CustomSlider.ValueChanged -= CustomSlider_ValueChanged;
                    CustomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(CustomSlider_ValueChanged);
                    UnSubcribeNotificationEvent();
                    SubscribeNotificationEvent();
                    BindNeccessaryValues();
                    CreateTimeSlideValue();
                    PerformHistoryIntitalLoad();
                    this.cmbItemsPerPage.SelectionChanged -= this.cmbItemsPerPage_SelectionChanged;
                    this.cmbItemsPerPage.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cmbItemsPerPage_SelectionChanged);
                    HideProgress();

                    //Code to determine the export button view dacision.
                    if (historyType == HistoryType.MyHistory && _configContainer.AllKeys.Contains("contact.history.enable.export")
                        && _configContainer.GetAsBoolean("contact.history.enable.export"))
                        Btn_Export.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while history load as " + ex.Message);
                }
            }));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UnSubcribeNotificationEvent();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        #endregion Methods
    }

    public static class DateToStringOperation
    {
        #region Methods

        public static string GetLocalDateTime(this string datetime)
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = enUS.DateTimeFormat;
            DateTime _time;
            if (DateTime.TryParse(datetime, null, System.Globalization.DateTimeStyles.AssumeLocal, out _time))
                return _time.ToString(dtfi);
            else
                return datetime;
        }

        #endregion Methods
    }
}