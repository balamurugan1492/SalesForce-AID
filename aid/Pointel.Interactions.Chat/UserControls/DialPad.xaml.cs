namespace Pointel.Interactions.Chat.UserControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Chat.ApplicationReader;
    using Pointel.Interactions.Chat.Helpers;
    using Pointel.Interactions.Chat.Settings;

    /// <summary>
    /// Interaction logic for DialPad.xaml
    /// </summary>
    public partial class DialPad : UserControl
    {
        #region Fields

        public Hashtable agentContact = new Hashtable();
        public Hashtable applcationContact = new Hashtable();
        public Hashtable groupContact = new Hashtable();
        public bool isGlobalContactChecked = false;
        public bool isGroupContactChecked = false;
        public bool isLocalContactChecked = false;
        public string searchContact = string.Empty;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private string SearchText = string.Empty;
        private Hashtable _agentContact = new Hashtable();
        private Hashtable _applcationContact = new Hashtable();
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        ChatUtil _chatUtil = null;
        private Hashtable _groupContact = new Hashtable();
        private ContextMenu _phoneBookMenu = new ContextMenu();
        private string _phoneBookText = string.Empty;
        private StringBuilder _typednumber = new StringBuilder();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DialPad"/> class.
        /// </summary>
        public DialPad(ChatUtil chatUtil)
        {
            InitializeComponent();
            _chatUtil = chatUtil;
            this.DataContext = _chatUtil;
            txtNumbers.Focus();
            _chatUtil.Contacts.Clear();
            _chatUtil.ContactsFilter.Clear();
            _chatUtil.DialedNumbers = string.Empty;
            MenuItem mnuItem = new MenuItem();
            mnuItem.Header = "Dial";
            mnuItem.VerticalContentAlignment = VerticalAlignment.Center;
            mnuItem.Height = 18;
            mnuItem.Background = System.Windows.Media.Brushes.Transparent;
            mnuItem.Icon = new System.Windows.Controls.Image { Height = 12, Width = 12, Source = _chatDataContext.GetBitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative)) };
            mnuItem.Click += new RoutedEventHandler(mnuItem_Click);
            _phoneBookMenu.Items.Add(mnuItem);
            if (_chatUtil.DialedNumbers.Length >= 9 && _chatUtil.ModifiedTextSize != 0)
            {
                txtNumbers.FontSize = _chatUtil.ModifiedTextSize;
            }
            Loaded += (s, e) =>
            {
                Window.GetWindow(this)
                      .Closing += (s1, e1) => Somewhere();
            };

            if (ConfigContainer.Instance().AllKeys.Contains("voice.enable.phonebook.double-click-to-call") && ((string)ConfigContainer.Instance().GetValue("voice.enable.phonebook.double-click-to-call")).ToLower().Equals("true"))
                dgvContact.MouseDoubleClick += new MouseButtonEventHandler(dgvContact_MouseDoubleClick);
        }

        #endregion Constructors

        #region Delegates

        public delegate void LoadContacts(object contacts);

        #endregion Delegates

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
        /// Keyboard values the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string Keyboardvalue(Key key)
        {
            string value = string.Empty;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (key == Key.D3)
                {
                    value = "#";
                }
                else if (key == Key.D8)
                {
                    value = "*";
                }
            }
            else
            {
                if (key == Key.D0 || key == Key.NumPad0)
                    value = "0";
                if (key == Key.D1 || key == Key.NumPad1)
                    value = "1";
                if (key == Key.D2 || key == Key.NumPad2)
                    value = "2";
                if (key == Key.D3 || key == Key.NumPad3)
                    value = "3";
                if (key == Key.D4 || key == Key.NumPad4)
                    value = "4";
                if (key == Key.D5 || key == Key.NumPad5)
                    value = "5";
                if (key == Key.D6 || key == Key.NumPad6)
                    value = "6";
                if (key == Key.D7 || key == Key.NumPad7)
                    value = "7";
                if (key == Key.D8 || key == Key.NumPad8)
                    value = "8";
                if (key == Key.D9 || key == Key.NumPad9)
                    value = "9";
                if (key == Key.Multiply)
                    value = "*";
            }
            return value;
        }

        /// <summary>
        /// Handles the Checked event of the ChkAgentLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkAgentLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                //Code added by Manikandan on 26-Nov-2013
                if (((string)ConfigContainer.Instance().GetValue("voice.enable.read-agent-annex-value")).ToLower().Equals("true"))
                {
                    if (ConfigContainer.Instance().AllKeys.Contains("AgentContacts"))
                        if (ConfigContainer.Instance().GetValue("AgentContacts") != null)
                            _chatDataContext.AnnexContacts = ConfigContainer.Instance().GetValue("AgentContacts");
                    _agentContact.Clear();
                    if (_chatDataContext.AnnexContacts != null && _chatDataContext.AnnexContacts.Count > 0)
                    {
                        foreach (string key in _chatDataContext.AnnexContacts.Keys)
                        {
                            if (!_agentContact.ContainsKey(key))
                            {
                                _agentContact.Add(key, _chatDataContext.AnnexContacts[key].ToString());
                            }
                        }
                    }
                }
                else
                {
                    XMLHandler xmlHandler = new XMLHandler();
                    Dictionary<string, string> xmlContacts = xmlHandler.LoadXmlContacts(_chatDataContext.SpeedDialXMLFile);
                    _agentContact.Clear();
                    if (xmlContacts.Count > 0)
                    {
                        foreach (string key in xmlContacts.Keys)
                        {
                            if (!_agentContact.ContainsKey(key))
                            {
                                _agentContact.Add(key, xmlContacts[key].ToString());
                            }
                        }
                    }
                }
                //code Ended
                //agentContact = (Hashtable)Datacontext.GetInstance().ConfiguredSpeedDial;
                if (!_agentContact.ContainsKey("type"))
                {
                    _agentContact.Add("type", "11");
                }
                else
                {
                    _agentContact["type"] = "11";
                }
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_agentContact);
                isLocalContactChecked = true;
            }
            catch (Exception generalException)
            {
                logger.Error("chkAgentLevel_CheckedChanged : Error occurred while getting contact details from agent level " +
                    generalException.ToString());
            }
            finally
            {
                contactControl = null;
                //GC.Collect();
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkAgentLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkAgentLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                agentContact["type"] = 21;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)agentContact);
                isLocalContactChecked = false;
            }
            catch { }
        }

        /// <summary>
        /// Handles the Checked event of the ChkApplicationLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkApplicationLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                //if (ConfigContainer.Instance().AllKeys.Contains("GlobalContacts") && ConfigContainer.Instance().GetValue("GlobalContacts") != null)
                //    ProcessConfigurationData(ConfigContainer.Instance().GetValue("GlobalContacts"));

                if (ConfigContainer.Instance().AllKeys.Contains("GlobalContacts") && ConfigContainer.Instance().GetValue("GlobalContacts") != null)
                {
                    try
                    {
                        _chatDataContext.HshApplicationLevel.Clear();
                        foreach (var name in ((KeyValueCollection)ConfigContainer.Instance().GetValue("GlobalContacts")).AllKeys.Where(name => !_chatDataContext.HshApplicationLevel.ContainsKey(name)))
                            _chatDataContext.HshApplicationLevel.Add(name, ((KeyValueCollection)ConfigContainer.Instance().GetValue("GlobalContacts"))[name].ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error loading when Global contacts : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                    }
                }

                _applcationContact = (Hashtable)_chatDataContext.HshApplicationLevel;
                if (!_applcationContact.ContainsKey("type"))
                {
                    _applcationContact.Add("type", 12);
                }
                else
                {
                    _applcationContact["type"] = 12;
                }
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)_applcationContact);
                isGlobalContactChecked = true;
            }
            catch (Exception commonException)
            {
                logger.Error("chkApplicationLevel_CheckedChanged : Error occurred while getting contact details from application level " +
                    commonException.ToString());
            }
            finally
            {
                contactControl = null;
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkApplicationLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkApplicationLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                applcationContact["type"] = 22;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)applcationContact);
                isGlobalContactChecked = false;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Checked event of the ChkGroupLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkGroupLevel_Checked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("GroupContacts") && ConfigContainer.Instance().GetValue("GroupContacts") != null)
                {
                    ChatDataContext.hshLoadGroupContact.Clear();
                    try
                    {
                        foreach (string name in ((KeyValueCollection)ConfigContainer.Instance().GetValue("GroupContacts")).Keys)
                            ChatDataContext.hshLoadGroupContact.Add(name, ((KeyValueCollection)ConfigContainer.Instance().GetValue("GroupContacts"))[name].ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error loading when Group contacts : " + ((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString()));
                    }
                }

                groupContact = (Hashtable)ChatDataContext.hshLoadGroupContact;

                if (!groupContact.ContainsKey("type"))
                {
                    groupContact.Add("type", 13);
                }
                else
                {
                    groupContact["type"] = 13;
                }
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)groupContact);
                isGroupContactChecked = true;
            }
            catch (Exception generalException)
            {
                logger.Error("chkGroup_CheckedChanged : Error occurred while getting contact details from agent group level " +
                 generalException.ToString());
            }
            finally
            {
                contactControl = null;
                //GC.Collect();
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the ChkGroupLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ChkGroupLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread contactControl;
            try
            {
                groupContact["type"] = 23;
                contactControl = new Thread(new ParameterizedThreadStart(ContactController));
                contactControl.Start((object)groupContact);
                isGroupContactChecked = false;
            }
            catch { }
        }

        /// <summary>
        /// Contacts the controller.
        /// </summary>
        /// <param name="contacts">The contacts.</param>
        private void ContactController(object contacts)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    if (contacts != null)
                    {
                        Hashtable contact = (Hashtable)contacts;

                        if (contact.ContainsKey("type"))
                        {
                            //Add agent/application/global contact in Grid

                            if (string.Compare(contact["type"].ToString(), "11", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "12", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "13", true) == 0)
                            {

                                IDictionaryEnumerator keys = contact.GetEnumerator();
                                while (keys.MoveNext())
                                {
                                    if (keys.Key.ToString() != "type")
                                    {
                                        _chatUtil.Contacts.Add(new Contacts(keys.Key.ToString(), keys.Value.ToString(), contact["type"].ToString()));
                                    }
                                }
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    dgvContact.ItemsSource = _chatUtil.Contacts;
                                    dgvContact.UpdateLayout();
                                    if (dgvContact.Items.Count > 1)
                                        dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                                    _chatUtil.ContactsFilter.Clear();
                                }
                                else
                                {
                                    ContactSearch(txtContactSearch.Text);
                                }

                                if (dgvContact.Items.Count.ToString() == "0")
                                {
                                    // btnDial.IsEnabled = false;
                                }
                                else
                                {
                                    //btnDial.IsEnabled = true;
                                }
                            }
                            else if (string.Compare(contact["type"].ToString(), "21", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "22", true) == 0 ||
                                string.Compare(contact["type"].ToString(), "23", true) == 0)
                            {
                                ObservableCollection<IContacts> temp = null;
                                ObservableCollection<IContacts> temp1 = null;
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    temp = _chatUtil.Contacts;
                                    temp1 = _chatUtil.ContactsFilter;
                                }
                                else
                                {
                                    temp = _chatUtil.ContactsFilter;
                                    temp1 = _chatUtil.Contacts;
                                }
                                if (temp.Count > 0)
                                {
                                    if (string.Compare(contact["type"].ToString(), "21", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "11").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "11").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    else if (string.Compare(contact["type"].ToString(), "22", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "12").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "12").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    else if (string.Compare(contact["type"].ToString(), "23", true) == 0)
                                    {
                                        var toRemove = temp.Where(x => x.Type == "13").ToList();
                                        foreach (var item in toRemove)
                                            temp.Remove(item);
                                        var toRemove1 = temp1.Where(x => x.Type == "13").ToList();
                                        foreach (var item in toRemove1)
                                            temp1.Remove(item);
                                    }
                                    dgvContact.ItemsSource = temp;
                                    dgvContact.UpdateLayout();
                                    if (dgvContact.Items.Count > 1)
                                        dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                                }
                                if (string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                                }
                                else if (!string.IsNullOrEmpty(txtContactSearch.Text))
                                {
                                    lblStatus.Content = dgvContact.Items.Count.ToString() + " matches found ";
                                }

                                if (dgvContact.Items.Count.ToString() == "0")
                                {
                                    // btnDial.IsEnabled = false;
                                }
                                else
                                {
                                    // btnDial.IsEnabled = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("No contacts available");
                    }
                }));
            }
            catch (ThreadAbortException threadException)
            {
                logger.Error("DialPad:ContactController:" + threadException.ToString());
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Add/Remove contacts from grid " + generalException.ToString());
            }
            finally
            {
            }
        }

        //Below code added for implement contact search
        //SMoorthy - 07-01-2014
        void ContactSearch(object value)
        {
            string searchString = string.Empty;
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    if (value != null)
                    {
                        searchString = Convert.ToString(value).ToLower();
                        _chatUtil.ContactsFilter.Clear();
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            for (int index = 0; index < _chatUtil.Contacts.Count; index++)
                            {
                                if (_chatUtil.Contacts[index].Name.ToString().ToLower().StartsWith(searchString))
                                {
                                    _chatUtil.ContactsFilter.Add(_chatUtil.Contacts[index]);
                                }
                            }
                            dgvContact.ItemsSource = _chatUtil.ContactsFilter;
                            dgvContact.UpdateLayout();
                            if (dgvContact.Items.Count > 1)
                                dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                        }
                        else if (string.IsNullOrEmpty(searchString))
                        {
                            dgvContact.ItemsSource = _chatUtil.Contacts;
                            dgvContact.UpdateLayout();
                            if (dgvContact.Items.Count > 1)
                                dgvContact.ScrollIntoView(dgvContact.Items[dgvContact.Items.Count - 1]);
                        }

                        //Notify total contact
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            lblStatus.Content = dgvContact.Items.Count.ToString() + " matches found ";
                        }
                        else if (string.IsNullOrEmpty(searchString))
                        {
                            lblStatus.Content = dgvContact.Items.Count.ToString() + " contacts available ";
                        }
                    }
                }));
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while searching contact in data grid " +
                    generalException.ToString());
            }
            finally
            {

            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the dgvContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void dgvContact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgvContact.SelectedIndex >= 0)
                Dial_Click(null, null);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the dgvContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void dgvContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgvContact.SelectedItem is Contacts)
                {
                    Contacts temp = dgvContact.SelectedItem as Contacts;
                    if (temp != null && !string.IsNullOrEmpty(temp.Number))
                    {
                        if (ChatDataContext.messageToClientChat != null)
                            ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Email, temp.Number);
                    }
                }

            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the MouseLeftClick event of the DialTabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DialTabItem_MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TabItem)
            {
                var tabItem = sender as TabItem;
                if (tabItem.Name == "tabDial")
                {
                    ChkAgentLevel.IsChecked = false;
                    ChkApplicationLevel.IsChecked = false;
                    ChkGroupLevel.IsChecked = false;
                    txtNumbers.Text = string.Empty;
                }
                else
                {
                    txtNumbers.Text = string.Empty;
                    _typednumber.Clear();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Dial control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Dial_Click(object sender, RoutedEventArgs e)
        {
            //if (WindowType == ChatDataContext.DialPadType.Transfer)
            //{
            //    if (_chatDataContext.UserSetTransType == ChatDataContext.ConsultType.OneStep)
            //    {
            //        _chatDataContext.IsInitiateConfClicked = false;
            //        _chatDataContext.IsInitiateTransClicked = true;
            //    }
            //    if (_chatDataContext.UserSetTransType == ChatDataContext.ConsultType.DualStep)
            //    {
            //        _chatDataContext.IsInitiateConfClicked = false;
            //        _chatDataContext.IsInitiateTransClicked = true;
            //    }
            //}
            //if (WindowType == ChatDataContext.DialPadType.Conference)
            //{
            //    if (_chatDataContext.UserSetConfType == ChatDataContext.ConsultType.OneStep)
            //    {
            //        _chatDataContext.IsInitiateConfClicked = true;
            //        _chatDataContext.IsInitiateTransClicked = false;
            //    }
            //    if (_chatDataContext.UserSetConfType == ChatDataContext.ConsultType.DualStep)
            //    {
            //        _chatDataContext.IsInitiateConfClicked = true;
            //        _chatDataContext.IsInitiateTransClicked = false;
            //    }
            //}

            //if (_chatDataContext.ThisDN != chatUtil.DialedNumbers)
            //{
            //    if (_chatDataContext.IsInitiateTransClicked)
            //    {
            //        _chatDataContext.cmshow.IsOpen = false;
            //        _chatDataContext.IsTransDialPadOpen = false;
            //        eventFireBackNum.Invoke("transfer");
            //    }
            //    else if (_chatDataContext.IsInitiateConfClicked)
            //    {
            //        _chatDataContext.cmshow.IsOpen = false;
            //        _chatDataContext.IsConfDialPadOpen = false;
            //        eventFireBackNum.Invoke("conference");
            //    }
            //    else
            //    {
            //        _chatDataContext.cmshow.IsOpen = false;
            //        eventFireBackNum.Invoke("DIAL");
            //    }
            //}
            //else
            //{
            //    _chatDataContext.UserSetConfType = ChatDataContext.ConsultType.None;
            //    _chatDataContext.UserSetTransType = ChatDataContext.ConsultType.None;
            //}
            if (ChatDataContext.messageToClientChat != null)
                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, "DIAL");
            _chatDataContext.cmshow.StaysOpen = false;
            _chatDataContext.cmshow.Items.Clear();
        }

        /// <summary>
        /// Handles the Click event of the Double control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Double_Click(object sender, RoutedEventArgs e)
        {
            //_chatDataContext.Dualclick = true;
            //_chatDataContext.Singleclick = false;
        }

        private void mnuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ChatDataContext.messageToClientChat != null && !String.IsNullOrEmpty(_phoneBookText))
            {
                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, _phoneBookText);
                Dial_Click(null, null);
            }
        }

        /// <summary>
        /// Handles the Click event of the Number control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button number = sender as Button;
            if (_chatUtil.DialedNumbers.Length < _chatDataContext.MaxDialDigits)
            {
                if (_chatUtil.DialedNumbers.Length > 9 && txtNumbers.FontSize > 26.75)
                {
                    _chatUtil.ModifiedTextSize = txtNumbers.FontSize = txtNumbers.FontSize - 2.75;
                }
                else if (_chatUtil.DialedNumbers.Length <= 9)
                {
                    if (txtNumbers.FontSize != 35)
                    {
                        txtNumbers.FontSize = 35;
                    }
                }
                _chatUtil.DialedNumbers = _chatUtil.DialedNumbers + number.Content.ToString();
                _typednumber.Append(number.Content.ToString());
            }
            Call.Focus();
        }

        /// <summary>
        /// Handles the MouseLeftClick event of the PhoneBookTabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PhoneBookTabItem_MouseLeftClick(object sender, MouseButtonEventArgs e)
        {
            ChkAgentLevel.IsChecked = false;
            ChkApplicationLevel.IsChecked = false;
            ChkGroupLevel.IsChecked = false;
            txtNumbers.Text = string.Empty;
            //txtNumbersBook.Text = string.Empty;
            _typednumber.Clear();
            //_chatDataContext.DiallingNumber = string.Empty;
            //chatUtil.DialedNumbers = string.Empty;
            ////txtSearch.Text = string.Empty;
        }

        private void PhoneBook_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Microsoft.Windows.Controls.DataGridCell)
            {
                Microsoft.Windows.Controls.DataGridCell tempcell = sender as Microsoft.Windows.Controls.DataGridCell;
                Microsoft.Windows.Controls.DataGridRow row = null;
                var parent = VisualTreeHelper.GetParent(tempcell);
                while (parent != null && parent.GetType() != typeof(Microsoft.Windows.Controls.DataGridRow))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is Microsoft.Windows.Controls.DataGridRow)
                    {
                        row = parent as Microsoft.Windows.Controls.DataGridRow;
                        break;
                    }
                }
                if (row != null)
                {
                    Pointel.Interactions.Chat.Helpers.Contacts selectedContact = (Pointel.Interactions.Chat.Helpers.Contacts)row.Item;
                    dgvContact.SelectedItem = (Pointel.Interactions.Chat.Helpers.Contacts)row.Item;
                    _phoneBookText = selectedContact.Number;
                    _phoneBookMenu.PlacementTarget = row;
                    _phoneBookMenu.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Processes the configuration data.
        /// </summary>
        /// <param name="configurationData">The configuration data.</param>
        private void ProcessConfigurationData(KeyValueCollection configurationData)
        {
            try
            {
                if (configurationData.ContainsKey("speed-dial.contacts"))
                {
                    if (configurationData["speed-dial.contacts"] != null)
                        ReadSpeedDialContacts(configurationData["speed-dial.contacts"] as KeyValueCollection);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Reads the speed dial contacts.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        private void ReadSpeedDialContacts(KeyValueCollection keyValue)
        {
            try
            {
                if (keyValue != null && keyValue.Count > 0)
                {
                    foreach (string key in keyValue.AllKeys)
                    {
                        if (!_applcationContact.ContainsKey(key))
                            _applcationContact.Add(key, keyValue[key].ToString());
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveNumber_Click(object sender, RoutedEventArgs e)
        {
            if (_chatUtil.DialedNumbers.Length != 0)
            {
                if (_typednumber.Length > 0)
                    _typednumber.Length -= 1;
                _chatUtil.DialedNumbers = _chatUtil.DialedNumbers.Remove(_chatUtil.DialedNumbers.Length - 1, 1);
            }
            if (_chatUtil.DialedNumbers.Length < _chatDataContext.MaxDialDigits && txtNumbers.FontSize < 35)
            {
                _chatUtil.ModifiedTextSize = txtNumbers.FontSize = txtNumbers.FontSize + 2.75;
            }
            if (ChatDataContext.messageToClientChat != null)
                ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, "Back");
        }

        private void SelectionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (System.Windows.Controls.TabItem item in (this.SelectionTab as System.Windows.Controls.TabControl).Items)
            {
                if (item == (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabDial")
                {
                    item.Foreground = (Brush)(new BrushConverter().ConvertFromString("#0071C6"));
                }
                else if (item != (this.SelectionTab as System.Windows.Controls.TabControl).SelectedItem && item.Name == "tabDial")
                {
                    item.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Single control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Single_Click(object sender, RoutedEventArgs e)
        {
            //_chatDataContext.Singleclick = true;
            //_chatDataContext.Dualclick = false;
        }

        private void Somewhere()
        {
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the TabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (chatUtil.DialedNumbers.Length < _chatDataContext.MaxDialDigits)
            //{
            if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D3)
            {
                btnh.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.D8)
            {
                btns.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            else if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                btn0.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                btn1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                btn2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                btn3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                btn4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                btn5.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                btn6.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D7 || e.Key == Key.NumPad7)
                btn7.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D8 || e.Key == Key.NumPad8)
                btn8.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.D9 || e.Key == Key.NumPad9)
                btn9.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if (e.Key == Key.Multiply)
                btns.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //}
            if (e.Key == Key.Back)
                btnClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            else if ((e.Key == Key.Enter || e.Key == Key.Space) && txtNumbers.Text != string.Empty && !Call.IsFocused)
                Call.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private string TeamCommunicatorEventNotify(Dictionary<string, string> dictionaryValues)
        {
            return string.Empty;
        }

        //Below event added for implement contact search
        //SMoorthy - 07-01-2014
        private void txtContactSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtSearch = sender as TextBox;
            SearchText = txtSearch.Text.ToString();
            Thread searchContacts = null;
            try
            {
                object searchString = (object)SearchText;
                searchContacts = new Thread(new ParameterizedThreadStart(ContactSearch));
                searchContacts.Start(searchString);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while searching contacts at key-up " + generalException.ToString());
            }
            finally
            {
                searchContact = null;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtNumbersBook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtNumbersBook_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //if (txtNumbersBook.Text.Length <= _chatDataContext.MaxDialDigits && eventFireBackNum != null)
                //{
                //    eventFireBackNum.Invoke(txtNumbersBook.Text);
                //}
            }
            catch { }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtNumbers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtNumbers_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //if (_chatDataContext.UserSetConfType != ChatDataContext.ConsultType.None || _chatDataContext.UserSetTransType != ChatDataContext.ConsultType.None || iswindowTypeNormal)
                //{
                //if (txtNumbers.Text.Length <= _chatDataContext.MaxDialDigits && eventFireBackNum != null)
                //{
                //    eventFireBackNum.Invoke(txtNumbers.Text);
                //}
                //}
                if (ChatDataContext.messageToClientChat != null)
                    ChatDataContext.messageToClientChat.PluginDialEvents(Interactions.IPlugins.PluginType.Chat, txtNumbers.Text);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtNumbers.Focus();
                _chatDataContext.MaxDialDigits = _chatDataContext.DialpadDigits;
                //if (_chatDataContext.isOnCall)
                //{
                //    _chatDataContext.MaxDialDigits = _chatDataContext.ConsultDialDigits;
                //}
                //else
                //{
                //    _chatDataContext.MaxDialDigits = _chatDataContext.DialpadDigits;
                //}
                //Configurations Library
                //iConfigListener configListener = this;
                //ConfigManager configSubscribe = new ConfigManager();
                //configSubscribe.Subscribe(configListener);

                //configSubscribe.ReadCallTypeContacts();
                //if (_chatDataContext.isOnCall)
                //{
                //    cmbCallType.Items.Clear();
                //    cmbCallType.Items.Add("--------------CallType--------------");
                //    for (int i = 0; i < _chatDataContext.hshCallType.Count; i++)
                //    {
                //        cmbCallType.Items.Add(_chatDataContext.hshCallType[i].ToString());
                //    }
                //    cmbCallType.SelectedIndex = 0;
                //    cmbCallType.Height = 20;
                //    cmbCallType.Visibility = System.Windows.Visibility.Visible;
                //}
                //else

                //Code commented by smoorthy hide the calltype
                //{
                //    cmbCallType.Height = 0;
                //    cmbCallType.Visibility = System.Windows.Visibility.Hidden;
                //}
                //end

                //dgvContact.Columns[0].Width = new Microsoft.Windows.Controls.DataGridLength(1, DataGridLengthUnitType.Star);
            }
            catch (Exception generalException)
            {
                logger.Error("DialPad:UsercontrolLoaded:" + generalException.ToString());
            }
        }

        //end
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _chatDataContext.cmshow.IsOpen = false;
            _chatDataContext.cmshow.StaysOpen = false;
            _chatDataContext.cmshow.Items.Clear();
        }

        #endregion Methods

        #region Other

        //end

        #endregion Other
    }
}