using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interaction.Contact.Settings;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Settings;
using Pointel.Windows.Views.Common.Editor.Controls;

namespace Pointel.Interactions.Contact.Controls
{
    /// <summary>
    /// Interaction logic for ContactResponse.xaml
    /// </summary>
    public partial class ContactResponse : UserControl
    {
        #region private fields
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        public string currentResponse = string.Empty;
        public string currentResponseName = string.Empty;
        StackPanel stackPanel = null;

        Image imgclose = null;
        System.Windows.Controls.Label txtBlock = null;
        ContactDataContext contactDataContext = ContactDataContext.GetInstance();
        string stdResponseDesc = null;

        public delegate string EventResponseNotification(string plaintext, string response, string subject, string name, AttachmentList selectedAttachments);
        public event EventResponseNotification eventResponseNotify;
        // System.Windows.Forms.WebBrowser txtContent = new System.Windows.Forms.WebBrowser();
        //RichTextBox RTB = new RichTextBox();
        string initialValue = null;
        string[] value = null;
        List<Pointel.Interactions.IPlugins.IWorkbinPlugin> Contactplugins = new List<Pointel.Interactions.IPlugins.IWorkbinPlugin>();
        List<Pointel.Interactions.IPlugins.IEmailPlugin> ContactpluginInteraction = new List<Pointel.Interactions.IPlugins.IEmailPlugin>();
        List<Pointel.Interactions.IPlugins.IChatPlugin> ContactpluginChat = new List<Pointel.Interactions.IPlugins.IChatPlugin>();

        //WindowsFormsHost host = new WindowsFormsHost();
        //bool isEnableResponseWebHost = true;
        string url = string.Empty;
        bool isAdvancedResponseSearch = false;
        #endregion

        int _responseCount = 0;
        string responseId = "";
        EventGetAllCategories _allCategories;
        StandardResponseList _standardResponseList = new StandardResponseList();
        private string _fileExtension = string.Empty;
        private EventGetDocument getAttachDocument = null;
        private EventGetAgentStdRespFavorites _favoriteCategories;
        private bool _isFavoriteResponseClicked = false;
        private List<string> _favoriteResponseIdList = new List<string>();
        bool isResponseNameChecked = false;
        bool isResponseBodyChecked = false;
        AttachmentList _selectedAttachments = null;
        private HTMLEditor htmlEditor = null;
        private Func<string, string, string, string, AttachmentList, string> _refFunction;
        public ContactResponse(bool isEnableWebHost, Func<string, string, string, string, AttachmentList, string> refFunction)
        {
            InitializeComponent();
            //isEnableResponseWebHost = isEnableWebHost;
            btnAddResponse.Visibility = System.Windows.Visibility.Collapsed;
            btnAddResponseCompose.Visibility = System.Windows.Visibility.Collapsed;
            btnRemoveResponse.Visibility = System.Windows.Visibility.Collapsed;
            _refFunction = refFunction;
            LoadImage();
            loadHistorySearchDefaultAttributes();
            if (eventResponseNotify != null)
                eventResponseNotify = null;
            eventResponseNotify += new EventResponseNotification(_refFunction);
        }

        void UnLoadEvent(dynamic method)
        {

        }

        void LoadImage()
        {
            try
            {
                ImageDataContext.GetInstance().NewEmailIconImageFolder = null;
                ImageDataContext.GetInstance().NewEmailMsg = null;
                ImageDataContext.GetInstance().EmailIconImageSourceWhiteText = null;
                ImageDataContext.GetInstance().SaveMailInfo = null;
                ImageDataContext.GetInstance().EMailIconImageSourceExpand = null;
                ImageDataContext.GetInstance().ResetMailInfo = null;
                ImageDataContext.GetInstance().EmailIconImageSourceUP = null;
                ImageDataContext.GetInstance().MarkDoneMailMsg = null;
                ImageDataContext.GetInstance().EmailIconImageSourceDown = null;
                ImageDataContext.GetInstance().ConsultMailMsg = null;
                ImageDataContext.GetInstance().EMailIconImageSourceCollapse = null;
                ImageDataContext.GetInstance().ConsultMailMsg = null;

                this.DataContext = ImageDataContext.GetInstance();

                ImageDataContext.GetInstance().NewEmailIconImageFolder = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Contact-01.png", UriKind.Relative));
                ImageDataContext.GetInstance().NewEmailMsg = "ComposeMail";
                ImageDataContext.GetInstance().EmailIconImageSourceWhiteText = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\ViewStandardResponse-01.png", UriKind.Relative));
                ImageDataContext.GetInstance().SaveMailInfo = "Save";
                ImageDataContext.GetInstance().EMailIconImageSourceExpand = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Detailed.png", UriKind.Relative));
                ImageDataContext.GetInstance().ResetMailInfo = "Reset";
                ImageDataContext.GetInstance().EmailIconImageSourceUP = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\HideAdvanceSearchOption-01.png", UriKind.Relative));
                ImageDataContext.GetInstance().MarkDoneMailMsg = "MarkDone";
                ImageDataContext.GetInstance().EmailIconImageSourceDown = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\DoubleArrow.png", UriKind.Relative));
                ImageDataContext.GetInstance().ConsultMailMsg = "Consult";
                ImageDataContext.GetInstance().EMailIconImageSourceCollapse = new BitmapImage(new Uri(ConfigContainer.Instance().GetValue("image-path") + "\\Contact\\Limited.png", UriKind.Relative));
                ImageDataContext.GetInstance().ConsultMailMsg = "Consult";
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }



        }

