namespace Pointel.Interactions.Chat.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Chat.Core;
    using Pointel.Interactions.Chat.Core.General;
    using Pointel.Interactions.Chat.Helpers;
    using Pointel.Interactions.Chat.Settings;
    using Pointel.Interactions.IPlugins;

    /// <summary>
    /// Interaction logic for ChatConsultationWindow.xaml
    /// </summary>
    public partial class ChatConsultationWindow : UserControl
    {
        #region Fields

        public static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();

        string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
        private string CustomDictionary = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\dic\dictionary.lex";
        private List<string> ignoreWords = new List<string>();
        List<TextRange> rangeList = new List<TextRange>();
        string selectedUser = string.Empty;
        Thread updateRTB;
        string userId = string.Empty;
        private ContextMenu _btnChatPersonDataContextMenu = new ContextMenu();
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        ChatUtil _chatUtil = null;
        private string _interactionID = string.Empty;
        private bool _isAgentAliveinChatIXN = false;
        bool _isPasteText = false;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ContextMenu _rtbContextMenu = new ContextMenu();
        private SpellChecker _spellChecker = null;
        TextDecorationCollection _txtDecorationCollection = null;
        private ContextMenu _txtMessageContextMenu = new ContextMenu();
        private ContextMenu _txtSuggestionContextMenu = new ContextMenu();

        #endregion Fields

        #region Constructors

        public ChatConsultationWindow(ChatUtil chatutil, string interactionID)
        {
            try
            {
                InitializeComponent();
                CustomTextDecorationCollection();
                DataObject.AddPastingHandler(txtChatSend, OnPaste);
                _chatUtil = chatutil;
                _interactionID = interactionID;
                this.DataContext = _chatUtil;
                _spellChecker = chatutil.Spellcheck;
                CreateLEXFile(CustomDictionary);
            }
            catch (Exception genralException)
            {
                _logger.Error("Error occurred in ChatConsultationWindow() : " + genralException.ToString());
            }
        }

        ~ChatConsultationWindow()
        {
            try
            {
                _spellChecker = null;
                //_txtSuggestionContextMenu.Items.Clear();
                _txtSuggestionContextMenu = null;
                //_txtMessageContextMenu.Items.Clear();
                _txtMessageContextMenu = null;
                //_rtbContextMenu.Items.Clear();
                _rtbContextMenu = null;
                _chatUtil.ConsultChatWindowRowHeight = new GridLength(0);
                _chatUtil.RTBConsultDocument = null;
                // _chatUtil.RTBConsultDocument.Blocks.Clear();
                _interactionID = null;
                //_btnChatPersonDataContextMenu.Items.Clear();
                //_btnChatPersonDataContextMenu = null;
                _txtDecorationCollection = null;
                selectedUser = null;
                userId = null;
                _chatUtil = null;
                mainRTB.Document = new FlowDocument();
                mainRTB.Documents = new FlowDocument();
            }
            catch (Exception genralException)
            {
                _logger.Error("Error occurred in ~ChatConsultationWindow() : " + genralException.ToString());
            }
        }

        #endregion Constructors

        #region Methods

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

        private void AddToDictionary(string entry)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(CustomDictionary, true))
                {
                    streamWriter.WriteLine(entry);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while AddToDictionary() :" + generalException.ToString());
            }
        }

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
                        if (menuitem.Header.ToString() == "Delete from Consultation")
                        {
                            ObservableCollection<Pointel.Interactions.Chat.Helpers.IPartyInfo> temp = _chatUtil.PartiesInfo;
                            var getUserNickName = temp.Where(x => x.UserNickName == selectedUser).ToList();
                            if (getUserNickName.Count > 0)
                            {
                                foreach (var item in getUserNickName)
                                {
                                    userId = item.UserID;
                                    agentNickName = item.UserNickName;
                                }
                            }
                            else
                            {
                                var getTypeName = temp.Where(x => x.UserType == ChatDataContext.ChatUsertype.Client.ToString()).ToList();
                                if (getTypeName.Count > 0)
                                    foreach (var item in getTypeName)
                                    {
                                        userId = item.UserID;
                                        agentNickName = item.UserNickName;
                                    }
                            }
                            if (userId != string.Empty)
                            {
                                if (_chatUtil.IsChatConferenceClick)
                                {
                                    output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userId);
                                    _chatUtil.IsChatConferenceClick = false;
                                }
                                else
                                {
                                    output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userId, _chatDataContext.ChatFareWellMessage);
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

                                //added for delete user after delete from consultation
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
                                userId = string.Empty;
                                selectedUser = string.Empty;
                            }
                        }
                        else if (menuitem.Header.ToString().Contains("Call"))
                        {
                            var phoneNumber = menuitem.Tag.ToString();
                            phoneNumber = phoneNumber.Trim();
                            if (ChatDataContext.messageToClientChat != null)
                            {
                                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, phoneNumber);
                                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, "DIAL");
                            }
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

        private void btnChatPersonDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedUser = string.Empty;
                var tempbtn = sender as Button;
                var selectedChatPersonData = DGChatPersonsStatus.SelectedItem as ChatPersonsStatus;
                if (selectedChatPersonData != null)
                {
                    if (getContacts(selectedChatPersonData))
                    {
                        selectedUser = selectedChatPersonData.ChatPersonName;
                        _btnChatPersonDataContextMenu.PlacementTarget = tempbtn;
                        _btnChatPersonDataContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        _btnChatPersonDataContextMenu.IsOpen = true;
                        _btnChatPersonDataContextMenu.StaysOpen = true;
                        _btnChatPersonDataContextMenu.Focus();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnChatPersonDetails_Click() :" + generalException.ToString());
            }
        }

        private void btnChatRelease_Click(object sender, RoutedEventArgs e)
        {
            ChatMedia chatMedia = new ChatMedia();
            try
            {
                if (_chatUtil.ConsultReleaseText == "Release")
                {
                    string userID = string.Empty;
                    string visibility = string.Empty;
                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatPersonsStatus> tempChatStatus = _chatUtil.ChatPersonsStatusInfo;
                    var getClient = tempChatStatus.Where(x => x.ChatPersonName == _chatUtil.ChatFromPersonName).ToList();
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
                        OutputValues output = null;
                        if (_chatUtil.IsChatConferenceClick)
                        {
                            var getOtherUserNickName = temp.Where(x => x.UserType == ChatDataContext.ChatUsertype.Agent.ToString()).ToList();
                            if (getOtherUserNickName != null && getOtherUserNickName.Count > 0 && visibility == "All")
                            {
                                foreach (var item in getOtherUserNickName)
                                {
                                    if (item.Visibility == "Int")
                                    {
                                        output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, item.UserID, "");
                                    }
                                }
                            }
                            else
                            {
                                output = chatMedia.DoLeaveInteractionFromConference(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                if (output.MessageCode == "200")
                                {
                                    output = chatMedia.DoKeepAliveReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID);
                                }
                            }
                            if (output.MessageCode == "200")
                            {
                                _chatUtil.IsChatConferenceClick = false;
                                foreach (var data in getClient)
                                {
                                    _chatUtil.ConsultPersonStatus = "Ended";
                                }
                                _chatUtil.ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat/Chat.MarkDone.png", UriKind.Relative));
                                _chatUtil.ConsultReleaseText = "Done";
                                _chatUtil.ConsultReleaseTTHeading = "Mark Done";
                                lblTabItemShowTimer.Stop_CustomTimer();
                                _chatUtil.SendchatConsultWindowRowHeight = new GridLength(0);
                            }
                        }
                        else
                        {
                            output = chatMedia.DoReleasePartyChatSession(_chatUtil.SessionID, _chatUtil.ProxyId, userID, _chatDataContext.ChatFareWellMessage);
                            if (output.MessageCode == "200")
                            {
                                foreach (var data in getClient)
                                {
                                    _chatUtil.ConsultPersonStatus = "Ended";
                                }
                                _chatUtil.ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat/Chat.MarkDone.png", UriKind.Relative));
                                _chatUtil.ConsultReleaseText = "Done";
                                _chatUtil.ConsultReleaseTTHeading = "Mark Done";
                                lblTabItemShowTimer.Stop_CustomTimer();
                                _chatUtil.SendchatConsultWindowRowHeight = new GridLength(0);
                            }
                        }
                    }
                }
                else
                {
                    try
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
                                else if (item.UserNickName == _chatDataContext.AgentNickName && item.UserState == "Connected")
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
                            _chatUtil.ConsultChatWindowRowHeight = new GridLength(0);
                            _chatUtil.RTBConsultDocument.Blocks.Clear();
                            mainRTB.Document = new FlowDocument();
                            mainRTB.Documents = new FlowDocument();
                            _chatUtil.ConsultDockPanel.Children.Clear();
                            //_chatDataContext.ConsultUserControl = null;
                        }
                        else
                        {
                            OutputValues output = chatMedia.DoStopInteraction(_interactionID, _chatUtil.ProxyId);
                            if (output.MessageCode == "200")
                            {
                                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                                    ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(_interactionID,
                                        _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                                        _chatUtil.UserData, 3, DateTime.UtcNow.ToString());
                                _chatUtil.ConsultChatWindowRowHeight = new GridLength(0);
                                _chatUtil.RTBConsultDocument.Blocks.Clear();
                                mainRTB.Document = new FlowDocument();
                                mainRTB.Documents = new FlowDocument();
                                _chatUtil.ConsultDockPanel.Children.Clear();
                                //_chatDataContext.ConsultUserControl = null;
                            }
                        }

                    }
                    catch (Exception generalException)
                    {
                        _logger.Error(" Error occurred while btnChatDone_Click() :" + generalException.ToString());
                    }
                }
                if (this.Parent is ContentControl)
                    if (((this.Parent as ContentControl).Parent is DockPanel))
                        ((this.Parent as ContentControl).Parent as DockPanel).Children.Clear();
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while btnChatRelease_Click () : " + generalException.ToString());
            }
            finally
            {
                chatMedia = null;
            }
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var range = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
                if (range.Text != string.Empty && range.Text != "\r\n")
                {
                    if (_chatUtil.Spellcheck.MisspelledWords.Count > 0)
                        if ((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                               ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                        {
                            var showMessageBox = new Pointel.Interactions.Chat.WinForms.MessageBox("Information",
                                              "Please check the misspelled words", "OK", "Cancel", false);
                            showMessageBox.Owner = Window.GetWindow(this);
                            showMessageBox.ShowDialog();
                            if (showMessageBox.DialogResult == true || showMessageBox.DialogResult == false)
                            {
                                return;
                            }
                        }
                    range.Text = range.Text.Replace("\r\n", "");
                    _chatUtil.ChatConsultMessageText = range.Text.ToString();
                    ChatMedia chatMedia = new ChatMedia();
                    OutputValues output = chatMedia.DoSendConsultMessage(_chatUtil.SessionID, _chatUtil.ChatConsultMessageText);
                    if (output.MessageCode == "200")
                    {
                        txtChatSend.Document.Blocks.Clear();
                        txtChatSend.Focus();
                        _chatUtil.ChatConsultMessageText = string.Empty;
                    }
                    chatMedia = null;
                }
                else
                {
                    txtChatSend.Document.Blocks.Clear();
                    txtChatSend.Focus();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnSendMessage_Click() :" + generalException.ToString());
            }
        }

        private void btnShowHideConsultEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnShowHideConsultEx.Tag.ToString() == "Show")
                {
                    _chatUtil.ConsultChatRowHight = GridLength.Auto;
                    _chatUtil.ImgConsultChatExpander = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/upArrow.png", UriKind.Relative));
                    btnShowHideConsultEx.Tag = "Hide";
                }
                else
                {
                    _chatUtil.ConsultChatRowHight = new GridLength(0);
                    _chatUtil.ImgConsultChatExpander = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/downArrow.png", UriKind.Relative));
                    btnShowHideConsultEx.Tag = "Show";
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while btnShowHideConsultEx_Click() :" + generalException.ToString());
            }
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

        private void CustomTextDecorationCollection()
        {
            try
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
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while CustomTextDecorationCollection() :" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the contacts.
        /// </summary>
        private bool getContacts(ChatPersonsStatus data)
        {
            bool isSuccess = false;
            string placeID = string.Empty;
            try
            {
                _btnChatPersonDataContextMenu.Items.Clear();
                if (data.PlaceID == string.Empty || data.PlaceID == "")
                {
                    string employeeID = data.AgentID;
                    if (_chatUtil.ChatParties.Count > 0 && !string.IsNullOrEmpty(employeeID))
                    {
                        if (_chatUtil.ChatParties.ContainsKey(employeeID))
                            placeID = _chatUtil.ChatParties[employeeID].ToString();
                    }
                    if (string.IsNullOrEmpty(placeID))
                    {
                        isSuccess = false;
                    }
                    else
                    {
                        var _mnuPhoneNumber = new MenuItem();
                        _mnuPhoneNumber.Header = "Call ";
                        _mnuPhoneNumber.Tag = getDNFromPlace(placeID.ToString().Trim());
                        _mnuPhoneNumber.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Voice.png", UriKind.Relative)) };
                        _mnuPhoneNumber.Margin = new System.Windows.Thickness(2);
                        _mnuPhoneNumber.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                        _btnChatPersonDataContextMenu.Items.Add(_mnuPhoneNumber);
                        _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                        isSuccess = true;
                    }
                }
                else
                {
                    var _mnuPhoneNumber = new MenuItem();
                    _mnuPhoneNumber.Header = "Call ";
                    _mnuPhoneNumber.Tag = getDNFromPlace(data.PlaceID.ToString().Trim());
                    _mnuPhoneNumber.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = _chatDataContext.GetBitmapImage(new Uri(_chatDataContext.Imagepath + "\\Voice.png", UriKind.Relative)) };
                    _mnuPhoneNumber.Margin = new System.Windows.Thickness(2);
                    _mnuPhoneNumber.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                    _btnChatPersonDataContextMenu.Items.Add(_mnuPhoneNumber);
                    _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                    isSuccess = true;
                }
                ObservableCollection<IPartyInfo> temp = _chatUtil.PartiesInfo;
                var getAliveUser = temp.Where(x => x.UserState == "Connected" && x.UserType != ChatDataContext.ChatUsertype.Client.ToString() && x.Visibility == "Int" && x.PersonId != _chatDataContext.AgentID).ToList();
                if (getAliveUser.Count >= 1 && _chatUtil.IxnType != "Consult")
                {
                    foreach (var item in getAliveUser)
                    {
                        var _mnuDeleteConference = new MenuItem();
                        _mnuDeleteConference.Header = "Delete from Consultation";
                        _mnuDeleteConference.Margin = new System.Windows.Thickness(2);
                        _mnuDeleteConference.Click += new RoutedEventHandler(btnChatPersonDataContextMenu_Click);
                        _btnChatPersonDataContextMenu.Items.Add(_mnuDeleteConference);
                        _btnChatPersonDataContextMenu.Style = (Style)FindResource("Contextmenu");
                        isSuccess = true;
                        break;
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("getContacts() :" + generalException.ToString());
                isSuccess = false;
            }
            return isSuccess;
        }

        private string GetWordAtPointer(TextPointer textPointer)
        {
            return string.Join(string.Empty, GetWordCharactersBefore(textPointer), GetWordCharactersAfter(textPointer));
        }

        private string GetWordCharactersAfter(TextPointer textPointer)
        {
            var fowards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(fowards.TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).ToArray());
            return wordCharactersAfterPointer;
        }

        private string GetWordCharactersBefore(TextPointer textPointer)
        {
            var backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).Reverse().ToArray());
            return wordCharactersBeforePointer;
        }

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

        private void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            mainRTB.ScrollToEnd();
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
        void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while MenuItemChecked() :" + generalException.ToString());
            }
        }

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
                                AddToDictionary(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                _chatUtil.Spellcheck.AddTodictionary(_chatUtil.Spellcheck.SelectedMisspelledWord);
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

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
                if (!isText) return;
                var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
                if (!string.IsNullOrEmpty(text))
                    _isPasteText = true;
            }
            catch (Exception genralException)
            {
                _logger.Error("Error occurred in OnPaste() : " + genralException.ToString());
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

        private void ReplaceWordAtPointer(TextPointer textPointer, string replacementWord)
        {
            try
            {
                textPointer.DeleteTextInRun(-GetWordCharactersBefore(textPointer).Count());
                textPointer.DeleteTextInRun(GetWordCharactersAfter(textPointer).Count());
                textPointer.InsertTextInRun(replacementWord);
                txtChatSend.CaretPosition = textPointer.GetNextContextPosition(LogicalDirection.Forward);
                UpdateRTB_BackgroundWorker();
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while ReplaceWordAtPointer() :" + generalException.ToString());
            }
        }

        void rtbContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Copy")
                        {
                            mainRTB.Copy();
                        }
                        else if (menuitem.Header.ToString() == "Select All")
                        {
                            mainRTB.SelectAll();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while rtbContextMenu_Click() :" + exception.ToString());
            }
        }

        //private void IgnoreAll(string word)
        //{
        //    _chatUtil.Spellcheck.MisspelledWords.Remove(word);
        //    ignoreWords.Add(word);
        //    var textRange = new TextRange(txtChatSend.Document.ContentStart, txtChatSend.Document.ContentEnd);
        //    var txt = textRange.Text;
        //    txt = txt.Replace("\r\n", " ");
        //    txt = removeSpecialChars(txt);
        //    var tempWords = txt.Split(' ');
        //    if (tempWords != null && tempWords.Length > 0)
        //        updateRTBText(txtChatSend.Document);
        //}
        private void StartSpellCheck()
        {
            try
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
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while StartSpellCheck() :" + generalException.ToString());
            }
        }

        private void StopSpellCheck()
        {
            try
            {
                _chatUtil.Spellcheck.MisspelledWords.Clear();
                UpdateRTB_BackgroundWorker();
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while StopSpellCheck() :" + generalException.ToString());
            }
        }

        private void txtChatSend_KeyDown(object sender, KeyEventArgs e)
        {
            _chatUtil.ErrorRowHeight = new GridLength(0);
            _chatUtil.ErrorMessage = string.Empty;
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
                        var txtBox = e.Source as RichTextBox;
                        var textRange = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                        string Richtextvalue = textRange.Text;
                        if (Richtextvalue != string.Empty && Richtextvalue != "\r\n")
                        {
                            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                            {
                                txtBox.AcceptsReturn = true;
                            }
                            else
                            {
                                e.Handled = true;
                                btnSendMessage.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            }
                        }
                        else
                        {
                            txtBox.Document.Blocks.Clear();
                            txtBox.CaretPosition = txtBox.Document.ContentStart;
                        }
                        break;
                }

            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_KeyDown() :" + generalException.ToString());
            }
        }

        //private void updateRTBText(FlowDocument document)
        //{
        //    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(delegate
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
        //                            wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, CustomTextDecorationCollection());
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
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_KeyUp() :" + generalException.ToString());
            }
        }

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
                                //var t = start.GetTextInRun(LogicalDirection.Backward);
                                //var end = start.GetNextContextPosition(LogicalDirection.Backward);
                                //var t1 = end.GetTextInRun(LogicalDirection.Backward);
                                //txtChatSend.Selection.Select(start, end);
                                _chatUtil.Spellcheck.SelectedMisspelledWord = word;
                                _chatUtil.Spellcheck.LoadSuggestions(_chatUtil.Spellcheck.SelectedMisspelledWord);
                                foreach (var item in _spellChecker.SuggestedWords)
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

        private void txtChatSend_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var rtxtBox = sender as RichTextBox;
                if (rtxtBox != null)
                {
                    var textRange = new TextRange(rtxtBox.Document.ContentStart, rtxtBox.Document.ContentEnd);
                    if (textRange.Text != string.Empty && textRange.Text.Length > 0 && textRange.Text != "\r\n\r\n" && textRange.Text != "\r\n")
                    {
                        _chatUtil.IsButtonConsultSendEnabled = true;
                        if (_isPasteText)
                        {
                            _isPasteText = false;
                            if (_chatUtil.IsEnableSpellCheck)
                                StartSpellCheck();
                        }
                    }
                    else
                    {
                        _chatUtil.IsButtonConsultSendEnabled = false;
                        rtxtBox.Document.Blocks.Clear();
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while txtChatSend_TextChanged() :" + generalException.ToString());
            }
        }

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

        private void UpdateRTB_BackgroundWorker()
        {
            try
            {
                if (updateRTB != null && updateRTB.IsAlive)
                    updateRTB.Abort();
                updateRTB = new Thread(UpdateSpellcheck);
                updateRTB.IsBackground = true;
                updateRTB.Priority = ThreadPriority.BelowNormal;
                updateRTB.Start();
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while UpdateRTB_BackgroundWorker() :" + generalException.ToString());
            }
        }

        private void UpdateSpellcheck()
        {
            try
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
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while UpdateSpellcheck() :" + generalException.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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
                _chatUtil.ConsultChatRowHight = GridLength.Auto;
                _chatUtil.ImgConsultChatExpander = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/upArrow.png", UriKind.Relative));
                btnShowHideConsultEx.Tag = "Hide";
                _chatUtil.ConsultPersonStatus = "Connected";
                _chatUtil.ConsultReleaseImageSource = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat/Chat.Release.png", UriKind.Relative));
                // = new BitmapImage(new Uri("/Pointel.Interactions.Chat;component/Images/Chat/Chat.MarkDone.Disable.png", UriKind.Relative));
                _chatUtil.ConsultReleaseText = "Release";
                _chatUtil.ConsultReleaseTTHeading = "End Chat";
                _chatUtil.ConsultReleaseTTContent = "Release the Chat.";
                _chatUtil.IsButtonConsultSendEnabled = false;
                _chatUtil.SendchatConsultWindowRowHeight = GridLength.Auto;

                //Done
                //Mark Done
                //Agent can mark done after release the chat interaction.
            }
            catch (Exception genralException)
            {
                _logger.Error("Error occurred in UserControl_Loaded() : " + genralException.ToString());
            }
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
        #endregion Methods
    }
}