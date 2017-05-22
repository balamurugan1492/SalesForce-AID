using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Threading;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Chat.Core;
using Pointel.Interactions.Chat.Core.General;
using Pointel.Interactions.Chat.CustomControls;
using Pointel.Interactions.Chat.Helpers;
using Pointel.Interactions.Chat.Settings;
using Pointel.Interactions.Chat.UserControls;
using Pointel.Interactions.IPlugins;
using Pointel.Tools;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Chat.WinForms
{
    /// <summary>
    /// Interaction logic for ChatMainWindow.xaml
    /// </summary>
    public partial class ChatMainWindow : Window
    {
        #region Fields Declaration
        private ChatUtil _chatUtil = null;
        private ContextMenu _availablePushURL = new ContextMenu();
        private ContextMenu _rtbContextMenu = new ContextMenu();
        private ContextMenu _txtMessageContextMenu = new ContextMenu();
        private ContextMenu _txtSuggestionContextMenu = new ContextMenu();
        private ContextMenu _btnChatPersonDataContextMenu = new ContextMenu();
        private DialPad dialpad;
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        public string InteractionID;
        string userID = string.Empty;
        string visibility = string.Empty;
        string otherAgentID = string.Empty;
        string selectedUser = string.Empty;
        private static bool _isAgentAliveinChatIXN = false;
        private DispatcherTimer typingTimer = new DispatcherTimer();
        private static bool _isChatTyping = true;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private string theComment = string.Empty;
        private Regex urlRegex = new Regex(@"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$");
        private Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

        Paragraph consultInviteParagraph = new Paragraph();
        bool _isChatDoneClicked = false;
        private DispatcherTimer _timerforcloseError;
        private double _windowMainGridWidth = 400;
        private double _windowRightGridWidth = 400;
        private double _windowFullWidth = 0;
        private string CustomDictionary = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\dic\dictionary.lex";
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        bool _isPasteText = false;
        public static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();
        private List<string> ignoreWords = new List<string>();
        List<TextRange> rangeList = new List<TextRange>();
        string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
        TextDecorationCollection _txtDecorationCollection = null;
        #endregion Fields Declaration

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMainWindow"/> class.
        /// </summary>
        public ChatMainWindow(string interactionID, ChatUtil chatUtil = null)
        {
            InitializeComponent();
            DataObject.AddPastingHandler(txtChatSend, new DataObjectPastingEventHandler(OnPaste));
            WindowResizer winResize = new WindowResizer(this);
            winResize.addResizerDown(BottomSideRect);
            winResize.addResizerRight(RightSideRect);
            winResize.addResizerRightDown(RightbottomSideRect);
            winResize = null;
            InteractionID = interactionID;
            this.ChatWindow.Tag = interactionID;
            this.Tag = interactionID;
            //if (chatUtil != null)
            _chatUtil = chatUtil;
            //else if (_chatDataContext.MainWindowSession.ContainsKey(interactionID))
            //    _chatUtil = _chatDataContext.MainWindowSession[interactionID];
            this.DataContext = _chatUtil;
            _chatUtil.UserName = _chatDataContext.UserName;
            _chatUtil.PlaceID = _chatDataContext.PlaceID;
            _chatUtil.NotificationImage = (FrameworkElement)imgChatIcon;
            try
            {
                _chatUtil.Spellcheck = null;
                _chatUtil.Spellcheck = new SpellChecker();
            }
            catch (Exception generalException)
            {
                _logger.Error("Spell Checker problem : " + generalException.ToString());
            }
            typingTimer.Tick += new EventHandler(typingTimer_Tick);
            mainRTB.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate));
            Width = _windowMainGridWidth + _windowRightGridWidth + 16 + columnButtons.Width.Value;
            MinWidth = Width;
            //txtChatSend.SpellCheck.CustomDictionaries.Add(new Uri(@CustomDictionary));
            CustomTextDecorationCollection();
            CreateLEXFile(CustomDictionary);
            //InputLanguageManager.SetInputLanguage(txtChatSend, CultureInfo.CreateSpecificCulture("en-us"));
            //string lex = txtChatSend.Language.ToString();
            //txtChatSend.Language = System.Windows.Markup.XmlLanguage.GetLanguage("pt-br");   
            // var lang = InputLanguageManager.Current.AvailableInputLanguages;
            //_logger.Info(lang.ToString());
            // string lan=  this.Dispatcher.Thread.CurrentCulture.Name.ToString();
            //InputLanguageManager.SetInputLanguage(txtChatSend, CultureInfo.CreateSpecificCulture("fr"));
            //string lange = "Input Language of myTextBox is " + InputLanguageManager.GetInputLanguage(txtChatSend).ToString();
            //string data = "CurrentCulture is Set to " + this.Dispatcher.Thread.CurrentCulture.Name.ToString();

        }

        #endregion Constructor

        #region Window Events

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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
                _chatUtil.ChatWindowTitleText = _chatUtil.ChatFromPersonName + " - " + "Agent Interaction Desktop";
                _chatUtil.ChatTypeStatus = _chatUtil.IxnType + " Chat";
                _chatUtil.ConsultDockPanel = dpConsultWindow;
                if (_chatUtil.IxnType == "Consult")
                {
                    _chatUtil.SendChatWindowRowHeight = new GridLength(0);
                    _chatUtil.ChatToolBarRowHeight = new GridLength(0);
                    _chatUtil.ConsultDockPanel.Children.Clear();
                    _chatUtil.ConsultDockPanel.Children.Add(new ChatConsultationWindow(_chatUtil, InteractionID));
                    _chatUtil.ConsultChatWindowRowHeight = GridLength.Auto;
                }
                else
                {
                    _chatUtil.SendChatWindowRowHeight = GridLength.Auto;
                    _chatUtil.ConsultChatWindowRowHeight = new GridLength(0);
                    _chatUtil.ChatToolBarRowHeight = GridLength.Auto;
                }
                _chatUtil.ChatContentRowHeight = new GridLength(0);
                _chatUtil.CaseExpanderRowHight = GridLength.Auto;
                _chatUtil.NotificationImageSource = _chatDataContext.Imagepath + "\\Chat\\Chat.png";
                //imgChatIcon.Source = loadBitmap(LoadPicture(new BitmapImage(new Uri("/Pointel.Interactions.Chat;component/Images/Chat/Chat.png", UriKind.Relative))));
                CloseError(null, null);
                _chatUtil.BtnArrowImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\downArrow.png", UriKind.Relative));
                btnSendUrl.Tag = "Show";
                _chatUtil.ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.png", UriKind.Relative));
                _chatUtil.IsEnableRelease = true;
                _chatUtil.TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.png", UriKind.Relative));
                _chatUtil.IsEnableTransfer = true;
                _chatUtil.ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.png", UriKind.Relative));
                _chatUtil.IsEnableConference = true;
                _chatUtil.ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.png", UriKind.Relative));
                _chatUtil.IsEnableChatConsult = true;
                _chatUtil.DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.Disable.png", UriKind.Relative));
                _chatUtil.IsEnableDone = false;
                if (!ChatDataContext.GetInstance().IsAvailableVoiceMedia)
                {
                    _chatUtil.VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                    _chatUtil.IsEnableVoiceConsult = false;
                    _chatUtil.EnableCallMenuitems = false;
                }
                else
                {
                    _chatUtil.VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.png", UriKind.Relative));
                    _chatUtil.IsEnableVoiceConsult = true;
                    _chatUtil.EnableCallMenuitems = true;
                }
                _chatUtil.IsTextMessageEnabled = true;
                _chatUtil.IsTextURLEnabled = true;
                _chatUtil.IsButtonSendEnabled = true;
                _chatUtil.IsButtonCheckURL = true;
                _chatUtil.IsButtonAvailableURL = true;
                _chatUtil.IsButtonPushURLExpander = true;
                _chatUtil.IsConversationRTBEnabled = true;
                grdUC.Children.Clear();
                Pointel.Interactions.Chat.UserControls.InteractionData interactionData = new Pointel.Interactions.Chat.UserControls.InteractionData(_chatUtil, InteractionID);
                interactionData.Name = "InterData_" + InteractionID + "_" + _chatUtil.ProxyId;
                Grid.SetColumn(interactionData, 1);
                grdUC.Children.Add(interactionData);
                btnData.IsChecked = true;
                _chatUtil.CasedataImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\leftArrow.png", UriKind.Relative));
                btnData.Tag = "Hide";
                _chatUtil.ContactImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                btnContacts.Tag = "Show";
                _chatUtil.ResponseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                btnResponses.Tag = "Show";

                Pointel.Interactions.Chat.HandleInteractions.ChatInteractionSubscriber._ixnFailed += new HandleInteractions.ChatInteractionSubscriber.IxnServerFailed(ChatInteractionSubscriber__ixnFailed);

                foreach (string item in _chatDataContext.LoadAvailablePushURL.Keys)
                {
                    MenuItem _mItem = new MenuItem();
                    _mItem.Header = item;
                    _mItem.Margin = new System.Windows.Thickness(2);
                    _mItem.Click += new RoutedEventHandler(PushURLMenuitem_Click);
                    _availablePushURL.Style = (Style)FindResource("Contextmenu");
                    _availablePushURL.Items.Add(_mItem);
                }

                MenuItem _mnuCopy = new MenuItem();
                _mnuCopy.Header = "Copy";
                //_mnuCopy.InputGestureText = "Ctrl+C";
                _mnuCopy.Click += new RoutedEventHandler(rtbContextMenu_Click);
                _mnuCopy.Margin = new System.Windows.Thickness(2);
                _rtbContextMenu.Items.Add(_mnuCopy);
                MenuItem _mnuSelectAll = new MenuItem();
                _mnuSelectAll.Header = "Select All";
                _mnuSelectAll.Margin = new System.Windows.Thickness(2);
                // _mnuSelectAll.InputGestureText = "Ctrl+A";
                _mnuSelectAll.Click += new RoutedEventHandler(rtbContextMenu_Click);
                _rtbContextMenu.Style = (Style)FindResource("Contextmenu");
                _rtbContextMenu.Items.Add(_mnuSelectAll);
                _rtbContextMenu.Opened += new RoutedEventHandler(_rtbContextMenu_Opened);

                MenuItem _mnuItemCut = new MenuItem();
                _mnuItemCut.Header = "Cut";
                _mnuItemCut.Margin = new System.Windows.Thickness(2);
                _mnuItemCut.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                _txtMessageContextMenu.Items.Add(_mnuItemCut);

                MenuItem _mnuItemCopy = new MenuItem();
                _mnuItemCopy.Header = "Copy";
                _mnuItemCopy.Margin = new System.Windows.Thickness(2);
                _mnuItemCopy.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                _txtMessageContextMenu.Items.Add(_mnuItemCopy);

                MenuItem _mnuItemPaste = new MenuItem();
                _mnuItemPaste.Header = "Paste";
                _mnuItemPaste.Margin = new System.Windows.Thickness(2);
                _mnuItemPaste.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                _txtMessageContextMenu.Items.Add(_mnuItemPaste);

                MenuItem _mnuItemSelectAll = new MenuItem();
                _mnuItemSelectAll.Header = "Select All";
                _mnuItemSelectAll.Margin = new System.Windows.Thickness(2);
                _mnuItemSelectAll.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                _txtMessageContextMenu.Items.Add(_mnuItemSelectAll);

                MenuItem _mnuItemSpellCheck = new MenuItem();
                _mnuItemSpellCheck.Header = "Enable Spell Checking";
                _mnuItemSpellCheck.Margin = new System.Windows.Thickness(2);
                _mnuItemSpellCheck.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                _txtMessageContextMenu.Items.Add(_mnuItemSpellCheck);
                _txtMessageContextMenu.Style = (Style)FindResource("Contextmenu");

                MenuItem _mnuItemLanguages = new MenuItem();
                _mnuItemLanguages.Header = "Languages";
                _mnuItemLanguages.Margin = new System.Windows.Thickness(2);
                _mnuItemLanguages.Click += new RoutedEventHandler(txtMessageContextMenu_Click);

                AbstractDictionary[] abstractDictionary = _chatUtil.Spellcheck.FindDictionaries("Dictionaries");
                foreach (AbstractDictionary current in abstractDictionary)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += new RoutedEventHandler(this._mnuLang_Click);
                    menuItem.IsCheckable = true;
                    menuItem.Tag = current;
                    ElementToGroupNames.Add(menuItem, "Language");
                    menuItem.Checked += MenuItemChecked;
                    menuItem.Margin = new System.Windows.Thickness(2);
                    menuItem.Header = current.ToString();
                    if (current.Name == "HunSpellDic_en-US")
                    {
                        menuItem.IsChecked = true;
                        _mnuLang_Click(menuItem, null);
                    }
                    _mnuItemLanguages.Items.Add(menuItem);
                }

                _txtMessageContextMenu.Items.Add(_mnuItemLanguages);
                _txtMessageContextMenu.Style = (Style)FindResource("Contextmenu");

                _txtMessageContextMenu.Opened += new RoutedEventHandler(txtMessageContextMenu_Opened);

                _chatUtil.ChatAgentsStatusRowHight = GridLength.Auto;
                _chatUtil.IsButtonSendEnabled = false;
                //_chatDataContext.IsButtonCheckURL = false;
                txtChatSend.Focus();
                ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).SubscribeUpdateNotification(ContactUpdation);

                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Email))
                    {
                        IMessage response1 = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetMediaViceInteractionCount("email", _chatUtil.ContactID, _chatUtil.InteractionID);
                        if (response1 != null)
                        {
                            if (response1 != null && response1.Id == EventCountInteractions.MessageId)
                            {
                                EventCountInteractions eventInteractionListGet = (EventCountInteractions)response1;

                                if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                                {
                                    //BitmapImage bi = new BitmapImage();
                                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                    //bi.BeginInit();
                                    //bi.CacheOption = BitmapCacheOption.OnLoad;
                                    //bi.UriSource = new Uri(_chatDataContext.Imagepath + "\\Email\\Email.png", UriKind.Relative);
                                    //bi.EndInit();
                                    image.Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Email\\Email.png", UriKind.Relative));
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
                                        stk.Children.Add(new TextBlock() { Text = " Email " + "(" + inprogressIXNCount + ")", Padding = new Thickness(2) });
                                        stkPanelIXNCountContent.Children.Add(stk);
                                    }
                                }
                            }
                        }
                    }
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Chat))
                    {
                        IMessage response2 = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetMediaViceInteractionCount("chat", _chatUtil.ContactID, _chatUtil.InteractionID);
                        if (response2 != null)
                        {
                            if (response2 != null && response2.Id == EventCountInteractions.MessageId)
                            {
                                EventCountInteractions eventInteractionListGet = (EventCountInteractions)response2;

                                if (eventInteractionListGet != null && !eventInteractionListGet.TotalCount.IsNull)
                                {
                                    // BitmapImage bi = new BitmapImage();
                                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                    //bi.BeginInit();
                                    //bi.UriSource = new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative);
                                    //bi.EndInit();
                                    image.Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.png", UriKind.Relative));
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
                if (_chatUtil.RemainingDetails != string.Empty)
                    lblRemainingIXN.Visibility = Visibility.Visible;
                else
                    lblRemainingIXN.Visibility = Visibility.Collapsed;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while Window_Loaded() :" + generalException.ToString());
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true); //DataFormats.UnicodeText
            if (!isText) return;
            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
            if (!string.IsNullOrEmpty(text))
                _isPasteText = true;
            //if (Clipboard.ContainsText())
            //{
            //    txtChatSend.Selection.Text = Clipboard.GetText();
            //}
            //e.Handled = true;
        }

        /// <summary>
        /// Handles the Click event of the _mnuLang control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void _mnuLang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    _chatUtil.Spellcheck.SetDictionary(menuItem.Tag as AbstractDictionary);
                    string[] lines = System.IO.File.ReadAllLines(CustomDictionary);
                    if (lines != null && lines.Length > 0)
                        for (int i = 0; i < lines.Length; i++)
                            _chatUtil.Spellcheck.AddTodictionary(lines[i]);
                    var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                    textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                    if (_chatUtil.IsEnableSpellCheck)
                        StartSpellCheck();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while _mnuLang_Click() :" + generalException.ToString());
            }
        }

        void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            foreach (var item in ElementToGroupNames)
            {
                if (item.Key != menuItem)
                {
                    item.Key.IsChecked = false;
                }
            }
        }

        void ChatInteractionSubscriber__ixnFailed()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                    {
                        CloseError(null, null);
                        if (_chatUtil != null)
                            _chatUtil.IsOnChatInteraction = false;
                        ProcessBeforeClosing();
                        _isChatDoneClicked = true;
                        this.Close();
                    }), DispatcherPriority.ContextIdle, new object[0]);
        }

        /// <summary>
        /// Handles the Activated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BitmapEffect = _shadowEffect;
            _chatUtil.MainBorderBrush = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString("#0070C5"));
            WindowInteropHelper h = new WindowInteropHelper(this);
            FlashWindow.Stop(h.Handle);
        }

        /// <summary>
        /// Handles the Deactivated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            _chatUtil.MainBorderBrush = System.Windows.Media.Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        /// <summary>
        /// Handles the Unloaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (typingTimer != null)
                {
                    typingTimer.Tick -= typingTimer_Tick;
                    typingTimer.Stop();
                    typingTimer = null;
                }
                _timerforcloseError = null;

                _shadowEffect = null;
                if (_availablePushURL != null)
                    _availablePushURL.Items.Clear();
                _availablePushURL = null;
                if (_rtbContextMenu != null)
                    _rtbContextMenu.Items.Clear();
                _rtbContextMenu = null;
                if (_txtMessageContextMenu != null)
                    _txtMessageContextMenu.Items.Clear();
                _txtMessageContextMenu = null;
                if (_txtSuggestionContextMenu != null)
                    _txtSuggestionContextMenu.Items.Clear();
                _txtSuggestionContextMenu = null;
                if (_btnChatPersonDataContextMenu != null)
                    _btnChatPersonDataContextMenu.Items.Clear();
                _btnChatPersonDataContextMenu = null;

                InteractionID = null;
                userID = null;
                otherAgentID = null;
                selectedUser = null;
                _chatUtil.Spellcheck = null;
                theComment = null;

                urlRegex = null;
                UrlRegex = null;

                _txtDecorationCollection = null;

                consultInviteParagraph = null;
                CustomDictionary = null;
                _chatDataContext.PushedURLKey = string.Empty;
                mainRTB.Document = new FlowDocument();
                mainRTB.Documents = new FlowDocument();
                if (_chatUtil != null)
                {
                    _chatUtil.PartiesInfo.Clear();
                    _chatUtil.ChatPersonsStatusInfo.Clear();
                    _chatUtil.ChatConsultPersonStatusInfo.Clear();
                    _chatUtil.InteractionNoteContent = string.Empty;
                    _chatUtil.RTBDocument.Blocks.Clear();
                    _chatUtil.ChatParties.Clear();
                    _chatUtil.UserData.Clear();
                    _chatUtil.Dispose();
                    _chatUtil = null;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while Window_Unloaded() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the ChatWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void ChatWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (columnChat.ActualWidth != 0 && columnHistory.ActualWidth != 0)
                {
                    //if (((this.Width - 16) / 2) > (columnButtons.Width.Value / 2))
                    columnChat.Width = new GridLength(((this.Width - 16) / 2) - (columnButtons.Width.Value / 2));
                    //if (((this.Width - 16) / 2) > (columnButtons.Width.Value / 2))
                    columnHistory.Width = new GridLength(((this.Width - 16) / 2) - (columnButtons.Width.Value / 2));
                    _windowMainGridWidth = columnChat.Width.Value;
                    _windowRightGridWidth = columnHistory.Width.Value;
                }
                else
                {
                    if (columnChat.ActualWidth != 0)
                    {
                        columnChat.Width = new GridLength((this.Width - (16 + columnButtons.Width.Value)));
                        _windowMainGridWidth = columnChat.Width.Value;
                    }
                    if (columnHistory.ActualWidth != 0)
                    {
                        columnHistory.Width = new GridLength((this.Width - (16 + columnButtons.Width.Value)));
                        _windowRightGridWidth = columnHistory.Width.Value;
                    }
                }

                _windowFullWidth = this.Width;
                _chatUtil.TitleText = _chatUtil.ChatWindowTitleText;
                try
                {
                    ScrollViewer.UpdateLayout();
                    mainRTB.Height = (Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)) < 151 ? 150 : Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)));
                    ScrollViewer.UpdateLayout();
                    ScrollViewer.ScrollToHome();
                    mainRTB.ScrollToEnd();
                }
                catch
                { }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while ChatWindow_SizeChanged() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the StateChanged event of the ChatWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ChatWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                StateChanged -= ChatWindow_StateChanged;
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Restore, "Restore");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                    mainRTB.Height = 280;
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
                    mainRTB.Height = 500;
                }
                if (WindowState == System.Windows.WindowState.Normal)
                {
                    DeleteMenu(SystemMenu, CU_Restore, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Minimize, MF_BYCOMMAND);
                    DeleteMenu(SystemMenu, CU_Maximize, MF_BYCOMMAND);
                    InsertMenu(SystemMenu, 0, MF_BYPOSITION, CU_Minimize, "Minimize");
                    InsertMenu(SystemMenu, 1, MF_BYPOSITION, CU_Maximize, "Maximize");
                    mainRTB.Height = 280;
                }
                if (_chatWindowState == ChatWindowState.Maximized)
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
                if (_chatWindowState == ChatWindowState.Maximized && WindowState == System.Windows.WindowState.Minimized)
                {

                }
                StateChanged += ChatWindow_StateChanged;
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while ChatWindow_StateChanged() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Closing event of the ChatWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void ChatWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (_chatUtil.IsOnChatInteraction)
                {
                    starttimerforerror("End the interaction before closing the window.");
                    e.Cancel = true;
                }
                else
                {
                    if (!_isChatDoneClicked)
                        MarkDone(e, false);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while btnExit_Click() :" + exception.ToString());
            }
        }

        #endregion Window Events

        void DialPad_ConsultSelectedEvent(Dictionary<string, string> dicValues)
        {
            TeamCommunicatorEventNotify(dicValues);
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            doStopTyping();
        }


        /// <summary>
        /// Handles the Opened event of the _rtbContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void _rtbContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu contextmenu = sender as ContextMenu;
                if (contextmenu != null)
                    foreach (MenuItem mi in contextmenu.Items)
                    {
                        if ((string)mi.Header == "Copy")
                        {
                            if (mainRTB.Document != null)
                            {

                                if (mainRTB.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Select All")
                        {
                            if (mainRTB.Document != null)
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                    }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while _rtbContextMenu_Opened() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the request navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleRequestNavigate(object sender, RoutedEventArgs args)
        {
            try
            {
                if (args is RequestNavigateEventArgs)
                    Process.Start(((RequestNavigateEventArgs)args).Uri.ToString());
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while HandleRequestNavigate() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the PushURLMenuitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void PushURLMenuitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                string urlString = string.Empty;
                if (menuitem != null)
                {
                    if (_chatDataContext.LoadAvailablePushURL.Count(p => p.Key == menuitem.Header.ToString()) > 0)
                    {
                        urlString = _chatDataContext.LoadAvailablePushURL[menuitem.Header.ToString()].ToString();
                        if (urlString != string.Empty)
                            urlString = urlString.Contains("http") == true ? urlString : "http://" + urlString;
                        txtPushURL.Text = urlString;
                        _chatDataContext.PushedURLKey = menuitem.Header.ToString();
                        txtPushURL.Focus();
                        txtPushURL.CaretIndex = txtPushURL.Text.Length;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while PushURLMenuitem_Click() :" + exception.ToString());
            }
        }


        /// <summary>
        /// Handles the Opened event of the _txtMessageContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void txtMessageContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu contextmenu = sender as ContextMenu;
                if (contextmenu != null)
                    foreach (MenuItem mi in contextmenu.Items)
                    {
                        if ((string)mi.Header == "Paste")
                        {
                            if (Clipboard.ContainsText())
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Copy")
                        {
                            if (txtChatSend.Document.Blocks != null)
                            {
                                if (txtChatSend.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Cut")
                        {
                            if (txtChatSend.Document.Blocks != null)
                            {
                                if (txtChatSend.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Select All")
                        {
                            if (txtChatSend.Document.Blocks != null)
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Enable Spell Checking")
                        {
                            _chatUtil.IsEnableSpellCheck = false;
                        }
                        if ((string)mi.Header == "Disable Spell Checking")
                        {
                            _chatUtil.IsEnableSpellCheck = true;
                        }
                    }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while txtMessageContextMenu_Opened() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the txtMessageContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void txtMessageContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                if (menuitem == null) return;
                switch (menuitem.Header.ToString())
                {
                    case "Cut":
                        txtChatSend.Cut();
                        break;
                    case "Copy":
                        txtChatSend.Copy();
                        break;
                    case "Paste":
                        txtChatSend.Paste();
                        break;

                    case "Select All":
                        txtChatSend.SelectAll();
                        break;
                    case "Enable Spell Checking":
                        _chatUtil.IsEnableSpellCheck = true;
                        menuitem.Header = "Disable Spell Checking";
                        StartSpellCheck();
                        break;
                    case "Disable Spell Checking":
                        _chatUtil.IsEnableSpellCheck = false;
                        _chatUtil.Spellcheck.IgnoredWords.Clear();
                        _chatUtil.Spellcheck.MisspelledWords.Clear();
                        menuitem.Header = "Enable Spell Checking";
                        StopSpellCheck();
                        break;
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while txtMessageContextMenu_Click() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the rtbContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void rtbContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        switch (menuitem.Header.ToString())
                        {
                            case "Copy":
                                mainRTB.Copy();
                                break;
                            case "Select All":
                                mainRTB.SelectAll();
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while rtbContextMenu_Click() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnChatPersonDataContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void btnChatPersonDataContextMenu_Click(object sender, RoutedEventArgs e)
        {
            ChatMedia chatMedia = new ChatMedia();
            try
            {
                OutputValues output = null;
                string agentNickName = string.Empty;
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Delete from Conference")
                        {
                            ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = _chatUtil.PartiesInfo;
                            var getUserNickName = temp.Where(x => x.UserNickName == selectedUser).ToList();
                            if (getUserNickName.Count > 0)
                            {
                                foreach (var item in getUserNickName)
                                {
                                    userID = item.UserID;
                                    agentNickName = item.UserNickName;
                                }
                            }
                            else
                            {
                                var getTypeName = temp.Where(x => x.UserType == ChatDataContext.ChatUsertype.Client.ToString()).ToList();
                                if (getTypeName.Count > 0)
                                    foreach (var item in getTypeName)
                                    {
                                        userID = item.UserID;
                                        agentNickName = item.UserNickName;
                                    }
                            }
                            if (userID != string.Empty)
                            {
                                if (_chatUtil.IsChatConferenceClick)
                                {
                                    output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                    _chatUtil.IsChatConferenceClick = false;
                                }
                                else
                                {
                                    output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                                }
                                if (output.MessageCode == "200" && _chatDataContext.AgentNickName == agentNickName)
                                {
                                    _chatUtil.ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                                    _chatUtil.IsEnableRelease = false;
                                    _chatUtil.TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                                    _chatUtil.IsEnableTransfer = false;
                                    _chatUtil.ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                                    _chatUtil.IsEnableConference = false;
                                    _chatUtil.ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                                    _chatUtil.IsEnableChatConsult = false;
                                    _chatUtil.DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                                    _chatUtil.IsEnableDone = true;
                                    _chatUtil.VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                                    _chatUtil.IsEnableVoiceConsult = false;
                                    _chatUtil.IsTextMessageEnabled = false;
                                    _chatUtil.IsTextURLEnabled = false;
                                    _chatUtil.IsButtonSendEnabled = false;
                                    _chatUtil.IsButtonCheckURL = false;
                                    _chatUtil.IsButtonAvailableURL = false;
                                    _chatUtil.IsButtonPushURLExpander = false;
                                    _chatUtil.IsConversationRTBEnabled = true;
                                }

                                //added for delete user after delete from conference
                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = _chatUtil.ChatPersonsStatusInfo;
                                var getUser = tempChatStatus.Where(x => x.ChatPersonName == agentNickName).ToList();
                                if (getUser.Count > 0)
                                {
                                    foreach (var data in getUser)
                                    {
                                        _chatUtil.ChatPersonsStatusInfo.Remove(data);
                                    }
                                }
                                //end
                                userID = string.Empty;
                                selectedUser = string.Empty;
                            }
                        }
                        else if (menuitem.Header.ToString().Contains("Call "))
                        {
                            var phoneNumber = menuitem.Tag.ToString();
                            phoneNumber = phoneNumber.Trim();
                            if (ChatDataContext.messageToClientChat != null)
                            {
                                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, phoneNumber);
                                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, "DIAL");
                            }
                        }
                        else if (menuitem.Header.ToString().Contains("E-mail"))
                        {
                            if (IsEmailReachMaximumCount())
                            {
                                starttimerforerror("Email reached maximum count. Please close opened mail and then try to open.");
                                return;
                            }
                            if (!string.IsNullOrEmpty(menuitem.Tag as string) && Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                                ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyNewEmail(menuitem.Tag as string, null);

                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while btnChatPersonDataContextMenu_Click() :" + exception.ToString());
            }
            finally
            {
                chatMedia = null;
            }
        }

        public bool IsEmailReachMaximumCount()
        {
            int maximumEmailCount = 5;
            if (ConfigContainer.Instance().AllKeys.Contains("email.max.intstance-count"))
                int.TryParse(((string)ConfigContainer.Instance().GetValue("email.max.intstance-count")), out maximumEmailCount);
            List<Window> emailWindows = Application.Current.Windows.Cast<Window>().Where(x => x.Title.Equals("Email")).ToList();
            if (emailWindows.Count == maximumEmailCount)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Mouses the left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_chatWindowState != ChatWindowState.Maximized && _chatWindowState != ChatWindowState.Minimized)
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
            catch (Exception commonException)
            {
                _logger.Warn(" MouseLeftButtonDown() : Warn :" + commonException.ToString());
            }
        }

        #region System Menu

        private const Int32 WM_SYSCOMMAND = 0x112;
        private const int CU_Minimize = 1000;
        private const int CU_Normal = 1001;
        private const int CU_Close = 1002;
        private const int CU_Restore = 1003;
        private const int CU_Maximize = 1004;
        private const int SC_Minimize = 0x0000f020;
        private const int SC_Maximize = 0x0000f030;
        private const int SC_Restore = 0x0000f120;
        private const int SC_Close = 0x0000f060;
        private const int SC_Size = 0x0000f000;
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_Move = 0x0000f010;
        private const int MF_GRAYED = 0x1;
        private const int MF_DISABLED = 0x2;
        private const int MF_ENABLED = 0x0;
        public const Int32 MF_BYPOSITION = 0x400;
        private IntPtr SystemMenu;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, Int32 uIDEnableItem, Int32 uEnable);

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
                    else if (_chatWindowState == ChatWindowState.Maximized)
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

        #endregion

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
                //WindowState = WindowState.Minimized;
                //_chatWindowState = ChatWindowState.Minimized;
                Topmost = false;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred while btnMinimize_Click() :" + exception.ToString());
            }
        }
        double tempWidth;
        double tempHeight;
        double tempLeft;
        double tempTop;
        enum ChatWindowState { Normal, Minimized, Maximized };
        private ChatWindowState _chatWindowState;

        /// <summary>
        /// Handles the Click event of the btnMaximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged -= ChatWindow_SizeChanged;
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    WindowState = System.Windows.WindowState.Normal;
                }
                if (_chatWindowState == ChatWindowState.Normal)
                {
                    btnMaximize.Style = FindResource("RestoreButton") as Style;
                    tempWidth = this.ActualWidth;
                    tempHeight = this.ActualHeight;
                    tempLeft = this.Left;
                    tempTop = this.Top;
                    _chatWindowState = ChatWindowState.Maximized;
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
                    if (ToolHeading.Text == "Show")
                    {
                        columnChat.Width = new GridLength(0);
                        columnChat.MinWidth = 0;
                        columnHistory.Width = new GridLength(this.Width - 20);
                    }
                    else if (btnResponses.IsChecked == false && btnContacts.IsChecked == false && btnData.IsChecked == false)
                    {
                        columnHistory.Width = new GridLength(0);
                        columnHistory.MinWidth = 0;
                        columnChat.Width = new GridLength(this.Width - 20);
                    }
                    else
                    {
                        var mathSize = (this.Width - 20) / 2;
                        columnChat.Width = new GridLength(mathSize);
                        columnHistory.Width = new GridLength(mathSize);
                    }

                    RightSideRect.Visibility = Visibility.Hidden;
                    RightbottomSideRect.Visibility = Visibility.Hidden;
                    BottomSideRect.Visibility = Visibility.Hidden;
                }
                else if (_chatWindowState == ChatWindowState.Maximized)
                {
                    _chatWindowState = ChatWindowState.Normal;
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

                    if (ToolHeading.Text == "Show")
                    {
                        columnChat.Width = new GridLength(0);
                        columnChat.MinWidth = 0;
                        columnHistory.MinWidth = 400;
                        columnHistory.Width = new GridLength(this.Width - (20 + 16));
                    }
                    else if (btnResponses.IsChecked == false && btnContacts.IsChecked == false && btnData.IsChecked == false)
                    {
                        columnHistory.Width = new GridLength(0);
                        columnHistory.MinWidth = 0;
                        columnChat.MinWidth = 400;
                        columnChat.Width = new GridLength(this.Width - (20 + 16));
                    }
                    else
                    {
                        var mathSize = (this.Width - (20 + 16)) / 2;
                        columnChat.MinWidth = 400;
                        columnHistory.MinWidth = 400;
                        columnChat.Width = new GridLength(mathSize);
                        columnHistory.Width = new GridLength(mathSize);
                    }

                    RightSideRect.Visibility = Visibility.Visible;
                    RightbottomSideRect.Visibility = Visibility.Visible;
                    BottomSideRect.Visibility = Visibility.Visible;
                }
                SizeChanged += ChatWindow_SizeChanged;

                try
                {
                    ScrollViewer.UpdateLayout();
                    mainRTB.Height = (Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)) < 151 ? 150 : Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)));
                    ScrollViewer.UpdateLayout();
                    ScrollViewer.ScrollToHome();
                    mainRTB.ScrollToEnd();
                }
                catch { }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while btnMaximize_Click() :" + exception.ToString());
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
                if (_chatUtil.IsOnChatInteraction)
                {
                    starttimerforerror("End the interaction before closing the window.");
                }
                else
                {
                    if (!_isChatDoneClicked)
                        btnChatDone_Click(null, null);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while btnExit_Click() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnChatConsultation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnVoiceConsultation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dialpad != null)
                {
                    dialpad = null;
                }
                dialpad = new DialPad(_chatUtil);
                var grid = new Grid();
                grid.Background = System.Windows.Media.Brushes.White;
                grid.Children.Add(dialpad);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = System.Windows.Media.Brushes.Transparent;
                menuConsultItem.Header = grid;
                menuConsultItem.Margin = new Thickness(-12, -1, -18, -3);
                menuConsultItem.Width = Double.NaN;
                _chatDataContext.cmshow.Items.Clear();
                _chatDataContext.cmshow.Items.Add(menuConsultItem);
                _chatDataContext.cmshow.PlacementTarget = btnVoiceConsultation;
                _chatDataContext.cmshow.Placement = PlacementMode.Bottom;
                _chatDataContext.cmshow.IsOpen = true;
                _chatDataContext.cmshow.StaysOpen = true;
                _chatDataContext.cmshow.Focus();
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while btnChatConsultation_Click() :" + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnChatConference control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChatConference_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl userControl = TeamCommunicatorConference();
                ShowTeamCommunicator(userControl, btnChatConference);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatConference_Click() :" + generalException.ToString());
            }
        }

        private void btnChatConsult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl userControl = TeamCommunicatorConsult();
                ShowTeamCommunicator(userControl, btnChatConsult);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatConsult_Click() :" + generalException.ToString());
            }
        }

        private void btnCloseError_Click(object sender, RoutedEventArgs e)
        {
            CloseError(null, null);
        }

        void starttimerforerror(string errorMessage)
        {
            try
            {
                if (_timerforcloseError == null)
                {
                    _timerforcloseError = new DispatcherTimer();
                    _timerforcloseError.Interval = TimeSpan.FromSeconds(10);
                    _timerforcloseError.Tick += new EventHandler(CloseError);
                    _timerforcloseError.Start();
                    _chatUtil.ErrorRowHeight = GridLength.Auto;
                    _chatUtil.ErrorMessage = errorMessage;
                }
                else
                {
                    CloseError(null, null);
                    starttimerforerror(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("starttimerforerror() : " + ex.Message);
            }

        }

        void CloseError(object sender, EventArgs e)
        {
            try
            {
                if (_chatUtil != null)
                {
                    _chatUtil.ErrorRowHeight = new GridLength(0);
                    _chatUtil.ErrorMessage = string.Empty;
                }
                if (_timerforcloseError != null)
                {
                    _timerforcloseError.Stop();
                    _timerforcloseError.Tick -= CloseError;
                    _timerforcloseError = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CloseError() : " + ex.Message);
            }

        }

        private void ShowTeamCommunicator(UserControl userControl, UIElement uiElement)
        {
            if (userControl != null)
            {
                userControl.Unloaded += new RoutedEventHandler(userControl_Unloaded);
                _chatDataContext.cmshow = new System.Windows.Controls.ContextMenu();
                _chatDataContext.cmshow.Style = (Style)FindResource("Contextmenu");
                var parent = FindAncestor<Grid>(userControl);
                if (parent != null)
                    parent.Children.Clear();

                Grid grid1 = new Grid();
                grid1.Background = System.Windows.Media.Brushes.White;
                grid1.Children.Add(userControl);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = System.Windows.Media.Brushes.Transparent;
                menuConsultItem.Header = grid1;
                menuConsultItem.Margin = new Thickness(-13, -6, -18, -5);
                menuConsultItem.Width = userControl.Width;
                menuConsultItem.Height = userControl.Height;
                _chatDataContext.cmshow.Items.Clear();
                _chatDataContext.cmshow.Items.Add(menuConsultItem);
                _chatDataContext.cmshow.PlacementTarget = uiElement;
                _chatDataContext.cmshow.Placement = PlacementMode.Bottom;
                _chatDataContext.cmshow.IsOpen = true;
                _chatDataContext.cmshow.StaysOpen = true;
                _chatDataContext.cmshow.Focus();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnChatRelease control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChatRelease_Click(object sender, RoutedEventArgs e)
        {
            ChatMedia chatMedia = new ChatMedia();
            try
            {
                _chatUtil.IsChatReleaseClick = true;
                OutputValues output = null;
                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = _chatUtil.PartiesInfo;
                var getUserNickName = temp.Where(x => x.UserNickName == _chatDataContext.AgentNickName).ToList();
                if (getUserNickName.Count > 0)
                    foreach (var item in getUserNickName)
                    {
                        userID = item.UserID;
                        visibility = item.Visibility;
                    }
                if (userID != string.Empty)
                {
                    if ((((string)ConfigContainer.Instance().GetValue("chat.enable.prompt-for-done")).ToLower().Equals("true")))
                    {
                        var showMessageBox = new MessageBox("Information",
                                      "Are you sure that you want to end this conversation ?", "OK", "Cancel", false);
                        showMessageBox.Owner = this;
                        showMessageBox.ShowDialog();
                        if (showMessageBox.DialogResult == true)
                        {
                            showMessageBox.Dispose();
                            if (_chatUtil.IsChatConferenceClick)
                            {
                                var getOtherUserNickName = temp.Where(x => x.UserType == ChatDataContext.ChatUsertype.Agent.ToString() && x.Visibility == "Int").ToList();
                                if (getOtherUserNickName != null && getOtherUserNickName.Count > 0)
                                {
                                    foreach (var item in getOtherUserNickName)
                                    {
                                        output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, item.UserID);
                                    }
                                    //if (visibility == "All")
                                    //{
                                    //    output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                    //}
                                }
                                else
                                {
                                    //output = chatMedia.DoLeaveInteractionFromConference(InteractionID, _chatUtil.ProxyId, userID);
                                    //if (output.MessageCode == "200")
                                    output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                }
                                _chatUtil.IsChatConferenceClick = false;
                            }
                            else
                                output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                        }
                    }
                    else
                    {
                        if (_chatUtil.IsChatConferenceClick)
                        {
                            var getOtherUserNickName = temp.Where(x => x.UserType == ChatDataContext.ChatUsertype.Agent.ToString() && x.Visibility == "Int").ToList();
                            if (getOtherUserNickName != null && getOtherUserNickName.Count > 0)
                            {
                                foreach (var item in getOtherUserNickName)
                                {
                                    output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, item.UserID);
                                }
                                //if (visibility == "All")
                                //{
                                //    output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                //}
                            }
                            else
                            {
                                //output = chatMedia.DoLeaveInteractionFromConference(InteractionID, _chatUtil.ProxyId, userID);
                                //if (output.MessageCode == "200")
                                output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                            }
                            _chatUtil.IsChatConferenceClick = false;
                        }
                        else
                            output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                    }
                    if (output.MessageCode == "200")
                    {
                        _chatUtil.ReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Release.Disable.png", UriKind.Relative));
                        _chatUtil.IsEnableRelease = false;
                        _chatUtil.TransImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Transfer.Disable.png", UriKind.Relative));
                        _chatUtil.IsEnableTransfer = false;
                        _chatUtil.ConfImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Conference.Disable.png", UriKind.Relative));
                        _chatUtil.IsEnableConference = false;
                        _chatUtil.ConsultChatImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Disable.png", UriKind.Relative));
                        _chatUtil.IsEnableChatConsult = false;
                        _chatUtil.DoneImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.MarkDone.png", UriKind.Relative));
                        _chatUtil.IsEnableDone = true;
                        _chatUtil.VoiceConsultImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Chat\\Chat.Consult.Call.Disable.png", UriKind.Relative));
                        _chatUtil.IsEnableVoiceConsult = false;
                        _chatUtil.IsTextMessageEnabled = false;
                        _chatUtil.IsTextURLEnabled = false;
                        _chatUtil.IsButtonSendEnabled = false;
                        _chatUtil.IsButtonCheckURL = false;
                        _chatUtil.IsButtonAvailableURL = false;
                        _chatUtil.IsButtonPushURLExpander = false;
                        _chatUtil.IsConversationRTBEnabled = true;
                        userID = string.Empty;
                        CloseError(null, null);
                        _chatUtil.IsOnChatInteraction = false;
                        lblTabItemShowTimer.Stop_CustomTimer();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatRelease_Click() :" + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
            }
        }

        private UserControl TeamCommunicatorConsult()
        {
            UserControl userControl = null;
            try
            {
                CloseError(null, null);
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.Chat);
                container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Consult);
                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                container.ComposeParts(_chatDataContext.ImportClass);

                userControl = (from d in _chatDataContext.ImportClass.win
                               where d.Name == "TeamCommunicator"
                               select d).FirstOrDefault() as UserControl;

                if (userControl != null)
                {
                    return userControl;
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return userControl;
        }

        private UserControl TeamCommunicatorTransfer()
        {
            UserControl userControl = null;
            try
            {
                CloseError(null, null);
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.Chat);
                container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Transfer);
                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                container.ComposeParts(_chatDataContext.ImportClass);

                userControl = (from d in _chatDataContext.ImportClass.win
                               where d.Name == "TeamCommunicator"
                               select d).FirstOrDefault() as UserControl;

                if (userControl != null)
                {
                    return userControl;
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return userControl;
        }

        private UserControl TeamCommunicatorConference()
        {
            UserControl userControl = null;
            try
            {
                CloseError(null, null);
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Plugins");
                DirectoryCatalog catalog;
                CompositionContainer container;

                catalog = new DirectoryCatalog(path);
                container = new CompositionContainer(catalog);
                container.ComposeExportedValue("InteractionType", Pointel.Interactions.IPlugins.InteractionType.Chat);
                container.ComposeExportedValue("OperationType", Pointel.Interactions.IPlugins.OperationType.Conference);
                container.ComposeExportedValue("RefFunction", (Func<Dictionary<string, string>, string>)TeamCommunicatorEventNotify);
                container.ComposeParts(_chatDataContext.ImportClass);

                userControl = (from d in _chatDataContext.ImportClass.win
                               where d.Name == "TeamCommunicator"
                               select d).FirstOrDefault() as UserControl;

                if (userControl != null)
                {
                    return userControl;
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                return null;
            }
            return userControl;
        }
        private string TeamCommunicatorEventNotify(Dictionary<string, string> dictionaryValues)
        {
            var chatMedia = new ChatMedia();
            try
            {
                if (dictionaryValues != null && dictionaryValues.Count > 0)
                {
                    string InteractionType = "";
                    string OperationType = "";
                    string SearchedType = "";
                    string Place = "";
                    string SearchedValue = "";
                    string employeeID = "";
                    //string ixnQueue = "";

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
                    if (dictionaryValues.ContainsKey("EmployeeId"))
                        employeeID = dictionaryValues["EmployeeId"];
                    //if (dictionaryValues.ContainsKey(""))
                    //    ixnQueue = dictionaryValues[""];
                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = (_chatDataContext.MainWindowSession[InteractionID] as ChatUtil).PartiesInfo;

                    var getConsultUsers = temp.Where(x => x.UserType == "Agent" && x.PersonId == employeeID && x.UserState == "Connected" && x.Visibility == "Int").ToList();
                    theComment = string.Empty;
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            if (_chatDataContext.cmshow.IsOpen)
                                _chatDataContext.cmshow.IsOpen = false;
                            if (_chatDataContext.cmshow.StaysOpen)
                                _chatDataContext.cmshow.StaysOpen = false;
                        }));
                        switch (OperationType)
                        {
                            case "Transfer":
                                theComment = "Transferred on " + DateTime.Now.ToString() + " by " + _chatUtil.UserName + " - ";
                                _logger.Info(theComment);
                                try
                                {
                                    if (string.IsNullOrEmpty(_chatUtil.InteractionNoteContent))
                                        _chatUtil.InteractionNoteContent = theComment;
                                    else
                                        _chatUtil.InteractionNoteContent += "\r\n" + theComment;
                                }
                                catch { }
                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = _chatUtil.ChatPersonsStatusInfo;
                                var getClient = tempChatStatus.Where(x => x.ChatPersonName == _chatUtil.ChatFromPersonName).ToList();
                                foreach (var data in getClient)
                                {
                                    int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                    _chatUtil.ChatPersonsStatusInfo.RemoveAt(position1);
                                    _chatUtil.ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(string.Empty, string.Empty, _chatUtil.ChatFromPersonName,
                                    _chatUtil.ChatPersonStatusIcon, "Transferring"));
                                }
                                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                    ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                        _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                                        _chatUtil.UserData, 2);

                                //if (getConsultUsers != null && getConsultUsers.Count > 0 && SearchedType.ToString().Equals("Agent"))
                                //{
                                //    consultChatMakeTransferConf(OperationType, employeeID);
                                //    return string.Empty;
                                //}
                                //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                //{
                                OutputValues output = null;
                                if (SearchedType.ToString().Equals("Agent"))
                                    output = chatMedia.DoChatTransferInteraction(InteractionID, employeeID, Place, _chatUtil.ProxyId, null, _chatUtil.UserData);
                                else if (!string.IsNullOrEmpty(SearchedValue) && SearchedType.ToString().Equals("InteractionQueue"))
                                    output = chatMedia.DoChatTransferInteraction(InteractionID, employeeID, Place, _chatUtil.ProxyId, SearchedValue, _chatUtil.UserData);
                                if (output.MessageCode == "200")
                                {
                                    if (!_chatDataContext.MainWindowSession.ContainsKey(InteractionID))
                                        goto closewin;
                                    var userID = (temp.Where(x => x.UserNickName == _chatDataContext.AgentNickName).FirstOrDefault()).UserID;
                                    if (userID != string.Empty)
                                    {
                                        output = chatMedia.DoKeepAliveReleasePartyChatSession((_chatDataContext.MainWindowSession[InteractionID] as ChatUtil).SessionID, (_chatDataContext.MainWindowSession[InteractionID] as ChatUtil).ProxyId, userID);
                                        if (output.MessageCode == "200")
                                        {
                                            //foreach (var data in getClient)
                                            //{
                                            //    int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                            //    (_chatDataContext.MainWindowSession[InteractionID] as ChatUtil).ChatPersonsStatusInfo.RemoveAt(position1);
                                            //    (_chatDataContext.MainWindowSession[InteractionID] as ChatUtil).ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(data.AgentID, data.PlaceID, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatFromPersonName, (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).ChatPersonStatusIcon, "Connected"));
                                            //}
                                        }
                                    }
                                // (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).TransferReleavePersonId = string.Empty;
                                //}

                                closewin:
                                    _chatUtil.IsChatTransferClick = true;
                                    CloseError(null, null);
                                    _chatUtil.IsOnChatInteraction = false;
                                    ProcessBeforeClosing();
                                    this.Close();
                                }
                                else
                                {
                                    _chatUtil.IsChatTransferClick = false;
                                    if (_chatUtil.InteractionNoteContent.Contains(theComment) || _chatUtil.InteractionNoteContent.Trim().Equals(theComment.Trim()) && !string.IsNullOrEmpty(theComment))
                                    {
                                        _chatUtil.InteractionNoteContent = _chatUtil.InteractionNoteContent.Replace(theComment.Trim(), string.Empty);
                                        ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                   _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                                   _chatUtil.UserData, 2);
                                    }
                                    foreach (var data in getClient)
                                    {
                                        int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                                        if (position1 >= 0)
                                        {
                                            _chatUtil.ChatPersonsStatusInfo.RemoveAt(position1);
                                            _chatUtil.ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(string.Empty, string.Empty, _chatUtil.ChatFromPersonName, _chatUtil.ChatPersonStatusIcon, "Connected"));
                                        }
                                    }
                                    if (output.Message.Contains("No answer"))
                                        starttimerforerror("The target did not answer - Warning");
                                    else
                                        starttimerforerror(output.Message.ToString());

                                }
                                // }));
                                break;
                            case "Conference":
                                try
                                {
                                    theComment = "Conference established on " + DateTime.Now.ToString() + " by " + _chatUtil.UserName + " - ";
                                    _logger.Info(theComment);
                                    if (string.IsNullOrEmpty(_chatUtil.InteractionNoteContent))
                                        _chatUtil.InteractionNoteContent = theComment;
                                    else
                                        _chatUtil.InteractionNoteContent += "\r\n" + theComment;
                                }
                                catch { }
                                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                    ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                        _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent, _chatUtil.UserData, 2);

                                //if (getConsultUsers != null && getConsultUsers.Count > 0 && SearchedType.ToString().Equals("Agent"))
                                //{
                                //    consultChatMakeTransferConf(OperationType, employeeID);
                                //}
                                //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                //{
                                OutputValues output1 = null;
                                if (SearchedType.ToString().Equals("Agent"))
                                    output1 = chatMedia.DoChatConferenceInteraction(InteractionID, employeeID, Place, null, _chatUtil.ProxyId, _chatUtil.UserData);
                                else if (!string.IsNullOrEmpty(SearchedValue) && SearchedType.ToString().Equals("InteractionQueue"))
                                    output1 = chatMedia.DoChatConferenceInteraction(InteractionID, employeeID, Place, SearchedValue, _chatUtil.ProxyId, _chatUtil.UserData);
                                if (output1.MessageCode != "200")
                                {
                                    if (_chatUtil.InteractionNoteContent.Contains(theComment) || _chatUtil.InteractionNoteContent.Trim().Equals(theComment.Trim()) && !string.IsNullOrEmpty(theComment))
                                    {
                                        _chatUtil.InteractionNoteContent = _chatUtil.InteractionNoteContent.Replace(theComment.Trim(), string.Empty);
                                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                            ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                                _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent, _chatUtil.UserData, 2);
                                    }
                                    if (output1.Message.Contains("No answer"))
                                        starttimerforerror("The target did not answer - Warning");
                                    else
                                        starttimerforerror(output1.Message.ToString());
                                }
                                if (output1.MessageCode == "200")
                                {
                                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                        ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                            _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent, _chatUtil.UserData, 2);
                                }
                                // }));
                                break;
                            case "Consult":
                                _logger.Info("Chat Consultation to " + employeeID);
                                consultInviteParagraph.Inlines.Clear();
                                consultInviteParagraph.FontStyle = FontStyles.Italic;
                                consultInviteParagraph.Inlines.Add(new Run("Invitation sent to " + employeeID));
                                consultInviteParagraph.Foreground = System.Windows.Media.Brushes.LightGray;
                                _chatUtil.RTBDocument.Blocks.Add(consultInviteParagraph);
                                OutputValues output3 = chatMedia.DoChatConsultInteraction(InteractionID, employeeID, "", _chatUtil.ProxyId, _chatUtil.UserData);
                                if (output3.MessageCode == "200")
                                {
                                    _chatUtil.ISConsultChatInitialized = true;
                                    _chatUtil.RTBDocument.Blocks.Remove(consultInviteParagraph);
                                }
                                else
                                {
                                    _chatUtil.RTBDocument.Blocks.Remove(consultInviteParagraph);
                                    if (output3.Message.Contains("No answer"))
                                        starttimerforerror("The target did not answer - Warning");
                                    else
                                        starttimerforerror(output3.Message.ToString());

                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while TeamCommunicatorEventNotify() :" + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
            }
            return string.Empty;
        }

        //Active Consultation for make two step transfer or conference
        private void consultChatMakeTransferConf(string operationType, string consultAgentID)
        {
            KeyValueCollection caseData = new KeyValueCollection();
            var chatMedia = new ChatMedia();
            string _operationType = string.Empty;
            if (operationType == "Conference")
                _operationType = "completeConference";
            else
                _operationType = "completeTransfer";
            caseData.Add(_operationType, consultAgentID);
            OutputValues output = chatMedia.DoAddCaseDataProperties(InteractionID, _chatUtil.ProxyId, caseData);
            if (output.MessageCode == "200")
            {
                _chatUtil.UserData.Add(_operationType, consultAgentID);
                _chatUtil.NotifyCaseData.Add(new ChatCaseData(_operationType, consultAgentID));
            }
            caseData.Clear();
            caseData = null;
            _operationType = null;
        }

        /// <summary>
        /// Handles the Click event of the btnChatTransfer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChatTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseError(null, null);
                UserControl userControl = TeamCommunicatorTransfer();
                ShowTeamCommunicator(userControl, btnChatTransfer);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatTransfer_Click() :" + generalException.ToString());
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
                //if (_chatDataContext.cmshow.IsOpen)
                //    _chatDataContext.cmshow.IsOpen = false;
                //if (_chatDataContext.cmshow.StaysOpen)
                //    _chatDataContext.cmshow.StaysOpen = false;
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while userControl_Unloaded() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Finds the ancestor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current">The current.</param>
        /// <returns></returns>
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
        /// Handles the Click event of the btnChatDone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChatDone_Click(object sender, RoutedEventArgs e)
        {
            MarkDone();
        }

        /// <summary>
        /// Marks the done.
        /// </summary>
        /// <param name="isWindowClose">if set to <c>true</c> [is window close].</param>
        private void MarkDone(CancelEventArgs e = null, bool isWindowClose = true)
        {
            ChatMedia chatMedia = new ChatMedia();
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        _isChatDoneClicked = true;
                        try
                        {
                            if (!(((string)ConfigContainer.Instance().GetValue("interaction.disposition.is-mandatory")).ToLower().Equals("true")))
                            {
                                int userCount = 0;
                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = _chatUtil.PartiesInfo;
                                var getUsers = temp.Where(x => x.UserType == "Agent").ToList();
                                if (getUsers.Count > 0)
                                {
                                    foreach (var item in getUsers)
                                    {
                                        if (item.UserNickName != _chatDataContext.AgentNickName && item.UserState == "Connected")
                                        {
                                            _isAgentAliveinChatIXN = true;
                                            userCount++;
                                        }
                                        else
                                        {
                                            _isAgentAliveinChatIXN = false;
                                        }
                                    }
                                }
                                else
                                {
                                    _isAgentAliveinChatIXN = false;
                                }
                                if (_isAgentAliveinChatIXN && userCount > 1)
                                {
                                    CloseError(null, null);
                                    _chatUtil.IsOnChatInteraction = false;
                                    ProcessBeforeClosing();
                                    if (isWindowClose)
                                        this.Close();
                                }
                                else
                                {
                                    OutputValues output = chatMedia.DoStopInteraction(InteractionID, _chatUtil.ProxyId);
                                    if (output.MessageCode == "200")
                                    {
                                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                            ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                                _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                                                _chatUtil.UserData, 3, DateTime.UtcNow.ToString());
                                        CloseError(null, null);
                                        _chatUtil.IsOnChatInteraction = false;
                                        ProcessBeforeClosing();
                                        if (isWindowClose)
                                            this.Close();
                                    }
                                }
                            }
                            else
                            {
                                if (!_chatUtil.IsChatTransferClick)
                                    if (!_chatUtil.IsDispositionSelected && _chatUtil.IxnType != "Consult")
                                    {
                                        if (_chatDataContext.LoadDispositionCodes.Count > 0)
                                        {
                                            if (e != null)
                                            {
                                                e.Cancel = true;
                                                _isChatDoneClicked = false;
                                            }
                                            var msg = new MessageBox("Warning", "Disposition code is mandatory.", "", "_Ok", false);
                                            msg.Owner = this;
                                            msg.ShowDialog();
                                            if (msg.DialogResult != true)
                                            {
                                                msg.Dispose();
                                                return;
                                            }
                                            else
                                            {
                                                if (btnData.Tag.ToString() == "Show")
                                                {
                                                    btnData.IsChecked = true;
                                                    btnData_Click(null, null);
                                                }
                                                IEnumerable<Pointel.Interactions.Chat.UserControls.InteractionData> collection = grdUC.Children.OfType<Pointel.Interactions.Chat.UserControls.InteractionData>();
                                                if (collection != null)
                                                    foreach (var data in collection)
                                                    {
                                                        var obj = (Pointel.Interactions.Chat.UserControls.InteractionData)data;
                                                        if (obj != null)
                                                            obj.tabitemDisposition.IsSelected = true;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            goto common;
                                        }
                                    }
                                    else
                                    {
                                        goto common;
                                    }

                            }
                            return;
                        common:
                            {
                                ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = _chatUtil.PartiesInfo;
                                var getUsers = temp.Where(x => x.UserType == "Agent").ToList();
                                if (getUsers.Count > 0)
                                {
                                    foreach (var item in getUsers)
                                    {
                                        if (item.UserNickName != _chatDataContext.AgentNickName && item.UserState == "Connected")
                                        {
                                            _isAgentAliveinChatIXN = true;
                                        }
                                        else
                                        {
                                            _isAgentAliveinChatIXN = false;
                                        }
                                    }
                                }
                                else
                                {
                                    _isAgentAliveinChatIXN = false;
                                }
                                if (_isAgentAliveinChatIXN)
                                {
                                    CloseError(null, null);
                                    _chatUtil.IsOnChatInteraction = false;
                                    ProcessBeforeClosing();
                                    if (isWindowClose)
                                        this.Close();
                                }
                                else
                                {
                                    OutputValues output = chatMedia.DoStopInteraction(InteractionID, _chatUtil.ProxyId);
                                    if (output.MessageCode == "200")
                                    {
                                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                            ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(InteractionID,
                                                _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                                                _chatUtil.UserData, 3, DateTime.UtcNow.ToString());
                                        CloseError(null, null);
                                        _chatUtil.IsOnChatInteraction = false;
                                        ProcessBeforeClosing();
                                        if (isWindowClose)
                                            this.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception generalException)
                        {
                            _logger.Error(" Error occurred while btnChatDone_Click() :" + generalException.ToString());
                        }
                        finally
                        {
                            chatMedia = null;
                        }
                    }));
        }

        public void ProcessBeforeClosing()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                   {
                       try
                       {
                           int count = 0;
                           foreach (var item in Application.Current.Windows)
                           {
                               if (item is ChatMainWindow)
                                   if (_chatDataContext.MainWindowSession.ContainsKey((item as ChatMainWindow).Tag.ToString()))
                                       count++;
                           }
                           if (count == 1)
                           {
                               if (ChatDataContext.messageToClientChat != null)
                                   ChatDataContext.messageToClientChat.PluginInteractionStatus(IPlugins.PluginType.Chat, Pointel.Interactions.IPlugins.IXNState.Closed);
                               if (_chatDataContext.MainWindowSession.ContainsKey(InteractionID))
                               {
                                   _chatDataContext.MainWindowSession[InteractionID] = null;
                                   _chatDataContext.MainWindowSession.Remove(InteractionID);
                               }
                           }
                       }
                       catch (Exception generalException)
                       {
                           _logger.Error("Error occurred while ProcessBeforeClosing() :" + generalException.ToString());
                       }
                   }));
        }

        /// <summary>
        /// Handles the Click event of the btnSendUrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSendUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_chatUtil.ChatContentRowHeight == new GridLength(0) && btnSendUrl.Tag.ToString() == "Show")
                {
                    _chatUtil.ChatContentRowHeight = GridLength.Auto;
                    _chatUtil.BtnArrowImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\upArrow.png", UriKind.Relative));
                    PushURLContent.Text = "Hide the Push URL area.";
                    btnSendUrl.Tag = "Hide";
                }
                else
                {
                    _chatUtil.ChatContentRowHeight = new GridLength(0);
                    _chatUtil.BtnArrowImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\downArrow.png", UriKind.Relative));
                    PushURLContent.Text = "Show the Push URL area.";
                    btnSendUrl.Tag = "Show";
                    txtPushURL.Text = string.Empty;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnSendUrl_Click() :" + generalException.ToString());
            }
        }

        public static T IsWindowOpen<T>(string name = null) where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(name) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Name.Equals(name));
        }

        public string getDocumentAsXaml(IDocumentPaginatorSource flowDocument)
        {
            string data = string.Empty;
            try
            {
                data = XamlWriter.Save(flowDocument);
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while getDocumentAsXaml() : " + generalException.ToString());
            }
            return data;
        }

        /// <summary>
        /// Checks the push URL is valid.
        /// </summary>
        /// <param name="urlString">The URL string.</param>
        /// <returns></returns>
        private bool checkPushURLIsValid(string urlString)
        {
            try
            {
                Uri tempUri = null;
                if (urlString != string.Empty || urlString != "")
                {
                    if (IsHyperlink(urlString))
                    {
                        tempUri = new Uri(urlString, UriKind.RelativeOrAbsolute);

                        if (!tempUri.IsAbsoluteUri)
                        {
                            tempUri = new Uri(@"http://" + urlString, UriKind.Absolute);
                        }
                        if (tempUri != null && tempUri.ToString() != string.Empty)
                            _chatUtil.ChatNoticeText = tempUri.ToString();
                        return true;
                    }
                    else
                    {
                        starttimerforerror("It is not possible to push the URL '" + urlString + "'");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while checkPushURLIsValid() :" + generalException.ToString());
                return false;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSendMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            ChatMedia chatMedia = new ChatMedia();
            try
            {
                var range = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                string chatText = range.Text;

                if (chatText != string.Empty && txtPushURL.Text != string.Empty && chatText != "\r\n")
                {
                    if (_chatUtil.Spellcheck.MisspelledWords.Count > 0)
                        if ((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                               ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                        {
                            var showMessageBox = new MessageBox("Information",
                                              "Please check the misspelled words", "OK", "Cancel", false);
                            showMessageBox.Owner = this;
                            showMessageBox.ShowDialog();
                            if (showMessageBox.DialogResult == true || showMessageBox.DialogResult == false)
                            {
                                return;
                            }
                        }
                    chatText = chatText.TrimEnd();
                    _chatUtil.ChatMessageText = chatText;
                    OutputValues output = chatMedia.DoSendMessage(_chatUtil.SessionID, _chatUtil.ChatMessageText);
                    if (output.MessageCode == "200")
                    {
                        txtChatSend.Document.Blocks.Clear();
                        txtChatSend.Focus();
                        _chatUtil.ChatMessageText = string.Empty;
                    }
                    if (checkPushURLIsValid(txtPushURL.Text.ToString().Trim()))
                    {
                        OutputValues output1 = chatMedia.DoSendNotifyMessage(_chatUtil.SessionID, _chatUtil.ChatNoticeText);
                        if (output1.MessageCode == "200")
                        {
                            txtPushURL.Text = string.Empty;
                            _chatUtil.ChatNoticeText = string.Empty;
                        }
                    }
                    else
                    {
                        txtPushURL.Text = string.Empty;
                        _chatUtil.ChatNoticeText = string.Empty;
                    }
                }
                else if (txtPushURL.Text == string.Empty && chatText != string.Empty && chatText != "\r\n")
                {
                    if (_chatUtil.Spellcheck.MisspelledWords.Count > 0)
                        if ((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                               ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                        {
                            var showMessageBox = new MessageBox("Information",
                                              "Please check the misspelled words", "OK", "Cancel", false);
                            showMessageBox.Owner = this;
                            showMessageBox.ShowDialog();
                            if (showMessageBox.DialogResult == true || showMessageBox.DialogResult == false)
                            {
                                return;
                            }
                        }
                    chatText = chatText.TrimEnd();
                    _chatUtil.ChatMessageText = chatText;
                    OutputValues output = chatMedia.DoSendMessage(_chatUtil.SessionID, _chatUtil.ChatMessageText);
                    if (output.MessageCode == "200")
                    {
                        txtChatSend.Document.Blocks.Clear();
                        txtChatSend.Focus();
                        _chatUtil.ChatMessageText = string.Empty;
                    }
                }
                else if (txtPushURL.Text != string.Empty && chatText == string.Empty)
                {
                    _chatUtil.ChatNoticeText = txtPushURL.Text.ToString();
                    if (checkPushURLIsValid(txtPushURL.Text.ToString().Trim()))
                    {
                        OutputValues output = chatMedia.DoSendNotifyMessage(_chatUtil.SessionID, _chatUtil.ChatNoticeText);
                        if (output.MessageCode == "200")
                        {
                            txtChatSend.Document.Blocks.Clear();
                            txtChatSend.Focus();
                            _chatUtil.ChatMessageText = string.Empty;
                            txtPushURL.Text = string.Empty;
                            _chatUtil.ChatNoticeText = string.Empty;
                        }
                    }
                }
                else
                {
                    txtChatSend.Document.Blocks.Clear();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnSendMessage_Click() :" + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
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
                SizeChanged -= ChatWindow_SizeChanged;
                if (btnContacts.IsChecked == null)
                {

                }
                else if (btnContacts.IsChecked == true)
                {
                    if (ToolHeading.Text == "Show")
                        btnContacts.IsEnabled = false;
                    if (btnContacts.IsEnabled == false || btnData.IsEnabled == false || btnResponses.IsEnabled == false)
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                            columnHistory.Width = new GridLength(this.Width - (16 + columnButtons.Width.Value));
                        else if (_chatWindowState == ChatWindowState.Maximized)
                            columnHistory.Width = new GridLength(this.Width - columnButtons.Width.Value);
                    }
                    else
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                        {
                            columnChat.MinWidth = columnHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                columnChat.Width = new GridLength(400);
                                columnHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                columnChat.Width = new GridLength(mathSize);
                                columnHistory.Width = new GridLength(mathSize);
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
                            Width = columnChat.Width.Value + columnHistory.Width.Value + columnButtons.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + columnButtons.Width.Value;
                        }
                        else if (_chatWindowState == ChatWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            columnChat.Width = new GridLength(mathSize);
                            columnHistory.Width = new GridLength(mathSize);
                        }
                    }

                    grdUC.Children.Clear();
                    btnShowHideIXNPanel.IsEnabled = true;

                    if (string.IsNullOrEmpty(_chatUtil.ContactID))
                    {
                        Dictionary<ContactDetails, string> _contactdetails = null;
                        _contactdetails = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactId(ConfigContainer.Instance().TenantDbId.ToString(), MediaTypes.Chat, _chatUtil.UserData);
                        if (_contactdetails != null)
                        {
                            _chatUtil.ContactID = _contactdetails.ContainsKey(ContactDetails.ContactId) ? _contactdetails[ContactDetails.ContactId].ToString() : string.Empty;
                        }
                    }

                    UserControl contact = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactUserControl(_chatUtil.ContactID, MediaTypes.Chat, _chatUtil.IxnType != "Consult");
                    if (contact != null)
                    {
                        contact.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        Grid.SetColumn(contact, 1);
                        grdUC.Children.Add(contact);
                    }

                    btnResponses.IsEnabled = true;
                    btnData.IsEnabled = true;
                    btnResponses.IsChecked = false;
                    btnData.IsChecked = false;
                    _chatUtil.ResponseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    _chatUtil.CasedataImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    _chatUtil.ContactImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\leftArrow.png", UriKind.Relative));
                    btnData.Tag = "Show";
                    btnContacts.Tag = "Hide";
                    btnResponses.Tag = "Show";
                }
                else
                {
                    _chatUtil.ContactImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    btnContacts.Tag = "Show";
                    _windowRightGridWidth = columnHistory.Width.Value;
                    grdUC.Children.Clear();
                    columnHistory.Width = new GridLength(0);
                    columnHistory.MinWidth = 0;

                    if (_chatWindowState == ChatWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + columnButtons.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + columnButtons.Width.Value;
                        columnChat.Width = new GridLength(_windowMainGridWidth);
                        columnChat.MinWidth = 400;
                    }
                    else if (_chatWindowState == ChatWindowState.Maximized)
                    {
                        columnChat.Width = new GridLength(this.Width - (columnButtons.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(ChatWindow_SizeChanged);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnContacts_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        private bool getContacts()
        {
            bool _isSuccess = false;
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                {
                    List<string> attributes = new List<string>();
                    attributes.Add("PhoneNumber");
                    attributes.Add("EmailAddress");
                    attributes.Add("FirstName");
                    attributes.Add("LastName");
                    attributes.Add("Title");
                    IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetAllAttributes(_chatUtil.ContactID, attributes);
                    if (response != null)
                    {
                        if (response != null && response.Id == EventGetAttributes.MessageId)
                        {
                            EventGetAttributes eventGetAttributes = (EventGetAttributes)response;
                            if (eventGetAttributes != null)
                            {
                                _chatDataContext.CustomerDetails.Clear();
                                if (eventGetAttributes.Attributes != null && eventGetAttributes.Attributes.Count > 0)
                                    for (int attributesCount = 0; attributesCount < eventGetAttributes.Attributes.Count; attributesCount++)
                                    {
                                        if (eventGetAttributes.Attributes[attributesCount].AttrName == "FirstName")
                                        {
                                            _chatDataContext.CustomerDetails.Add("FirstName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                        }
                                        if (eventGetAttributes.Attributes[attributesCount].AttrName == "LastName")
                                        {
                                            _chatDataContext.CustomerDetails.Add("LastName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                        }
                                        if (eventGetAttributes.Attributes[attributesCount].AttrName == "Title")
                                        {
                                            _chatDataContext.CustomerDetails.Add("Title", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                        }
                                        if (eventGetAttributes.Attributes[attributesCount].AttrName == "EmailAddress" && eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count > 0)
                                        {
                                            for (int listCount = 0; listCount < eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count; listCount++)
                                            {
                                                if (eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Primary.AttrId == eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrId)
                                                {
                                                    _chatDataContext.CustomerDetails.Add("PrimaryEmailAddress", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString());
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
                                                    _chatDataContext.CustomerDetails.Add("PrimaryPhoneNumber", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString());
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                _chatDataContext.CustomerDetails.Add("FullName", string.Empty);
                                if (_chatDataContext.CustomerDetails.ContainsKey("FirstName"))
                                    _chatDataContext.CustomerDetails["FullName"] += _chatDataContext.CustomerDetails["FirstName"].ToString() + " ";
                                if (_chatDataContext.CustomerDetails.ContainsKey("LastName"))
                                    _chatDataContext.CustomerDetails["FullName"] += _chatDataContext.CustomerDetails["LastName"].ToString();
                                _isSuccess = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                _isSuccess = false;
            }
            return _isSuccess;
        }

        #region getContact
        /// <summary>
        /// Gets the contacts.
        /// </summary>
        private bool getContacts(string placeID, bool isConference)
        {
            bool isSuccess = false;
            try
            {
                _btnChatPersonDataContextMenu.Items.Clear();
                if (placeID == string.Empty || placeID == "")
                {
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                    {
                        List<string> attributes = new List<string>();
                        attributes.Add("PhoneNumber");
                        attributes.Add("EmailAddress");
                        attributes.Add("FirstName");
                        attributes.Add("LastName");
                        attributes.Add("Title");
                        IMessage response = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetAllAttributes(_chatUtil.ContactID, attributes);
                        if (response != null)
                        {
                            if (response != null && response.Id == EventGetAttributes.MessageId)
                            {
                                EventGetAttributes eventGetAttributes = (EventGetAttributes)response;
                                if (eventGetAttributes != null)
                                {
                                    _chatDataContext.CustomerDetails.Clear();
                                    if (eventGetAttributes.Attributes != null && eventGetAttributes.Attributes.Count > 0)
                                        for (int attributesCount = 0; attributesCount < eventGetAttributes.Attributes.Count; attributesCount++)
                                        {
                                            if (eventGetAttributes.Attributes[attributesCount].AttrName == "PhoneNumber" && eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count > 0)
                                            {
                                                for (int listCount = 0; listCount < eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count; listCount++)
                                                {
                                                    string phoneNumber = eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString();
                                                    if (phoneNumber != string.Empty)
                                                    {
                                                        var _mnuPhoneNumber = new MenuItem();
                                                        _mnuPhoneNumber.Header = "Call " + phoneNumber;
                                                        _mnuPhoneNumber.Tag = phoneNumber;
                                                        _mnuPhoneNumber.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Voice.png", UriKind.Relative)) };
                                                        _mnuPhoneNumber.Margin = new System.Windows.Thickness(2);
                                                        _mnuPhoneNumber.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                                                        BindingOperations.SetBinding(_mnuPhoneNumber, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableCallMenuitems"), Source = DataContext });
                                                        _btnChatPersonDataContextMenu.Items.Add(_mnuPhoneNumber);
                                                        _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                                                        isSuccess = true;
                                                    }
                                                }
                                            }
                                            if (eventGetAttributes.Attributes[attributesCount].AttrName == "FirstName")
                                            {
                                                _chatDataContext.CustomerDetails.Add("FirstName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                            }
                                            if (eventGetAttributes.Attributes[attributesCount].AttrName == "LastName")
                                            {
                                                _chatDataContext.CustomerDetails.Add("LastName", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                            }
                                            if (eventGetAttributes.Attributes[attributesCount].AttrName == "Title")
                                            {
                                                _chatDataContext.CustomerDetails.Add("Title", eventGetAttributes.Attributes[attributesCount].AttributesInfoList[0].AttrValue.ToString());
                                            }
                                            if (eventGetAttributes.Attributes[attributesCount].AttrName == "EmailAddress" && eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count > 0)
                                            {
                                                for (int listCount = 0; listCount < eventGetAttributes.Attributes[attributesCount].AttributesInfoList.Count; listCount++)
                                                {
                                                    string emailAddress = eventGetAttributes.Attributes[attributesCount].AttributesInfoList[listCount].AttrValue.ToString();
                                                    if (emailAddress != string.Empty)
                                                    {
                                                        var _mnuEmailAddress = new MenuItem();
                                                        _mnuEmailAddress.Header = "E-mail(" + emailAddress + ")";
                                                        _mnuEmailAddress.Tag = emailAddress;
                                                        _mnuEmailAddress.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = _chatDataContext.GetBitmapImage(new Uri("/Pointel.Interactions.Chat;component/Images/Email.png", UriKind.Relative)) };
                                                        _mnuEmailAddress.Margin = new System.Windows.Thickness(2);
                                                        _mnuEmailAddress.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                                                        if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Email))
                                                        {
                                                            _chatUtil.EnableEmailMenuitems = true;
                                                            BindingOperations.SetBinding(_mnuEmailAddress, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableEmailMenuitems"), Source = DataContext });
                                                        }
                                                        else
                                                        {
                                                            _chatUtil.EnableEmailMenuitems = false;
                                                            BindingOperations.SetBinding(_mnuEmailAddress, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableEmailMenuitems"), Source = DataContext });
                                                        }
                                                        _btnChatPersonDataContextMenu.Items.Add(_mnuEmailAddress);
                                                        _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                                                        isSuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                }
                            }
                            else
                            {
                                isSuccess = false;
                            }
                        }
                        attributes = null;
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
                else
                {
                    var _mnuPhoneNumber = new MenuItem();
                    _mnuPhoneNumber.Header = "Call ";
                    _mnuPhoneNumber.Tag = getDNFromPlace(placeID.ToString().Trim());
                    _mnuPhoneNumber.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Voice.png", UriKind.Relative)) };
                    _mnuPhoneNumber.Margin = new System.Windows.Thickness(2);
                    _mnuPhoneNumber.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                    BindingOperations.SetBinding(_mnuPhoneNumber, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableCallMenuitems"), Source = DataContext });
                    _btnChatPersonDataContextMenu.Items.Add(_mnuPhoneNumber);
                    _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                    isSuccess = true;
                }
                //ObservableCollection<IPartyInfo> temp = _chatUtil.PartiesInfo;
                //var getAliveUser = temp.Where(x => x.UserState == "Connected" && x.UserType != ChatDataContext.ChatUsertype.Client.ToString() && x.Visibility == "All").ToList();
                //if (getAliveUser.Count > 1)
                //{
                //    foreach (var item in getAliveUser)
                //    {
                //        var _mnuDeleteConference = new MenuItem();
                //        _mnuDeleteConference.Header = "Delete from Conference";
                //        _mnuDeleteConference.Margin = new System.Windows.Thickness(2);
                //        _mnuDeleteConference.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                //        BindingOperations.SetBinding(_mnuDeleteConference, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableDelConfMenuitems"), Source = DataContext });
                //        _btnChatPersonDataContextMenu.Items.Add(_mnuDeleteConference);
                //        _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                //        isSuccess = true;
                //        break;
                //    }
                //}
                if (isConference)
                {
                    var _mnuDeleteConference = new MenuItem();
                    _mnuDeleteConference.Header = "Delete from Conference";
                    _mnuDeleteConference.Margin = new System.Windows.Thickness(2);
                    _mnuDeleteConference.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                    BindingOperations.SetBinding(_mnuDeleteConference, MenuItem.IsEnabledProperty, new Binding() { Path = new PropertyPath("EnableDelConfMenuitems"), Source = DataContext });
                    _btnChatPersonDataContextMenu.Items.Add(_mnuDeleteConference);
                    _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                    isSuccess = true;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("getContacts() :" + generalException.ToString());
                isSuccess = false;
            }
            return isSuccess;
        }

        public string getDNFromPlace(string placeName)
        {
            try
            {
                string dn = string.Empty;

                CfgPlace application = new CfgPlace(ChatDataContext.ComObject);
                CfgPlaceQuery queryApp = new CfgPlaceQuery();
                queryApp.TenantDbid = ConfigContainer.Instance().TenantDbId;
                queryApp.Name = placeName;
                application = ChatDataContext.ComObject.RetrieveObject<CfgPlace>(queryApp);

                IList<CfgDN> DNCollection = (IList<CfgDN>)application.DNs;

                if (DNCollection != null && DNCollection.Count > 0)
                {
                    foreach (CfgDN DN in DNCollection)
                    {
                        if (!String.IsNullOrEmpty(DN.Number))
                        {
                            if (DN.Type == CfgDNType.CFGExtension)
                            {
                                dn = DN.Number;
                                return dn;
                            }
                        }
                    }
                }
                else
                {
                    return dn;
                }
                return dn;
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while getDNFromPlace(): " + placeName + "  =  " + commonException.ToString());
                return string.Empty;
            }
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the btnResponses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnResponses_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SizeChanged -= ChatWindow_SizeChanged;
                if (btnResponses.IsChecked == null)
                {

                }
                else if (btnResponses.IsChecked == true)
                {
                    if (ToolHeading.Text == "Show")
                        btnResponses.IsEnabled = false;
                    if (btnResponses.IsEnabled == false || btnData.IsEnabled == false || btnContacts.IsEnabled == false)
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                            columnHistory.Width = new GridLength(this.Width - (16 + columnButtons.Width.Value));
                        else if (_chatWindowState == ChatWindowState.Maximized)
                            columnHistory.Width = new GridLength(this.Width - columnButtons.Width.Value);
                    }
                    else
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                        {
                            columnChat.MinWidth = columnHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                columnChat.Width = new GridLength(400);
                                columnHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                columnChat.Width = new GridLength(mathSize);
                                columnHistory.Width = new GridLength(mathSize);
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
                            Width = columnChat.Width.Value + columnHistory.Width.Value + columnButtons.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + columnButtons.Width.Value;
                        }
                        else if (_chatWindowState == ChatWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            columnChat.Width = new GridLength(mathSize);
                            columnHistory.Width = new GridLength(mathSize);
                        }
                    }

                    grdUC.Children.Clear();

                    btnShowHideIXNPanel.IsEnabled = true;

                    System.Windows.Controls.UserControl contactDirectory = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetResponseUserControl(false, EventNotifyResponseComposeClick);
                    contactDirectory.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    contactDirectory.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    Grid.SetColumn(contactDirectory, 1);
                    grdUC.Children.Add(contactDirectory);

                    btnContacts.IsEnabled = true;
                    btnData.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnData.IsChecked = false;
                    _chatUtil.ResponseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\leftArrow.png", UriKind.Relative));
                    _chatUtil.CasedataImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    _chatUtil.ContactImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    btnData.Tag = "Show";
                    btnContacts.Tag = "Show";
                    btnResponses.Tag = "Hide";
                }
                else
                {
                    _chatUtil.ResponseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    btnResponses.Tag = "Show";
                    _windowRightGridWidth = columnHistory.Width.Value;
                    grdUC.Children.Clear();
                    columnHistory.Width = new GridLength(0);
                    columnHistory.MinWidth = 0;

                    if (_chatWindowState == ChatWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + columnButtons.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + columnButtons.Width.Value;
                        columnChat.Width = new GridLength(_windowMainGridWidth);
                        columnChat.MinWidth = 400;
                    }
                    else if (_chatWindowState == ChatWindowState.Maximized)
                    {
                        columnChat.Width = new GridLength(this.Width - (columnButtons.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(ChatWindow_SizeChanged);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnResponses_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        /// <summary>
        /// Events the notify response compose click.
        /// </summary>
        /// <param name="plaintext">The plain text.</param>
        /// <param name="response">The response.</param>
        /// <param name="responseSubject">The response subject.</param>
        /// <param name="name">The name.</param>
        /// <param name="selectedAttachments">The selected attachments.</param>
        /// <returns></returns>
        private string EventNotifyResponseComposeClick(string plaintext, string response, string responseSubject, string name, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttachmentList selectedAttachments)
        {
            try
            {
                if (_chatUtil.IsEnableRelease)
                {
                    if (getContacts())
                    {
                        string data = GetResponseFieldCodeData(plaintext);
                        if (!string.IsNullOrEmpty(data))
                            txtChatSend.Document.Blocks.Add(new Paragraph(new Run(data)));
                        else
                            txtChatSend.Document.Blocks.Add(new Paragraph(new Run(plaintext)));

                        txtChatSend.Focus();
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error("EventNotifyResponseComposeClick() " + exception.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the response field code data.
        /// </summary>
        /// <param name="renderText">The render text.</param>
        /// <returns></returns>
        private string GetResponseFieldCodeData(string renderText)
        {
            string renderedText = string.Empty;
            ContactProperties contactProperties = null;
            AgentProperties agentProperties = null;
            ContactInteractionProperties interactionProperties = null;
            try
            {
                CfgPerson Person = ConfigContainer.Instance().GetValue("CfgPerson");
                if (Person != null)
                {
                    agentProperties = new AgentProperties();
                    agentProperties.FirstName = Person.FirstName;
                    agentProperties.LastName = Person.LastName;
                    agentProperties.FullName = Person.FirstName + " " + Person.LastName;
                    agentProperties.Signature = "NO SIGNATURE";
                }
                if (_chatDataContext.CustomerDetails.Count > 0)
                {
                    contactProperties = new ContactProperties();
                    contactProperties.Id = _chatUtil.ContactID;
                    if (_chatDataContext.CustomerDetails.ContainsKey("FirstName"))
                        contactProperties.FirstName = _chatDataContext.CustomerDetails["FirstName"];
                    if (_chatDataContext.CustomerDetails.ContainsKey("LastName"))
                        contactProperties.LastName = _chatDataContext.CustomerDetails["LastName"];
                    if (_chatDataContext.CustomerDetails.ContainsKey("FullName"))
                        contactProperties.FullName = _chatDataContext.CustomerDetails["FullName"];
                    if (_chatDataContext.CustomerDetails.ContainsKey("Title"))
                        contactProperties.Title = _chatDataContext.CustomerDetails["Title"];
                    if (_chatDataContext.CustomerDetails.ContainsKey("PrimaryEmailAddress"))
                        contactProperties.PrimaryEmailAddress = _chatDataContext.CustomerDetails["PrimaryEmailAddress"];
                    if (_chatDataContext.CustomerDetails.ContainsKey("PrimaryPhoneNumber"))
                        contactProperties.PrimaryPhoneNumber = _chatDataContext.CustomerDetails["PrimaryPhoneNumber"];
                }
                if (_chatUtil.UserData != null && _chatUtil.UserData.Count > 0)
                {
                    interactionProperties = new ContactInteractionProperties();
                    interactionProperties.Id = _chatUtil.InteractionID;
                    if (_chatUtil.UserData != null && _chatUtil.UserData.ContainsKey("Subject"))
                        interactionProperties.Subject = _chatUtil.UserData["Subject"].ToString();

                    if (_chatUtil.TimeShift > 0)
                    {
                        try
                        {
                            var timespan = TimeSpan.FromMinutes(Convert.ToDouble(_chatUtil.TimeShift));
                            interactionProperties.TimeZone = "GMT" + (timespan.Hours >= 0 ? ("+" + timespan.Hours.ToString()) : timespan.Hours.ToString()) + ":" + Math.Abs(timespan.Minutes);
                            _logger.Info("Timezone updated from server");
                        }
                        catch
                        {
                            interactionProperties.TimeZone = "GMT" + DateTime.Now.ToLocalTime().ToString("zzz");
                            _logger.Info("Current System timezone updated");
                        }
                    }
                    else
                    {
                        interactionProperties.TimeZone = "GMT" + DateTime.Now.ToLocalTime().ToString("zzz");
                        _logger.Info("Current System timezone updated");
                    }

                    interactionProperties.AttachedData = new PropertyList();
                    for (int i = 0; i < _chatUtil.UserData.Count; i++)
                    {
                        interactionProperties.AttachedData.Add(new Property() { Name = _chatUtil.UserData.AllKeys[i].ToString(), Value = _chatUtil.UserData.AllValues[i].ToString() });
                    }
                    if (!string.IsNullOrEmpty(_chatUtil.StartDate))
                        interactionProperties.StartDate = _chatUtil.StartDate;

                    interactionProperties.OtherProperties = new PropertyList();

                    // Date Created Attribute
                    string timezone = interactionProperties.TimeZone.Replace("GMT", "");
                    string[] time = timezone.Split(':');
                    NullableDateTime datecreated = interactionProperties.StartDate.Value.AddMinutes(Convert.ToInt64(time[0]) > 0 ?
                        ((Convert.ToInt64(time[0]) * 60) + Convert.ToInt64(time[1])) : ((Convert.ToInt64(time[0]) * 60) - Convert.ToInt64(time[1])));
                    interactionProperties.OtherProperties.Add(new Property() { Name = "DateCreated", Value = datecreated.ToString() });
                }
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                    renderedText = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetResponseFieldContents(agentProperties, contactProperties, interactionProperties, renderText);
                else
                    renderedText = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error("GetReponseFieldCodeData : " + (ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
                renderedText = string.Empty;
            }
            finally
            {
                contactProperties = null;
                agentProperties = null;
                interactionProperties = null;
            }
            return renderedText;
        }

        /// <summary>
        /// Determines whether the specified word is hyperlink.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public bool IsHyperlink(string word)
        {
            try
            {
                if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
                {
                    if (UrlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                    else if (urlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while IsHyperlink() :" + generalException.ToString());
            }
            return false;
        }

        /// <summary>
        /// Handles the Click event of the btnCheckURL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCheckURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri tempUri = null;
                string urlString = string.Empty;
                urlString = txtPushURL.Text.ToString().Trim();
                if (urlString != string.Empty || urlString != "")
                {
                    if (IsHyperlink(urlString))
                    {
                        tempUri = new Uri(urlString, UriKind.RelativeOrAbsolute);

                        if (!tempUri.IsAbsoluteUri)
                        {
                            tempUri = new Uri(@"http://" + urlString, UriKind.Absolute);
                        }
                        if (tempUri != null && tempUri.ToString() != string.Empty)
                        {
                            Process.Start(tempUri.ToString());
                        }
                    }
                    else
                    {
                        starttimerforerror("It is not possible to check the URL '" + txtPushURL.Text.ToString() + "'");
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnCheckURL_Click() :" + generalException.ToString());
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
                SizeChanged -= ChatWindow_SizeChanged;
                if (ToolHeading.Text == "Hide")
                {
                    _windowMainGridWidth = columnChat.ActualWidth;
                    imgShowHideIXNPanel.Source = _chatDataContext.GetBitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Show_Left.png", UriKind.Relative));

                    columnChat.Width = new GridLength(0);
                    columnChat.MinWidth = 0;
                    if (_chatWindowState == ChatWindowState.Normal)
                    {
                        columnHistory.Width = new GridLength(this.Width - (16 + columnButtons.Width.Value));
                        columnHistory.MinWidth = 400;
                        this.MinWidth = 400 + 16 + columnButtons.Width.Value;
                    }
                    else if (_chatWindowState == ChatWindowState.Maximized)
                    {
                        columnHistory.Width = new GridLength(this.Width - columnButtons.Width.Value);
                        columnHistory.MinWidth = 400;
                        this.MinWidth = 400 + columnButtons.Width.Value;
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
                    imgShowHideIXNPanel.Source = _chatDataContext.GetBitmapImage(new Uri((ConfigContainer.Instance().AllKeys.Contains("image-path") ? (string)ConfigContainer.Instance().GetValue("image-path") : string.Empty) + "\\Hide_Left.png", UriKind.Relative));
                    if (_chatWindowState == ChatWindowState.Normal)
                    {
                        double size = (this.Width - (16 + columnButtons.Width.Value)) / 2;
                        columnChat.Width = new GridLength(size);
                        columnHistory.Width = new GridLength(size);
                        columnChat.MinWidth = columnHistory.MinWidth = 400;
                        this.MinWidth = 400 + 400 + 16 + columnButtons.Width.Value;
                    }
                    else if (_chatWindowState == ChatWindowState.Maximized)
                    {
                        var _Width = this.Width - columnButtons.Width.Value;
                        columnChat.Width = new GridLength(_Width / 2);
                        columnHistory.Width = new GridLength(_Width / 2);
                    }
                    btnData.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsEnabled = true;
                }
                SizeChanged += new SizeChangedEventHandler(ChatWindow_SizeChanged);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnShowHideIXNPanel_Click() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAvailableURL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAvailableURL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseError(null, null);
                if (_availablePushURL.Items.Count > 0)
                {
                    _availablePushURL.PlacementTarget = this.btnAvailableURL;
                    _availablePushURL.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
                    _availablePushURL.IsOpen = true;
                    _availablePushURL.StaysOpen = true;
                    _availablePushURL.Focus();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnAvailableURL_Click() :" + generalException.ToString());
            }
        }
        /// <summary>
        /// Handles the KeyDown event of the txtChatSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtChatSend_KeyDown(object sender, KeyEventArgs e)
        {
            CloseError(null, null);
            bool isURLSend = false;
            bool isTextSend = false;
            try
            {
                switch (e.Key)
                {
                    case Key.Back:
                    case Key.Delete:
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Home:
                    case Key.Tab:
                    case Key.LeftAlt:
                    case Key.RightAlt:
                    case Key.Prior:
                    case Key.Next:
                    case Key.Escape:
                    case Key.System:
                    case Key.LeftShift:
                    case Key.RightShift:
                    case Key.RightCtrl:
                    case Key.LeftCtrl:
                    case Key.CapsLock:
                    case Key.NumLock:
                        break;
                    case Key.Enter:
                        if (txtPushURL.Text != string.Empty && txtPushURL.Text.Length > 0 && txtPushURL.Text != "\r\n")
                        {
                            isURLSend = true;
                        }
                        else
                        {
                            isURLSend = false;
                            txtPushURL.Text = string.Empty;
                            txtPushURL.Focus();
                        }
                        var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                        string Richtextvalue = textRange.Text;
                        if (Richtextvalue != string.Empty && Richtextvalue != "\r\n")
                        {
                            isTextSend = true;
                        }
                        else
                        {
                            isTextSend = false;
                            txtChatSend.Document.Blocks.Clear();
                            txtChatSend.Focus();
                            txtChatSend.CaretPosition = txtChatSend.Document.ContentStart;
                        }
                        if (isURLSend || isTextSend)
                        {
                            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                            {
                                var rtxtBox = e.Source as RichTextBox;
                                if (rtxtBox != null)
                                {
                                    rtxtBox.AcceptsReturn = true;
                                    rtxtBox.AppendText(Environment.NewLine);
                                    rtxtBox.Focus();
                                    TextPointer moveTo = rtxtBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                                    if (moveTo != null)
                                        rtxtBox.CaretPosition = moveTo;
                                    e.Handled = true;
                                    return;
                                }
                                var txtBox = e.Source as TextBox;
                                if (txtBox != null)
                                {
                                    txtBox.AcceptsReturn = true;
                                    txtBox.Focus();
                                    e.Handled = true;
                                    return;
                                }
                            }
                            else
                            {
                                e.Handled = true;
                                btnSendMessage.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            }
                        }
                        break;
                    default:
                        if ((((string)ConfigContainer.Instance().GetValue("chat.enable.typing")).ToLower().Equals("true")) && _isChatTyping)
                        {
                            ChatMedia chatMedia = new ChatMedia();
                            OutputValues output = chatMedia.DoSendTypeStartNotification(_chatUtil.SessionID, "user is typing.");
                            if (output.MessageCode == "200")
                            {
                                _isChatTyping = false;
                            }
                            chatMedia = null;
                        }
                        break;
                }

            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_KeyDown() :" + generalException.ToString());
            }
        }

        private string removeSpecialChars(string word)
        {
            for (int index = 0; index < chars.Length; index++)
            {
                if (word.Contains(chars[index]))
                    word = word.Replace(chars[index], " ");
            }
            word = Regex.Replace(word, @"\s+", " ");
            return word;
        }

        //private void updateRTBText(FlowDocument document)
        //{
        //    Dispatcher.CurrentDispatcher.BeginInvoke((Action)(delegate
        //    {
        //        string pattern = @"[^\W\d](\w|[-']{1,2}(?=\w))*";
        //        TextPointer pointer = document.ContentStart;
        //        var textRun = new TextRange(document.ContentStart, document.ContentEnd).Text;
        //        MatchCollection matches = Regex.Matches(textRun, pattern);
        //        foreach (Match match in matches)
        //        {
        //            while (pointer != null)
        //            {
        //                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        //                {
        //                    string textRun1 = pointer.GetTextInRun(LogicalDirection.Forward);

        //                    // Find the starting index of any substring that matches "word".
        //                    int indexInRun = textRun1.IndexOf(match.Value);
        //                    if (indexInRun >= 0)
        //                    {
        //                        TextPointer start = pointer.GetPositionAtOffset(indexInRun);
        //                        TextPointer end = start.GetPositionAtOffset(match.Value.Length);
        //                        var wordrange = (new TextRange(start, end));
        //                        //var fontfamily = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontFamilyProperty);
        //                        //var fontstyle = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontStyleProperty);
        //                        //var fontweight = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontWeightProperty);
        //                        //var fontstr = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontStretchProperty);
        //                        //var fontsize = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontSizeProperty);
        //                        //var fontfore = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty);
        //                        //var dfrtg = new FormattedText(wordrange.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface((FontFamily)fontfamily, (FontStyle)fontstyle, (FontWeight)fontweight, (FontStretch)fontstr), (double)fontsize, (Brush)fontfore);

        //                        if (_chatUtil.Spellcheck.MisspelledWords.Contains(match.Value) && _chatUtil.IsEnableSpellCheck)
        //                        {
        //                            //wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, CustomTextDecorationCollection());
        //                            //Canvas canvas = new Canvas();
        //                            //Rectangle dfr = new Rectangle() { Fill = wavyBrush, Height = 5, Width = dfrtg.Width };
        //                            //canvas.Children.Add(dfr);
        //                            //new InlineUIContainer(canvas, start);
        //                            //while the text wrapping ui will collapse
        //                        }
        //                        else
        //                        {
        //                            wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
        //                            //var sder = pointer.GetAdjacentElement(pointer.LogicalDirection);                                    
        //                        }
        //                        break;
        //                    }
        //                }
        //                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
        //            }
        //        }
        //    }), DispatcherPriority.DataBind);
        //}


        private void CustomTextDecorationCollection()
        {
            _txtDecorationCollection = new TextDecorationCollection();
            TextDecoration myUnderline = new TextDecoration();
            myUnderline.Location = TextDecorationLocation.Baseline;
            // Set the solid color brush.Brushes.Red
            VisualBrush _wavyBrush = (VisualBrush)this.FindResource("WavyBrush");
            myUnderline.Pen = new Pen(_wavyBrush, 10);
            myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            // Set the underline decoration to the text block.
            _txtDecorationCollection.Add(myUnderline);
        }

        /// <summary>
        /// Handles the TextChanged event of the txtPushURL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtPushURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtChatSend.Document != null)
                {
                    var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                    if (textRange.Text != string.Empty && textRange.Text.Length > 0 && textRange.Text != "\r\n\r\n" && textRange.Text != "\r\n")
                    {
                        _chatUtil.IsButtonSendEnabled = true;
                        if (_isPasteText)
                        {
                            _isPasteText = false;
                            if (_chatUtil.IsEnableSpellCheck)
                                StartSpellCheck();
                        }
                        return;
                    }
                    else
                    {
                        _chatUtil.IsButtonSendEnabled = false;
                        txtChatSend.Document.Blocks.Clear();
                    }
                }
                if (txtPushURL.Text != string.Empty && txtPushURL.Text.Length > 0 && txtPushURL.Text != "\r\n")
                {
                    _chatUtil.IsButtonSendEnabled = true;
                }
                else
                {
                    _chatUtil.IsButtonSendEnabled = false;
                    txtPushURL.Text = string.Empty;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtPushURL_TextChanged() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the txtPushURL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void txtPushURL_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPushURL.Text != string.Empty && txtPushURL.Text.Length > 0 && txtPushURL.Text != "\r\n")
                {
                    _chatUtil.IsButtonSendEnabled = true;
                    return;
                }
                else
                {
                    _chatUtil.IsButtonSendEnabled = false;
                    txtPushURL.Text = string.Empty;
                }
                if (txtChatSend.Document != null)
                {
                    var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                    if (textRange.Text != string.Empty && textRange.Text.Length > 0 && textRange.Text != "\r\n\r\n" && textRange.Text != "\r\n")
                    {
                        _chatUtil.IsButtonSendEnabled = true;
                        return;
                    }
                    else
                    {
                        _chatUtil.IsButtonSendEnabled = false;
                        txtChatSend.Document.Blocks.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtPushURL_GotFocus() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the mainRTB control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void mainRTB_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mainRTB.Focus();
                if (mainRTB.Document != null)
                {
                    _rtbContextMenu.PlacementTarget = this.mainRTB;
                    _rtbContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                    _rtbContextMenu.IsOpen = true;
                    _rtbContextMenu.StaysOpen = true;
                    _rtbContextMenu.Focus();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while mainRTB_PreviewMouseRightButtonDown() :" + generalException.ToString());
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
                SizeChanged -= ChatWindow_SizeChanged;
                if (btnData.IsChecked == null)
                {

                }
                else if (btnData.IsChecked == true)
                {
                    if (ToolHeading.Text == "Show")
                        btnData.IsEnabled = false;
                    if (btnData.IsEnabled == false || btnContacts.IsEnabled == false || btnResponses.IsEnabled == false)
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                            columnHistory.Width = new GridLength(this.Width - (16 + columnButtons.Width.Value));
                        else if (_chatWindowState == ChatWindowState.Maximized)
                            columnHistory.Width = new GridLength(this.Width - columnButtons.Width.Value);
                    }
                    else
                    {
                        if (_chatWindowState == ChatWindowState.Normal)
                        {
                            columnChat.MinWidth = columnHistory.MinWidth = 400;
                            if (this.Width < (400 + 400 + 16 + 19))
                            {
                                columnChat.Width = new GridLength(400);
                                columnHistory.Width = new GridLength(400);
                            }
                            else
                            {
                                var mathSize = (this.Width - (19 + 16)) / 2;

                                columnChat.Width = new GridLength(mathSize);
                                columnHistory.Width = new GridLength(mathSize);
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
                            Width = columnChat.Width.Value + columnHistory.Width.Value + columnButtons.Width.Value + 16;
                            MinWidth = 400 + 400 + 16 + columnButtons.Width.Value;
                        }
                        else if (_chatWindowState == ChatWindowState.Maximized)
                        {
                            var mathSize = (this.Width - 20) / 2;
                            columnChat.Width = new GridLength(mathSize);
                            columnHistory.Width = new GridLength(mathSize);
                        }
                    }


                    grdUC.Children.Clear();
                    btnShowHideIXNPanel.IsEnabled = true;
                    var interactionData = new Pointel.Interactions.Chat.UserControls.InteractionData(_chatUtil, InteractionID);
                    interactionData.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    interactionData.MinWidth = 395;
                    grdUC.Children.Add(interactionData);

                    btnContacts.IsEnabled = true;
                    btnResponses.IsEnabled = true;
                    btnContacts.IsChecked = false;
                    btnResponses.IsChecked = false;

                    _chatUtil.ResponseImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    _chatUtil.ContactImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    _chatUtil.CasedataImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\leftArrow.png", UriKind.Relative));
                    btnData.Tag = "Hide";
                    btnContacts.Tag = "Show";
                    btnResponses.Tag = "Show";
                }
                else
                {
                    _chatUtil.CasedataImageSource = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\rightArrow.png", UriKind.Relative));
                    btnData.Tag = "Show";
                    _windowRightGridWidth = columnHistory.Width.Value;

                    grdUC.Children.Clear();
                    columnHistory.Width = new GridLength(0);
                    columnHistory.MinWidth = 0;

                    if (_chatWindowState == ChatWindowState.Normal)
                    {
                        this.MinWidth = 400 + 16 + columnButtons.Width.Value;
                        this.Width = _windowMainGridWidth + 16 + columnButtons.Width.Value;
                        columnChat.Width = new GridLength(_windowMainGridWidth);
                        columnChat.MinWidth = 400;
                    }
                    else if (_chatWindowState == ChatWindowState.Maximized)
                    {
                        columnChat.Width = new GridLength(this.Width - (columnButtons.Width.Value + 5));
                    }
                    btnShowHideIXNPanel.IsEnabled = false;
                }
                SizeChanged += new SizeChangedEventHandler(ChatWindow_SizeChanged);
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnData_Click() :" + generalException.ToString());
            }
            finally { GC.SuppressFinalize(this); GC.Collect(); }
        }

        /// <summary>
        /// Handles the SizeChanged event of the rtbViewChatWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        //private void rtbViewChatWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    try
        //    {
        //        mainRTB.Width = (rtbViewChatWindow.ActualWidth - 4);
        //    }
        //    catch (Exception generalException)
        //    {
        //        _logger.Error(" Error occurred while rtbViewChatWindow_SizeChanged() :" + generalException.ToString());
        //    }
        //}

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the txtPushURL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtPushURL_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                txtPushURL.Focus();
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtPushURL_MouseLeftButtonDown() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the txtChatSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtChatSend_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                txtChatSend.Focus();
                var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.CaretPosition);
                int catatPos = textRange.Text.Length;
                if (catatPos >= 0)
                {
                    if (txtChatSend.Selection.Text == string.Empty)
                    {
                        var mousePosition = Mouse.GetPosition(txtChatSend);
                        var textPointer = txtChatSend.GetPositionFromPoint(mousePosition, false);
                        if (textPointer != null)
                        {
                            string word = GetWordAtPointer(textPointer);
                            if (!_chatUtil.IsEnableSpellCheck) return;
                            if (_chatUtil.Spellcheck.MisspelledWords.Contains(word))
                            {
                                _txtSuggestionContextMenu.Items.Clear();
                                //var start = txtChatSend.CaretPosition;
                                //   var t = start.GetTextInRun(LogicalDirection.Backward);
                                // var end = start.GetNextContextPosition(LogicalDirection.Forward);
                                //var t1 = end.GetTextInRun(LogicalDirection.Backward);
                                // txtChatSend.Selection.Select(start, end);
                                _chatUtil.Spellcheck.SelectedMisspelledWord = word;
                                _chatUtil.Spellcheck.LoadSuggestions(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                foreach (var item in _chatUtil.Spellcheck.SuggestedWords)
                                {
                                    var menuSuggestion = new MenuItem();
                                    menuSuggestion.Margin = new System.Windows.Thickness(2);
                                    menuSuggestion.Header = item;
                                    menuSuggestion.Tag = textPointer;
                                    menuSuggestion.Click += new RoutedEventHandler(menuSuggestion_Click);
                                    _txtSuggestionContextMenu.Items.Add(menuSuggestion);
                                }
                                _txtSuggestionContextMenu.Items.Add(new Separator());
                                MenuItem _mnuItemIgnore = new MenuItem();
                                _mnuItemIgnore.Header = "Ignore All";
                                _mnuItemIgnore.Tag = textPointer;
                                _mnuItemIgnore.Click += new RoutedEventHandler(menuSuggestion_Click);
                                _txtSuggestionContextMenu.Items.Add(_mnuItemIgnore);

                                _txtSuggestionContextMenu.Items.Add(new Separator());
                                MenuItem _mnuAddToDic = new MenuItem();
                                _mnuAddToDic.Header = "Add To Dictionary";
                                _mnuAddToDic.Tag = textPointer;
                                _mnuAddToDic.Click += new RoutedEventHandler(menuSuggestion_Click);
                                _txtSuggestionContextMenu.Items.Add(_mnuAddToDic);

                                _txtSuggestionContextMenu.Style = (Style)FindResource("Contextmenu");
                                _txtSuggestionContextMenu.PlacementTarget = this.txtChatSend;
                                _txtSuggestionContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                                _txtSuggestionContextMenu.IsOpen = true;
                                _txtSuggestionContextMenu.StaysOpen = true;
                                _txtSuggestionContextMenu.Focus();
                            }
                            else
                            {
                                _txtMessageContextMenu.PlacementTarget = this.txtChatSend;
                                _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                                _txtMessageContextMenu.IsOpen = true;
                                _txtMessageContextMenu.StaysOpen = true;
                                _txtMessageContextMenu.Focus();
                            }
                        }
                        else
                        {
                            _txtMessageContextMenu.PlacementTarget = this.txtChatSend;
                            _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                            _txtMessageContextMenu.IsOpen = true;
                            _txtMessageContextMenu.StaysOpen = true;
                            _txtMessageContextMenu.Focus();
                        }
                    }
                    else
                    {
                        _txtMessageContextMenu.PlacementTarget = this.txtChatSend;
                        _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        _txtMessageContextMenu.IsOpen = true;
                        _txtMessageContextMenu.StaysOpen = true;
                        _txtMessageContextMenu.Focus();
                    }

                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_PreviewMouseRightButtonDown() :" + generalException.ToString());
            }
        }

        private string GetWordAtPointer(TextPointer textPointer)
        {
            return string.Join(string.Empty, GetWordCharactersBefore(textPointer), GetWordCharactersAfter(textPointer));
        }

        private void ReplaceWordAtPointer(TextPointer textPointer, string replacementWord)
        {
            textPointer.DeleteTextInRun(-GetWordCharactersBefore(textPointer).Count());
            textPointer.DeleteTextInRun(GetWordCharactersAfter(textPointer).Count());
            textPointer.InsertTextInRun(replacementWord);
            txtChatSend.CaretPosition = textPointer.GetNextContextPosition(LogicalDirection.Forward);
            UpdateRTB_BackgroundWorker();
        }

        private string GetWordCharactersBefore(TextPointer textPointer)
        {
            var backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).Reverse().ToArray());
            return wordCharactersBeforePointer;
        }

        private string GetWordCharactersAfter(TextPointer textPointer)
        {
            var fowards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(fowards.TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).ToArray());
            return wordCharactersAfterPointer;
        }


        /// <summary>
        /// Handles the Click event of the menuSuggestion control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void menuSuggestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Add To Dictionary")
                        {
                            if (_chatUtil.Spellcheck != null)
                            {
                                // Add the word in dictionary
                                AddToDictionary(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                _chatUtil.Spellcheck.AddTodictionary(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                // Refresh the spell check
                                StartSpellCheck();
                            }
                        }
                        else if (menuitem.Header.ToString() == "Ignore All")
                        {
                            //Remove the ignored word in mispell collection
                            _chatUtil.Spellcheck.IgnoredWords.Add(_chatUtil.Spellcheck.SelectedMisspelledWord);
                            for (int i = 0; i < _chatUtil.Spellcheck.MisspelledWords.Count; i++)
                            {
                                if (_chatUtil.Spellcheck.SelectedMisspelledWord.ToLower() == _chatUtil.Spellcheck.MisspelledWords[i].ToLower())
                                {
                                    _chatUtil.Spellcheck.MisspelledWords.Remove(_chatUtil.Spellcheck.MisspelledWords[i]);
                                    i--;
                                }
                            }

                            // Update the spell check to remove style for ignored words
                            UpdateRTB_BackgroundWorker();
                        }
                        else
                        {
                            if (_chatUtil.Spellcheck != null)
                            {
                                _chatUtil.Spellcheck.SelectedSuggestedWord = menuitem.Header.ToString();
                                if (_chatUtil.Spellcheck.SelectedSuggestedWord != "(no suggestions)")
                                {
                                    _chatUtil.Spellcheck.MisspelledWords.Remove(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                    ReplaceWordAtPointer(menuitem.Tag as TextPointer, _chatUtil.Spellcheck.SelectedSuggestedWord);
                                    _chatUtil.Spellcheck.SuggestedWords.Clear();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while menuSuggestion_Click() :" + generalException.ToString());
            }
        }

        private void AddToDictionary(string entry)
        {
            using (StreamWriter streamWriter = new StreamWriter(CustomDictionary, true))
            {
                streamWriter.WriteLine(entry);
            }
            //IgnoreAll(entry);
        }

        //private void IgnoreAll(string word)
        //{
        //    _chatUtil.Spellcheck.MisspelledWords.Remove(word);
        //    ignoreWords.Add(word);
        //    var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
        //    var txt = textRange.Text;
        //    txt = txt.Replace("\r\n", " ");
        //    txt = removeSpecialChars(txt);
        //    _chatUtil.Spellcheck.SpellCheck(txt);
        //    if (!string.IsNullOrEmpty(textRange.Text) && _chatUtil.IsEnableSpellCheck)
        //        updateRTBText(txtChatSend.Document);
        //}


        private void StartSpellCheck()
        {
            // Find mispellwords from content and add in collection
            // Start
            var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
            var txt = textRange.Text;
            txt = txt.Replace("\r\n", " ");
            txt = txt.Replace(@"\", "");
            txt = removeSpecialChars(txt);
            _chatUtil.Spellcheck.SpellCheck(txt);
            // End

            UpdateRTB_BackgroundWorker();
        }

        private void StopSpellCheck()
        {
            _chatUtil.Spellcheck.MisspelledWords.Clear();
            UpdateRTB_BackgroundWorker();
        }

        Thread updateRTB;
        private void UpdateRTB_BackgroundWorker()
        {
            if (updateRTB != null && updateRTB.IsAlive)
                updateRTB.Abort();
            updateRTB = new Thread(UpdateSpellcheck);
            updateRTB.IsBackground = true;
            updateRTB.Priority = ThreadPriority.BelowNormal;
            updateRTB.Start();
        }

        private bool checkfullword(string mispellWord, string text)
        {
            text = text.Replace("\r\n", " ");
            text = removeSpecialChars(text);
            var tempWords = text.Split(' ');
            foreach (var tempWord in tempWords)
            {
                if (mispellWord.Trim().ToLower() == tempWord.Trim().ToLower())
                    return true;
            }
            return false;
        }

        private void UpdateSpellcheck()
        {

            // Add mispellwords in Regex example: thi|anf|
            // Start
            string mispelledWords = string.Empty;
            if (_chatUtil.Spellcheck.MisspelledWords.Count > 0)
            {
                for (int i = 0; i < _chatUtil.Spellcheck.MisspelledWords.Count; i++)
                {
                    if (!string.IsNullOrEmpty(mispelledWords))
                        mispelledWords = mispelledWords + "|" + _chatUtil.Spellcheck.MisspelledWords[i].ToString();
                    else
                        mispelledWords = _chatUtil.Spellcheck.MisspelledWords[i].ToString();
                }
            }
            Regex regMispell = new Regex(mispelledWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // End

            // Loop for the richtextbox content one by one 
            var start = txtChatSend.Document.ContentStart;
            while (start != null && start.CompareTo(txtChatSend.Document.ContentEnd) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    // Get the word
                    string text = start.GetTextInRun(LogicalDirection.Forward);

                    //if (_spellChecker.MisspelledWords.Count > 0)
                    //{
                    Match matchedMispellWords = regMispell.Match(start.GetTextInRun(LogicalDirection.Forward));
                    if (matchedMispellWords.Length > 0 && checkfullword(matchedMispellWords.Value, text))
                    {
                        txtChatSend.Dispatcher.Invoke((Action)(delegate
                               {
                                   try
                                   {
                                       var textrange = new TextRange(start.GetPositionAtOffset(matchedMispellWords.Index, LogicalDirection.Forward), start.GetPositionAtOffset(matchedMispellWords.Index + matchedMispellWords.Length, LogicalDirection.Backward));
                                       textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, _txtDecorationCollection);
                                       start = textrange.End;
                                   }
                                   catch (Exception ex)
                                   {
                                       _logger.Error("update spell check" + ex.Message);
                                   }

                               }));
                        Thread.Sleep(80);
                    }
                    else
                    {
                        if (text.Trim().Length > 0 && !_chatUtil.Spellcheck.MisspelledWords.Contains(text))
                        {
                            // Add correct words in Regex example: the|any|
                            // Start

                            string correctWords = string.Empty;
                            text = text.Replace("\r\n", " ");
                            text = removeSpecialChars(text);
                            var tempWords = text.Split(' ');
                            foreach (var tempWord in tempWords)
                            {
                                if (!string.IsNullOrEmpty(correctWords))
                                    correctWords = correctWords + "|" + tempWord;
                                else
                                    correctWords = tempWord;
                            }
                            Regex regCorrectWord = new Regex(correctWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            //End
                            txtChatSend.Dispatcher.Invoke((Action)(delegate
                                      {
                                          try
                                          {
                                              Match matchedCorrectWords = regCorrectWord.Match(start.GetTextInRun(LogicalDirection.Forward));
                                              var textrange = new TextRange(start.GetPositionAtOffset(matchedCorrectWords.Index, LogicalDirection.Forward), start.GetPositionAtOffset(matchedCorrectWords.Index + matchedCorrectWords.Length, LogicalDirection.Backward));
                                              textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                                              start = textrange.End;
                                          }
                                          catch (Exception ex)
                                          {
                                              _logger.Error("update spell check" + ex.Message);
                                          }
                                      }));
                            Thread.Sleep(80);
                        }
                    }
                    // Replace the styled mispelled word to empty example : text: org the -->replace "org" to string.empty

                    //}

                    //Apply normal style for other words

                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }


        }


        //private void UpdateRTB(RichTextBox rtBox)
        //{
        //    var textRange = new TextRange(rtBox.Document.ContentStart, rtBox.Document.ContentEnd);
        //    var txt = textRange.Text;
        //    txt = txt.Replace("\r\n", " ");
        //    txt = removeSpecialChars(txt);
        //    _chatUtil.Spellcheck.SpellCheck(txt);
        //    if (!string.IsNullOrEmpty(textRange.Text))
        //        updateRTBText(rtBox.Document);
        //}

        /// <summary>
        /// Handles the KeyUp event of the txtChatSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtChatSend_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var rtxtBox = sender as RichTextBox;
                if (e.Key == Key.Space)
                {
                    var endPointer = rtxtBox.CaretPosition;
                    var endIndex = rtxtBox.Document.ContentStart.GetOffsetToPosition(endPointer);
                    var startPointer = rtxtBox.Document.ContentStart.GetPositionAtOffset(endIndex - 1);
                    var wordrange = (new TextRange(startPointer, endPointer));
                    if (wordrange.Text == " ")
                        wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
                if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
                {
                    if (_chatUtil.IsEnableSpellCheck)
                        StartSpellCheck();
                }
                if ((((string)ConfigContainer.Instance().GetValue("chat.enable.typing")).ToLower().Equals("true")) && !_isChatTyping)
                {
                    if (typingTimer != null)
                    {
                        if (typingTimer.Interval < TimeSpan.FromSeconds(Convert.ToInt32(_chatDataContext.ChatTypingTimout)))
                        {
                            typingTimer.Interval += TimeSpan.FromSeconds(Convert.ToInt32(_chatDataContext.ChatTypingTimout));
                            typingTimer.Start();
                        }
                    }
                    else
                    {
                        typingTimer = new DispatcherTimer();
                        typingTimer.Tick += new EventHandler(typingTimer_Tick);
                        typingTimer.Interval = TimeSpan.FromSeconds(Convert.ToInt32(_chatDataContext.ChatTypingTimout));
                        typingTimer.Start();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_KeyUp() :" + generalException.ToString());
            }
        }

        void typingTimer_Tick(object sender, EventArgs e)
        {
            typingTimer.Stop();
            typingTimer.Tick -= typingTimer_Tick;
            typingTimer = null;
            doStopTyping();
        }
        private void doStopTyping()
        {
            ChatMedia chatMedia = new ChatMedia();
            OutputValues output = chatMedia.DoSendTypeStopNotification(_chatUtil.SessionID, "user has stopped typing.");
            if (output.MessageCode == "200")
            {
                _isChatTyping = true;
            }
            chatMedia = null;
        }
        /// <summary>
        /// Handles the Click event of the btnChatPersonDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnChatPersonDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedUser = string.Empty;
                var tempbtn = sender as Button;
                var selectedChatPersonData = DGChatPersonsStatus.SelectedItem as ChatPersonsStatus;
                if (selectedChatPersonData != null)
                {
                    if (getContacts(selectedChatPersonData.PlaceID, _chatUtil.IsChatConferenceClick))
                    {
                        selectedUser = selectedChatPersonData.ChatPersonName;
                        if (_chatUtil.IsChatConferenceClick)
                        {
                            _chatUtil.EnableDelConfMenuitems = true;
                            if (ChatDataContext.GetInstance().IsAvailableVoiceMedia)
                                _chatUtil.EnableCallMenuitems = true;
                            else
                                _chatUtil.EnableCallMenuitems = false;
                            _btnChatPersonDataContextMenu.PlacementTarget = tempbtn;
                            _btnChatPersonDataContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                            _btnChatPersonDataContextMenu.IsOpen = true;
                            _btnChatPersonDataContextMenu.StaysOpen = true;
                            _btnChatPersonDataContextMenu.Focus();
                        }
                        else
                        {
                            _chatUtil.EnableDelConfMenuitems = false;
                            if (ChatDataContext.GetInstance().IsAvailableVoiceMedia)
                                _chatUtil.EnableCallMenuitems = true;
                            else
                                _chatUtil.EnableCallMenuitems = false;
                            _btnChatPersonDataContextMenu.PlacementTarget = tempbtn;
                            _btnChatPersonDataContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                            _btnChatPersonDataContextMenu.IsOpen = true;
                            _btnChatPersonDataContextMenu.StaysOpen = true;
                            _btnChatPersonDataContextMenu.Focus();
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatPersonDetails_Click() :" + generalException.ToString());
            }
        }

        private void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            mainRTB.ScrollToEnd();
            _chatDataContext.RTBTextHasChanged = true;
        }

        private void imgChatAgentInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            popupAgentInfo.IsOpen = true;
            popupAgentInfo.Focusable = false;
            popupAgentInfo.StaysOpen = true;
            popupAgentInfo.PlacementTarget = imgChatAgentInfo;
        }

        private void imgChatAgentInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            popupAgentInfo.IsOpen = false;
        }

        private void borderRecentIXN_MouseEnter(object sender, MouseEventArgs e)
        {
            popupRecentIXNDetails.IsOpen = true;
            popupRecentIXNDetails.Focusable = false;
            popupRecentIXNDetails.StaysOpen = true;
            popupRecentIXNDetails.PlacementTarget = borderRecentIXN;
        }

        private void borderRecentIXN_MouseLeave(object sender, MouseEventArgs e)
        {
            popupRecentIXNDetails.IsOpen = false;
        }

        private void grdMainChatArea_SizeChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                ScrollViewer.UpdateLayout();
                mainRTB.Height = (Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)) < 151 ? 150 : Math.Round(grdMainChatArea.ActualHeight - Math.Round(grdRowStatus.ActualHeight + grdRowSendMainChat.ActualHeight + grdRowConsultChat.ActualHeight)));
                ScrollViewer.UpdateLayout();
                ScrollViewer.ScrollToHome();
                mainRTB.ScrollToEnd();
            }
            catch { }
        }

        #region Contact Notification

        public string ContactUpdation(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList attributeList)
        {
            string contactName = string.Empty;
            try
            {
                if (operationType == "Update")
                {
                    if (contactId.Equals(_chatUtil.ContactID))
                    {
                        AttributesHeader firstNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                        if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                        {
                            contactName += firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                            if (_chatDataContext.CustomerDetails.ContainsKey("FirstName"))
                                _chatDataContext.CustomerDetails["FirstName"] = firstNameHeader.AttributesInfoList[0].AttrValue.ToString();
                        }
                        AttributesHeader LastNameHeader = attributeList.Cast<AttributesHeader>().Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                        if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                        {
                            contactName += LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                            if (_chatDataContext.CustomerDetails.ContainsKey("LastName"))
                                _chatDataContext.CustomerDetails["LastName"] = LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                        }
                        _chatUtil.ChatWindowTitleText = contactName + " - Agent Interaction Desktop";
                        ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = _chatUtil.ChatPersonsStatusInfo;
                        var getUser = tempChatStatus.Where(x => x.ChatPersonName == _chatUtil.ChatFromPersonName).ToList();

                        foreach (var data in getUser)
                        {
                            int position1 = tempChatStatus.IndexOf(tempChatStatus.Where(p => p.ChatPersonName == data.ChatPersonName).FirstOrDefault());
                            _chatUtil.ChatPersonsStatusInfo.RemoveAt(position1);
                            _chatUtil.ChatPersonsStatusInfo.Insert(position1, new ChatPersonsStatus(string.Empty, string.Empty, contactName, _chatUtil.ChatPersonStatusIcon, data.ChatPersonStatus));
                        }
                        _chatUtil.ChatFromPersonName = contactName;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
            return null;
        }

        #endregion

        public void CreateLEXFile(string LEXFile)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(LEXFile)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LEXFile));
                if (!File.Exists(LEXFile))
                {
                    using (StreamWriter streamWriter = new StreamWriter(CustomDictionary, true))
                    {
                        streamWriter.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error While Creating LEX file ( " + LEXFile + ") -" + ex.Message);

            }
        }
    }
}