        public void loadHistorySearchDefaultAttributes()
        {
            char[] c = { ',' };
            try
            {
                string historySearchAttributess = "Contains";
                if (historySearchAttributess != string.Empty)
                {
                    initialValue = historySearchAttributess;
                    value = initialValue.Split(c);
                    var e1 = from s in value select s;
                    int c1 = e1.Count();
                    cmbFrom.Items.Clear();
                    for (int i = 0; i < c1; i++)
                    {
                        if (value[i] != "")
                            cmbFrom.Items.Add(value[i].ToString() + " Search");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
            finally
            {
                c = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //btnContactExpand.IsEnabled = false;
            ExpandGridAuto.Height = new GridLength(0);
            TreeView1.Items.Clear();
            //contactDataContext.contactResponse.Clear();
            ResponseExpand.Height = new GridLength(0);

            btnAddResponse.Width = 25;
            btnAddResponseCompose.Width = 25;
            OutputValues outputMsg = Pointel.Interactions.Contact.Core.Request.RequestGetAllResponse.GetResponseContent(ConfigContainer.Instance().TenantDbId);
            if (outputMsg.IContactMessage != null)
            {
                CategoryList categoryList = null;
                EventGetAllCategories eventGetAllCategories = (EventGetAllCategories)outputMsg.IContactMessage;
                _allCategories = eventGetAllCategories;
                if (eventGetAllCategories.SRLContent != null)
                {
                    logger.Info("AllCatageories " + eventGetAllCategories.SRLContent.ToString());
                    categoryList = eventGetAllCategories.SRLContent;
                    LoadResponses(categoryList);
                }
                else
                {

                }

            }
            if (grd_ResponseContent.Children.Contains(htmlEditor))
                grd_ResponseContent.Children.Remove(htmlEditor);
            //if (isEnableResponseWebHost)
            //{
            //    txtContent.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(txtContent_DocumentCompleted);
            //    txtContent.IsWebBrowserContextMenuEnabled = false;
            //    txtContent.WebBrowserShortcutsEnabled = false;
            //    txtContent.Height = 140;
            //    host.Child = txtContent;
            //    dockwebbrowser.Children.Add(host);
            //}
            //else
            //{
            //    RTB.BorderBrush = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString("#D6D7D6"));
            //    RTB.BorderThickness = new Thickness(1);
            //    RTB.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //    RTB.IsDocumentEnabled = true;
            //    RTB.IsReadOnly = true;
            //    RTB.Height = 140;
            //    RTB.MaxHeight = 200;
            //    RTB.Document.Blocks.Clear();
            //    dockwebbrowser.Children.Add(RTB);
            //}
            btnDownload.Source = ImageDataContext.GetInstance().EmailIconImageSourceDown;
            btnContact.Source = ImageDataContext.GetInstance().EMailIconImageSourceCollapse;
            ExpandContent.Text = "Show details panel";
            ExpandHeading.Text = "Expand";
            txtSubject.Visibility = System.Windows.Visibility.Collapsed;
            ExpandGridAuto.Height = new GridLength(0);
            btnContactExpand.IsEnabled = false;
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
                return "0 Bytes";
        }

        //private void txtContent_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        //{
        //    System.Windows.Forms.HtmlElementCollection links = null;
        //    try
        //    {
        //        links = txtContent.Document.Links;
        //        foreach (System.Windows.Forms.HtmlElement var in links)
        //        {
        //            var.AttachEventHandler("onclick", LinkClicked);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
        //    }
        //    finally
        //    {
        //        links = null;
        //    }
        //}

        //private void LinkClicked(object sender, EventArgs e)
        //{
        //    System.Diagnostics.Process process = new System.Diagnostics.Process();
        //    System.Windows.Forms.HtmlElement link = null;
        //    try
        //    {
        //        txtContent.AllowNavigation = false;
        //        link = txtContent.Document.ActiveElement;
        //        url = link.GetAttribute("href");
        //        process.StartInfo.FileName = url.ToString();
        //        process.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
        //    }
        //    finally
        //    {
        //        url = null;
        //        link = null;
        //        process.Dispose();
        //    }
        //}

        private void btnAddResponse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (stdResponseDesc != string.Empty)
                {
                    int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                    Pointel.Interactions.Contact.Core.Request.RequestAddStdRespFavorite.AddAgentFavoriteResponse(stdResponseDesc.ToString(), ConfigContainer.Instance().AgentId);
                    //contactService.DoRequestAddAgentStdRespFavorite(stdResponseDesc.ToString(), ContactDataContext.GetInstance().PersonDBID);
                    stdResponseDesc = string.Empty;
                }
                else
                {
                    //System.Windows.MessageBox.Show("Please Select below 16 character of Standard Response", "Message Info");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void btnAdvanceSearchResponse_Click(object sender, RoutedEventArgs e)
        {
            btnAddResponse.Visibility = Visibility.Collapsed;
            btnAddResponseCompose.Visibility = Visibility.Collapsed;
            btnRemoveResponse.Visibility = Visibility.Collapsed;

            _selectedAttachments = null;
            txtSubject.Text = "";
            wpAttachments.Children.Clear();
            currentResponse = string.Empty;
            if (grd_ResponseContent.Children.Contains(htmlEditor))
                grd_ResponseContent.Children.Remove(htmlEditor);
            //   RTB.Document.Blocks.Clear();
            //  host.Child = null;
            if (txtSearch.Text.Length == 0)
            {
                if (_isFavoriteResponseClicked)
                {
                    TreeView1.Items.Clear();

                    int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                    OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetStdRespFavorite.GetAgentFavoriteResponse(agentid);
                    if (output.IContactMessage.Name != "EventError")
                    {
                        _favoriteCategories = (EventGetAgentStdRespFavorites)output.IContactMessage;
                        if (_favoriteCategories != null)
                        {
                            _favoriteResponseIdList.Clear();
                            if (_favoriteCategories.StandardResponses != null)
                            {
                                for (int index = 0; index < _favoriteCategories.StandardResponses.Count; index++)
                                {
                                    _favoriteResponseIdList.Add(_favoriteCategories.StandardResponses.Get(index));
                                }
                            }
                            LoadFavoriteResponses(_allCategories.SRLContent);

                        }
                    }
                }
                else
                {
                    _isFavoriteResponseClicked = false;
                    if (_allCategories != null)
                        LoadResponses(_allCategories.SRLContent);
                }
            }
            else
            {
                string searchCriteria = cmbFrom.SelectedItem.ToString();
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                // RTB.Document.Blocks.Clear();
                try
                {
                    if (_isFavoriteResponseClicked)
                    {
                        //Search from Favorites responses
                        int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                        OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetStdRespFavorite.GetAgentFavoriteResponse(agentid);
                        if (output.IContactMessage.Name != "EventError")
                        {
                            _favoriteCategories = (EventGetAgentStdRespFavorites)output.IContactMessage;
                            if (_favoriteCategories != null)
                            {
                                _favoriteResponseIdList.Clear();
                                if (_favoriteCategories.StandardResponses != null)
                                {
                                    for (int index = 0; index < _favoriteCategories.StandardResponses.Count; index++)
                                    {
                                        _favoriteResponseIdList.Add(_favoriteCategories.StandardResponses.Get(index));
                                    }
                                }
                            }
                            LoadFavoriteSearchResponses(_allCategories.SRLContent, searchCriteria);
                        }
                    }
                    else
                    {
                        //Search from All responses
                        CategoryList categoryList = _allCategories.SRLContent;
                        LoadSearchResponses(categoryList, searchCriteria);
                    }
                    if (TreeView1.Items.Count > 0)
                    {
                        btnAddResponseCompose.IsEnabled = true;
                        if (!_isFavoriteResponseClicked)
                        {
                            btnAddResponse.IsEnabled = true;
                            btnRemoveResponse.IsEnabled = false;
                        }
                        else
                        {
                            btnRemoveResponse.IsEnabled = true;
                            btnAddResponse.IsEnabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
                }
            }
            if (TreeView1.Items.Count <= 0)
                txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
            else
                txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnAdvanceSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isAdvancedResponseSearch)
                {
                    isAdvancedResponseSearch = false;
                    btnDownload.Source = ImageDataContext.GetInstance().EmailIconImageSourceDown;
                    ResponseExpand.Height = new GridLength(0);// GridLength.Auto;                   
                    txtSearch.Text = string.Empty;
                    ShowAdvanceSearchHeading.Text = "Show advance search";
                    ShowAdvanceSearchContent.Text = "Agent can view the advance search options";
                    //brdSearchBar.Visibility = Visibility.Visible;
                }
                else
                {
                    isAdvancedResponseSearch = true;
                    btnDownload.Source = ImageDataContext.GetInstance().EmailIconImageSourceUP;
                    ResponseExpand.Height = new GridLength(50);
                    ShowAdvanceSearchHeading.Text = "Hide advance search";
                    ShowAdvanceSearchContent.Text = "Agent can Hide the advance search options";
                    //brdSearchBar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void cmbResponse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtSubject.Text = "";
                currentResponse = string.Empty;
                if (wpAttachments != null)
                    wpAttachments.Children.Clear();
                //if (RTB!=null)
                //    RTB.Document.Blocks.Clear();
                //if (host != null)
                //    host.Child = null;
                ExpandGridAuto.Height = new GridLength(0);
                btnAddResponse.IsEnabled = false;
                _selectedAttachments = null;
                System.Windows.Controls.ComboBoxItem cmb = ((System.Windows.Controls.ComboBoxItem)cmbResponse.SelectedItem);
                if (cmb != null && cmb.Content != null && cmb.Content != string.Empty)
                {
                    if (cmb.Content.Equals("Favorite Responses"))
                    {
                        _isFavoriteResponseClicked = true;
                        btnAddResponse.Visibility = System.Windows.Visibility.Collapsed;
                        btnAddResponse.Width = 0;
                        btnRemoveResponse.Visibility = System.Windows.Visibility.Collapsed;
                        btnAddResponseCompose.Visibility = System.Windows.Visibility.Collapsed;
                        btnRemoveResponse.Width = 25;
                        btnAddResponseCompose.Width = 25;
                        btnRemoveResponse.IsEnabled = true;
                        TreeView1.Items.Clear();

                        int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                        OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetStdRespFavorite.GetAgentFavoriteResponse(agentid);
                        if (output.IContactMessage.Name != "EventError")
                        {
                            _favoriteCategories = (EventGetAgentStdRespFavorites)output.IContactMessage;
                            if (_favoriteCategories != null)
                            {
                                _favoriteResponseIdList.Clear();
                                if (_favoriteCategories.StandardResponses != null)
                                {
                                    for (int index = 0; index < _favoriteCategories.StandardResponses.Count; index++)
                                    {
                                        _favoriteResponseIdList.Add(_favoriteCategories.StandardResponses.Get(index));
                                    }
                                }
                                LoadFavoriteResponses(_allCategories.SRLContent);

                            }
                        }
                    }
                    else if (cmb.Content.Equals("All Responses"))
                    {
                        btnAddResponse.Visibility = System.Windows.Visibility.Collapsed;
                        btnAddResponseCompose.Visibility = System.Windows.Visibility.Collapsed;
                        btnAddResponse.Width = 25;
                        btnAddResponseCompose.Width = 25;

                        btnRemoveResponse.Visibility = System.Windows.Visibility.Collapsed;
                        btnRemoveResponse.Width = 0;
                        _isFavoriteResponseClicked = false;
                        btnAddResponse.IsEnabled = true;
                        if (_allCategories != null)
                            LoadResponses(_allCategories.SRLContent);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void OnItemSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedAttachments = null;
                TreeView1.Tag = e.OriginalSource;
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                if (grd_ResponseContent.Children.Contains(htmlEditor))
                    grd_ResponseContent.Children.Remove(htmlEditor);
                currentResponse = string.Empty;
                //RTB.Document.Blocks.Clear();
                //host.Child = null;
                if (TreeView1.SelectedItem != null)
                {
                    btnAddResponse.Visibility = System.Windows.Visibility.Collapsed;
                    btnAddResponseCompose.Visibility = System.Windows.Visibility.Collapsed;
                    btnRemoveResponse.Visibility = System.Windows.Visibility.Collapsed;
                    if (TreeView1.SelectedItem is StackPanel)
                    {
                        StackPanel stackPanel = (StackPanel)TreeView1.SelectedItem;
                        UIElementCollection lbl = (UIElementCollection)stackPanel.Children;
                        for (int i = 0; i < lbl.Count; i++)
                        {
                            Visual childVisual = (Visual)VisualTreeHelper.GetChild(stackPanel, i);
                            //select particular control 
                            if (childVisual is System.Windows.Controls.Label)
                            {
                                System.Windows.Controls.Label txtValue = (System.Windows.Controls.Label)childVisual;
                                stdResponseDesc = txtValue.Tag.ToString();
                                if (txtValue.Name.Contains("txtResponse"))
                                {
                                    btnAddResponse.Visibility = (btnAddResponse.Width == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);
                                    btnAddResponseCompose.Visibility = (btnAddResponseCompose.Width == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);
                                    btnRemoveResponse.Visibility = (btnRemoveResponse.Width == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);
                                    responseId = stdResponseDesc;
                                    GetStandardResponseAndAttachment();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void btnAddResponseCompose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (eventResponseNotify != null)
                    eventResponseNotify.Invoke(htmlEditor.GetContentinTextFormat(), htmlEditor.GetResponseContent(), txtSubject.Text, currentResponseName, _selectedAttachments);
            }
            catch (Exception generalException)
            {

                logger.Error("btnAddResponseCompose_Click: Error occurred in" + generalException.ToString());
            }
        }

        private void btnContactExpand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnContact.Source == ImageDataContext.GetInstance().EMailIconImageSourceCollapse)
                {
                    btnContact.Source = ImageDataContext.GetInstance().EMailIconImageSourceExpand;
                    ExpandContent.Text = "Hide details panel";
                    ExpandHeading.Text = "Collapse";
                    txtSubject.Visibility = System.Windows.Visibility.Visible;

                    htmlEditor.Visibility = htmlEditor != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    ExpandGridAuto.Height = new GridLength(1, GridUnitType.Star);

                }
                else
                {
                    btnContact.Source = ImageDataContext.GetInstance().EMailIconImageSourceCollapse;
                    ExpandContent.Text = "Show details panel";
                    ExpandHeading.Text = "Expand";
                    htmlEditor.Visibility = htmlEditor != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    txtSubject.Visibility = System.Windows.Visibility.Collapsed;
                    ExpandGridAuto.Height = new GridLength(0);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void btnRemoveResponse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentResponse = string.Empty;
                _selectedAttachments = null;
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                //RTB.Document.Blocks.Clear();
                //host.Child = null;
                if (TreeView1.SelectedItem != null)
                {
                    Pointel.Interactions.Contact.Core.Request.RequestRemoveAgentStdRespFavorite.RemoveAgentFavoriteResponse(stdResponseDesc.ToString(), ConfigContainer.Instance().AgentId);
                    cmbResponse_SelectionChanged(null, null);
                    txtSubject.Text = string.Empty;
                    if (grd_ResponseContent.Children.Contains(htmlEditor))
                        grd_ResponseContent.Children.Remove(htmlEditor);
                    //txtContent.DocumentText = "";
                    //RTB.Document.Blocks.Clear();
                    //host.Child = null;
                }
                int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetStdRespFavorite.GetAgentFavoriteResponse(agentid);
                if (output.IContactMessage.Name != "EventError")
                {
                    _favoriteCategories = (EventGetAgentStdRespFavorites)output.IContactMessage;
                    if (_favoriteCategories != null)
                    {
                        _favoriteResponseIdList.Clear();

                        if (_favoriteCategories.StandardResponses != null)
                        {
                            for (int index = 0; index < _favoriteCategories.StandardResponses.Count; index++)
                            {
                                _favoriteResponseIdList.Add(_favoriteCategories.StandardResponses.Get(index));
                            }
                        }
                        LoadFavoriteResponses(_allCategories.SRLContent);
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("btnAddResponse_Click:" + generalException.ToString());
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

                //if (isEnableResponseWebHost)
                //    txtContent.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(txtContent_DocumentCompleted);
                this.btnAddResponse.Click -= btnAddResponse_Click;
                this.btnAddResponseCompose.Click -= btnAddResponseCompose_Click;
                this.btnAdvanceSearch.Click -= btnAdvanceSearch_Click;
                this.btnAdvanceSearchResponse.Click -= btnAdvanceSearchResponse_Click;
                this.btnContactExpand.Click -= btnContactExpand_Click;
                this.btnRemoveResponse.Click -= btnRemoveResponse_Click;
                this.cmbResponse.SelectionChanged -= cmbResponse_SelectionChanged;
                this.txtSearch.TextChanged -= txtSearch_TextChanged_1;
                if (eventResponseNotify != null)
                    eventResponseNotify -= new EventResponseNotification(_refFunction);
                stackPanel = null;
                imgclose = null;
                txtBlock = null;
                stdResponseDesc = null;
                //txtContent = null;
                //RTB = null;
                initialValue = null;
                value = null;
                Contactplugins = null;
                ContactpluginInteraction = null;
                ContactpluginChat = null;
                //host = null;
                url = null;
                if (grd_ResponseContent.Children.Contains(htmlEditor))
                    grd_ResponseContent.Children.Remove(htmlEditor);
                htmlEditor = null;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
            finally
            {
                //GC.Collect();
            }
        }

        private void txtSearch_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (txtSearch.Text.Length == 0)
                {
                    //LoadResponses();
                }
                else
                {
                    searchContent.Text = "clear selection and display all selected view contents";
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
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

        void btnAttachment_Click(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;
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
                        var docId = ((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString();
                        OutputValues outValues = Pointel.Interactions.Contact.Core.Request.RequestToGetDocument.GetAttachDocument(docId);
                        getAttachDocument = outValues.IContactMessage is EventGetDocument ? (outValues.IContactMessage as EventGetDocument) : null;

                        if (getAttachDocument != null)
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp\" + docId.ToString() + @"\";
                            using (fs = new FileStream(getAttachDocument.TheName, FileMode.Create)) { }
                            logger.Info("Opening the file : " + getAttachDocument.TheName);
                            if (string.IsNullOrEmpty(Path.GetDirectoryName(getAttachDocument.TheName)))
                                getAttachDocument.TheName = path + @"\" + getAttachDocument.TheName;
                            logger.Info("Creating the file : " + getAttachDocument.TheName);
                            if (!Directory.Exists(Path.GetDirectoryName(getAttachDocument.TheName)))
                                Directory.CreateDirectory(Path.GetDirectoryName(getAttachDocument.TheName));
                            File.WriteAllBytes(getAttachDocument.TheName, getAttachDocument.Content);
                            Process.Start(getAttachDocument.TheName);
                            //not working
                            //Process process = new Process();
                            //process.StartInfo.FileName = getAttachDocument.TheName;
                            //process.EnableRaisingEvents = true;
                            //process.Exited += new EventHandler(process_Exited);
                            //process.Start();
                        }
                        break;
                    case "save":
                        OutputValues outputValues = Pointel.Interactions.Contact.Core.Request.RequestToGetDocument.GetAttachDocument(((menuitem.Parent as System.Windows.Controls.ContextMenu).PlacementTarget as System.Windows.Controls.Button).Tag.ToString());
                        getAttachDocument = outputValues.IContactMessage is EventGetDocument ? (outputValues.IContactMessage as EventGetDocument) : null;
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
                logger.Error("Error occurred while " + menuitem.Header.ToString().ToLower() + " the attachment '" + menuitem.Header.ToString().Trim() + "'  as :" + generalException.ToString());
            }
            finally
            {
                fs = null;
                //GC.Collect();
            }
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
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void AddResponseCategories(CategoryList categoryList, TreeViewItem treeItemNode)
        {
            try
            {
                if (categoryList != null)
                {
                    if (categoryList.Count > 0)
                    {
                        int count = 0;
                        foreach (Category ctgy in categoryList)
                        {
                            TreeViewItem treeViewSubNode = new TreeViewItem();
                            treeViewSubNode.Header = categoryList.Get(count).Name;
                            treeItemNode.Items.Add(treeViewSubNode);
                            if (ctgy.ChildrenCategories != null)
                            {
                                if (ctgy.ChildrenCategories.Count > 0)
                                {
                                    CategoryList cList = categoryList.Get(count).ChildrenCategories;
                                    if (cList != null)
                                        AddResponseCategories(cList, treeViewSubNode);
                                }
                            }
                            if (ctgy.ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                            {
                                if (ctgy.ChildrenStdResponses.Count > 0)
                                {
                                    StandardResponseList responseListValues = categoryList.Get(count).ChildrenStdResponses;
                                    if (responseListValues != null)
                                    {
                                        for (int k = 0; k < responseListValues.Count; k++)
                                        {
                                            AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                        }
                                    }
                                }
                            }
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void AddFavoriteResponseCategories(CategoryList categoryList, TreeViewItem treeItemNode)
        {
            try
            {
                if (categoryList != null)
                {
                    if (categoryList.Count > 0)
                    {
                        int count = 0;
                        foreach (Category ctgy in categoryList)
                        {
                            TreeViewItem treeViewSubNode = new TreeViewItem();
                            treeViewSubNode.Header = categoryList.Get(count).Name;
                            if (ctgy.ChildrenCategories != null)
                            {
                                if (ctgy.ChildrenCategories.Count > 0)
                                {
                                    CategoryList cList = categoryList.Get(count).ChildrenCategories;
                                    if (cList != null)
                                        AddFavoriteResponseCategories(cList, treeViewSubNode);
                                }
                            }
                            if (ctgy.ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                            {
                                if (ctgy.ChildrenStdResponses.Count > 0)
                                {
                                    StandardResponseList responseListValues = categoryList.Get(count).ChildrenStdResponses;
                                    if (responseListValues != null)
                                    {
                                        for (int k = 0; k < responseListValues.Count; k++)
                                        {
                                            if (_isFavoriteResponseClicked)
                                            {
                                                if (_favoriteResponseIdList.Contains(responseListValues[k].StandardResponseId))
                                                    AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                            }
                                            else
                                                AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                        }
                                    }
                                }
                            }
                            if (treeViewSubNode.Items.Count > 0)
                                treeItemNode.Items.Add(treeViewSubNode);
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void AddStandardResponses(TreeViewItem treeItemNode, StandardResponse stdResponse)
        {
            try
            {
                _standardResponseList.Add(stdResponse);
                _responseCount++;
                stackPanel = new StackPanel();
                stackPanel.Name = "stack" + (_responseCount).ToString().Trim();
                stackPanel.Orientation = Orientation.Horizontal;
                imgclose = new Image();

                imgclose.Name = "Img" + (_responseCount).ToString().Trim();
                imgclose.Height = 15;
                imgclose.Width = 30;
                imgclose.Source = ImageDataContext.GetInstance().EmailIconImageSourceWhiteText;
                txtBlock = new System.Windows.Controls.Label();
                txtBlock.Name = "txtResponse" + (_responseCount).ToString().Trim();
                txtBlock.Content = stdResponse.TheName;
                txtBlock.Tag = stdResponse.StandardResponseId;

                imgclose.VerticalAlignment = VerticalAlignment.Center;

                stackPanel.Children.Add(imgclose);
                stackPanel.Children.Add(txtBlock);

                treeItemNode.Items.Add(stackPanel);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void LoadResponses(CategoryList categoryList)
        {
            try
            {
                wpAttachments.Children.Clear();
                txtSubject.Text = "";
                //RTB.Document.Blocks.Clear();
                //host.Child = null;

                TreeView1.Items.Clear();
                if (categoryList != null)
                {
                    for (int index = 0; index < categoryList.Count; index++)
                    {
                        TreeViewItem rootItem = new TreeViewItem();
                        rootItem.Header = categoryList[index].Name;
                        TreeView1.Items.Add(rootItem);

                        AddResponseCategories(categoryList.Get(index).ChildrenCategories, rootItem);
                        if (categoryList.Get(index).ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                        {
                            if (categoryList.Get(index).ChildrenStdResponses.Count > 0)
                            {
                                StandardResponseList responseListValues = categoryList.Get(index).ChildrenStdResponses;
                                if (responseListValues != null)
                                {
                                    for (int k = 0; k < responseListValues.Count; k++)
                                    {
                                        AddStandardResponses(rootItem, responseListValues[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                if (TreeView1.Items.Count <= 0)
                    txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
                else
                    txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception generalException)
            {
                logger.Error("UserControl_Loaded: Error occurred In" + generalException.ToString());
            }
        }

        private void LoadFavoriteResponses(CategoryList categoryList)
        {
            try
            {
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                //RTB.Document.Blocks.Clear();
                //host.Child = null;

                TreeView1.Items.Clear();
                if (categoryList != null)
                {
                    for (int index = 0; index < categoryList.Count; index++)
                    {
                        TreeViewItem rootItem = new TreeViewItem();
                        rootItem.Header = categoryList[index].Name;
                        AddFavoriteResponseCategories(categoryList.Get(index).ChildrenCategories, rootItem);
                        if (categoryList.Get(index).ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                        {
                            if (categoryList.Get(index).ChildrenStdResponses.Count > 0)
                            {
                                StandardResponseList responseListValues = categoryList.Get(index).ChildrenStdResponses;
                                if (responseListValues != null)
                                {
                                    for (int k = 0; k < responseListValues.Count; k++)
                                    {
                                        if (_isFavoriteResponseClicked)
                                        {
                                            if (_favoriteResponseIdList.Contains(responseListValues[k].StandardResponseId))
                                                AddStandardResponses(rootItem, responseListValues[k]);
                                        }
                                        else
                                            AddStandardResponses(rootItem, responseListValues[k]);
                                    }
                                }
                            }
                        }
                        if (rootItem.Items.Count > 0)
                            TreeView1.Items.Add(rootItem);
                    }
                }
                if (TreeView1.Items.Count <= 0)
                    txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
                else
                    txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception generalException)
            {
                logger.Error("LoadFavoriteResponses: Error occurred In" + generalException.ToString());
            }
        }

        private void GetStandardResponseAndAttachment()
        {
            try
            {
                grd_ResponseContent.Children.Clear();
                btnContact.Source = ImageDataContext.GetInstance().EMailIconImageSourceCollapse;
                btnContactExpand.IsEnabled = true;
                if (btnContact.Source != ImageDataContext.GetInstance().EMailIconImageSourceExpand)
                    btnContactExpand_Click(null, null);

                ExpandContent.Text = "Hide details panel";
                ExpandHeading.Text = "Collapse";
                ExpandGridAuto.Height = new GridLength(1, GridUnitType.Star);

                txtSubject.Text = string.Empty;
                //txtContent.DocumentText = string.Empty;
                //RTB.Document.Blocks.Clear();

                foreach (StandardResponse stdResponse in _standardResponseList)
                {
                    if (stdResponse.StandardResponseId == responseId)
                    {
                        txtSubject.Text = !string.IsNullOrEmpty(stdResponse.Subject) ? stdResponse.Subject : string.Empty;
                        if (stdResponse.AgentDesktopUsageType.Replace(" ", "").ToLower().Contains("notused"))
                        {
                            currentResponse = "The \"" + stdResponse.TheName + "\" standard response is not available." +
                                Environment.NewLine + "This standard response is not intended for manual use";
                            var txtBlock = new TextBlock()
                            {
                                Text = currentResponse,
                                FontStyle = FontStyles.Italic,
                                TextAlignment = System.Windows.TextAlignment.Center,
                                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                Margin = new Thickness(0, 10, 0, 0)
                            };
                            grd_ResponseContent.Background = (Brush)(new BrushConverter().ConvertFromString("#C6C7C6"));
                            grd_ResponseContent.Children.Add(txtBlock);
                            btnAddResponseCompose.IsEnabled = false;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(stdResponse.StructuredBody))
                                currentResponse = stdResponse.StructuredBody;
                            else
                            {
                                currentResponse = stdResponse.Body;
                                currentResponse = currentResponse.Replace("\r\n", "<br />");
                                currentResponse = currentResponse.Replace("\n", "<br />");
                            }

                            if (grd_ResponseContent.Children.Contains(htmlEditor))
                                grd_ResponseContent.Children.Remove(htmlEditor);

                            if (currentResponse != null)
                            {
                                htmlEditor = new HTMLEditor(false, !String.IsNullOrEmpty(stdResponse.StructuredBody), currentResponse);
                                htmlEditor.MinHeight = 90;
                                Grid.SetRow(htmlEditor, 2);
                                grd_ResponseContent.Background = Brushes.White;
                                grd_ResponseContent.Children.Add(htmlEditor);
                                btnAddResponseCompose.IsEnabled = true;
                            }

                            if (stdResponse.Attachments != null)
                            {
                                _selectedAttachments = stdResponse.Attachments;
                                wpAttachments.Children.Clear();
                                //rowAttachment.Height = new GridLength(30);
                                if (stdResponse.Attachments.Count > 0)
                                {
                                    if (stdResponse.Attachments.Count > 0)
                                    {
                                        for (int indexValue = 0; indexValue < stdResponse.Attachments.Count; indexValue++)
                                        {
                                            wpAttachments.Children.Add(LoadAttachments(stdResponse.Attachments[indexValue].TheName, stdResponse.Attachments[indexValue].TheSize.Value.ToString(), stdResponse.Attachments[indexValue].DocumentId));
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void cmbSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtSearch.Text.Length == 0)
                return;
            txtSubject.Text = "";
            wpAttachments.Children.Clear();
            //RTB.Document.Blocks.Clear();
            //host.Child = null;
            string searchCriteria = "";
            if (cmbFrom.SelectedItem != null)
                searchCriteria = cmbFrom.SelectedItem.ToString();
            try
            {
                if (_isFavoriteResponseClicked)
                {
                    //Search from Favorites responses
                    int? agentid = int.Parse(ConfigContainer.Instance().AgentId);
                    OutputValues output = Pointel.Interactions.Contact.Core.Request.RequestGetStdRespFavorite.GetAgentFavoriteResponse(agentid);
                    if (output.IContactMessage.Name != "EventError")
                    {
                        _favoriteCategories = (EventGetAgentStdRespFavorites)output.IContactMessage;
                        if (_favoriteCategories != null)
                        {
                            _favoriteResponseIdList.Clear();
                            for (int index = 0; index < _favoriteCategories.StandardResponses.Count; index++)
                            {
                                _favoriteResponseIdList.Add(_favoriteCategories.StandardResponses.Get(index));
                            }
                        }
                        LoadFavoriteSearchResponses(_allCategories.SRLContent, searchCriteria);
                    }
                }
                else
                {
                    //Search from All responses
                    CategoryList categoryList = _allCategories.SRLContent;
                    LoadSearchResponses(categoryList, searchCriteria);
                }
                if (TreeView1.Items.Count <= 0)
                    txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
                else
                    txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void LoadSearchResponses(CategoryList categoryList, string searchCriteria)
        {
            try
            {
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                //RTB.Document.Blocks.Clear();
                //host.Child = null;
                TreeView1.Items.Clear();
                if (categoryList != null)
                {
                    for (int index = 0; index < categoryList.Count; index++)
                    {
                        TreeViewItem rootItem = new TreeViewItem();
                        rootItem.Header = categoryList[index].Name;

                        AddSearchResponseCategories(categoryList.Get(index).ChildrenCategories, rootItem, searchCriteria);
                        if (categoryList.Get(index).ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                        {
                            if (categoryList.Get(index).ChildrenStdResponses.Count > 0)
                            {
                                StandardResponseList responseListValues = categoryList.Get(index).ChildrenStdResponses;
                                if (responseListValues != null)
                                {
                                    for (int k = 0; k < responseListValues.Count; k++)
                                    {
                                        if (searchCriteria.ToLower().Contains("any"))
                                        {
                                            if (isResponseNameChecked)
                                            {
                                                string[] searchKeys = txtSearch.Text.Split(' ');

                                            }

                                        }
                                        else if (searchCriteria.ToLower().Contains("all"))
                                        {

                                        }
                                        else if (searchCriteria.ToLower().Contains("exact"))
                                        {

                                        }
                                        else if (searchCriteria.ToLower().Contains("contains"))
                                        {
                                            //-----------
                                            if (!isAdvancedResponseSearch)
                                            {
                                                if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                        (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                       (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                {
                                                    AddStandardResponses(rootItem, responseListValues[k]);
                                                }
                                            }
                                            else
                                            {
                                                if (isResponseNameChecked && isResponseBodyChecked)
                                                {
                                                    if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                         (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                        (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                    {
                                                        AddStandardResponses(rootItem, responseListValues[k]);
                                                    }
                                                }
                                                else if (isResponseNameChecked)
                                                {
                                                    if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()))
                                                    {
                                                        AddStandardResponses(rootItem, responseListValues[k]);
                                                    }
                                                }
                                                else if (isResponseBodyChecked)
                                                {
                                                    if ((responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                        (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                    {
                                                        AddStandardResponses(rootItem, responseListValues[k]);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        if (rootItem.Items.Count > 0)
                            TreeView1.Items.Add(rootItem);
                    }
                }
                if (TreeView1.Items.Count <= 0)
                    txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
                else
                    txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception generalException)
            {
                logger.Error("UserControl_Loaded: Error occurred In" + generalException.ToString());
            }
        }

        private void AddSearchResponseCategories(CategoryList categoryList, TreeViewItem treeItemNode, string searchCriteria)
        {
            try
            {
                if (categoryList != null)
                {
                    if (categoryList.Count > 0)
                    {
                        int count = 0;
                        foreach (Category ctgy in categoryList)
                        {
                            TreeViewItem treeViewSubNode = new TreeViewItem();
                            treeViewSubNode.Header = categoryList.Get(count).Name;
                            if (ctgy.ChildrenCategories != null)
                            {
                                if (ctgy.ChildrenCategories.Count > 0)
                                {
                                    CategoryList cList = categoryList.Get(count).ChildrenCategories;
                                    if (cList != null)
                                        AddSearchResponseCategories(cList, treeViewSubNode, searchCriteria);
                                }
                            }
                            if (ctgy.ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                            {
                                if (ctgy.ChildrenStdResponses.Count > 0)
                                {
                                    StandardResponseList responseListValues = categoryList.Get(count).ChildrenStdResponses;
                                    if (responseListValues != null)
                                    {
                                        for (int k = 0; k < responseListValues.Count; k++)
                                        {
                                            if (searchCriteria.ToLower().Contains("any"))
                                            {
                                                if (isResponseNameChecked)
                                                {
                                                    string[] searchKeys = txtSearch.Text.Split(' ');
                                                }
                                            }
                                            else if (searchCriteria.ToLower().Contains("all"))
                                            {

                                            }
                                            else if (searchCriteria.ToLower().Contains("exact"))
                                            {

                                            }
                                            else if (searchCriteria.ToLower().Contains("contains"))
                                            {
                                                if (isResponseNameChecked && isResponseBodyChecked)
                                                {
                                                    if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                        (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                        (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                    {
                                                        AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                    }
                                                }
                                                else if (isResponseNameChecked)
                                                {
                                                    if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()))
                                                    {
                                                        AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                    }
                                                }
                                                else if (isResponseBodyChecked)
                                                {
                                                    if (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                        responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower()))
                                                    {
                                                        AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            if (treeViewSubNode.Items.Count > 0)
                                treeItemNode.Items.Add(treeViewSubNode);
                            //}
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void LoadFavoriteSearchResponses(CategoryList categoryList, string searchCriteria)
        {
            try
            {
                txtSubject.Text = "";
                wpAttachments.Children.Clear();
                //RTB.Document.Blocks.Clear();
                //host.Child = null;
                TreeView1.Items.Clear();
                if (categoryList != null)
                {
                    for (int index = 0; index < categoryList.Count; index++)
                    {
                        TreeViewItem rootItem = new TreeViewItem();
                        rootItem.Header = categoryList[index].Name;
                        AddFavoriteSearchResponseCategories(categoryList.Get(index).ChildrenCategories, rootItem, searchCriteria);
                        if (categoryList.Get(index).ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                        {
                            if (categoryList.Get(index).ChildrenStdResponses.Count > 0)
                            {
                                StandardResponseList responseListValues = categoryList.Get(index).ChildrenStdResponses;
                                if (responseListValues != null)
                                {
                                    for (int k = 0; k < responseListValues.Count; k++)
                                    {
                                        if (_isFavoriteResponseClicked)
                                        {
                                            if (_favoriteResponseIdList.Contains(responseListValues[k].StandardResponseId))
                                            {
                                                if (searchCriteria.ToLower().Contains("any"))
                                                {
                                                    if (isResponseNameChecked)
                                                    {
                                                        string[] searchKeys = txtSearch.Text.Split(' ');

                                                    }

                                                }
                                                else if (searchCriteria.ToLower().Contains("all"))
                                                {

                                                }
                                                else if (searchCriteria.ToLower().Contains("exact"))
                                                {

                                                }
                                                else if (searchCriteria.ToLower().Contains("contains"))
                                                {
                                                    if (isResponseNameChecked && isResponseBodyChecked)
                                                    {
                                                        if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                            (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                            (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                        {
                                                            AddStandardResponses(rootItem, responseListValues[k]);
                                                        }
                                                    }
                                                    else if (isResponseNameChecked)
                                                    {
                                                        if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()))
                                                        {
                                                            AddStandardResponses(rootItem, responseListValues[k]);
                                                        }
                                                    }
                                                    else if (isResponseBodyChecked)
                                                    {
                                                        if (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                            responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower()))
                                                        {
                                                            AddStandardResponses(rootItem, responseListValues[k]);
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (rootItem.Items.Count > 0)
                            TreeView1.Items.Add(rootItem);
                    }
                }
                if (TreeView1.Items.Count <= 0)
                    txtAlertMessage.Visibility = System.Windows.Visibility.Visible;
                else
                    txtAlertMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception generalException)
            {
                logger.Error("LoadFavoriteResponses: Error occurred In" + generalException.ToString());
            }
        }

        private void AddFavoriteSearchResponseCategories(CategoryList categoryList, TreeViewItem treeItemNode, string searchCriteria)
        {
            try
            {
                if (categoryList != null)
                {
                    if (categoryList.Count > 0)
                    {
                        int count = 0;
                        foreach (Category ctgy in categoryList)
                        {
                            TreeViewItem treeViewSubNode = new TreeViewItem();
                            treeViewSubNode.Header = categoryList.Get(count).Name;
                            if (ctgy.ChildrenCategories != null)
                            {
                                if (ctgy.ChildrenCategories.Count > 0)
                                {
                                    CategoryList cList = categoryList.Get(count).ChildrenCategories;
                                    if (cList != null)
                                        AddFavoriteSearchResponseCategories(cList, treeViewSubNode, searchCriteria);
                                }
                            }
                            if (ctgy.ChildrenStdResponses != null)// && ctgy.ChildrenCategories == null)
                            {
                                if (ctgy.ChildrenStdResponses.Count > 0)
                                {
                                    StandardResponseList responseListValues = categoryList.Get(count).ChildrenStdResponses;
                                    if (responseListValues != null)
                                    {
                                        for (int k = 0; k < responseListValues.Count; k++)
                                        {
                                            if (_isFavoriteResponseClicked)
                                            {
                                                if (_favoriteResponseIdList.Contains(responseListValues[k].StandardResponseId))
                                                {
                                                    if (searchCriteria.ToLower().Contains("any"))
                                                    {
                                                        if (isResponseNameChecked)
                                                        {
                                                            string[] searchKeys = txtSearch.Text.Split(' ');

                                                        }

                                                    }
                                                    else if (searchCriteria.ToLower().Contains("all"))
                                                    {

                                                    }
                                                    else if (searchCriteria.ToLower().Contains("exact"))
                                                    {

                                                    }
                                                    else if (searchCriteria.ToLower().Contains("contains"))
                                                    {
                                                        if (isResponseNameChecked && isResponseBodyChecked)
                                                        {
                                                            if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                                                (responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                                (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                            {
                                                                AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                            }
                                                        }
                                                        else if (isResponseNameChecked)
                                                        {
                                                            if (responseListValues[k].TheName.ToLower().Contains(txtSearch.Text.ToLower()))
                                                            {
                                                                AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                            }
                                                        }
                                                        else if (isResponseBodyChecked)
                                                        {
                                                            if ((responseListValues[k].Body != null && responseListValues[k].Body.ToLower().Contains(txtSearch.Text.ToLower())) ||
                                                                (responseListValues[k].StructuredBody != null && responseListValues[k].StructuredBody.ToLower().Contains(txtSearch.Text.ToLower())))
                                                            {
                                                                AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else
                                                AddStandardResponses(treeViewSubNode, responseListValues[k]);
                                        }
                                    }
                                }
                            }
                            if (treeViewSubNode.Items.Count > 0)
                                treeItemNode.Items.Add(treeViewSubNode);
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void ResponseName_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkResponseName.IsChecked)
                isResponseNameChecked = true;
            else
                isResponseNameChecked = false;
        }

        private void ResponseBodyChecked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkResponseBody.IsChecked)
                isResponseBodyChecked = true;
            else
                isResponseBodyChecked = false;
        }

        private void TreeView1_Unselected(object sender, RoutedEventArgs e)
        {
            btnContactExpand.IsEnabled = false;
        }
    }
}
