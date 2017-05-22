/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar and Moorthy
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Core;
using Pointel.Interactions.Email.DataContext;
using Pointel.Interactions.Email.Helper;
using Pointel.Interactions.IPlugins;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Pointel.Interactions.Email.Forms
{
    /// <summary>
    /// Interaction logic for EmailNotifier.xaml
    /// </summary>
    public partial class EmailNotifier : TaskbarNotifier.TaskbarNotifier, TaskbarNotifier.INotifier
    {
        #region Variable Declarations
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        public string contactName = string.Empty;
        Thread emailAutoAnswerThread = null;

        // Added for ringing bell functionality
        private MediaPlayer mediaPlayer;
        private bool isPlayTone = false;
        private DispatcherTimer timerforstopplayingtone;

        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        // public static bool needPerformAccept = true;
        #endregion

        private EmailDetails emailDetails = new EmailDetails();


        private string FirstName
        {
            get;
            set;
        }

        private string LastName
        {
            get;
            set;
        }

        public EventInvite eventInvite = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotifier"/> class.
        /// </summary>
        public EmailNotifier()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            this.DataContext = emailDetails;
            mediaPlayer = new System.Windows.Media.MediaPlayer();
            mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;
        }

        ~EmailNotifier()
        {
            mediaPlayer = null;
        }



        public void Notify()
        {
            btnAccept_Click(null, null);
        }

        /// <summary>
        /// Binds the information.
        /// </summary>
        /// <param name="eventRevoked">The event revoked.</param>
        public void BindInfo(EventRevoked eventRevoked)
        {
            HideWidow();
        }

        /// <summary>
        /// Hides the widow.
        /// </summary>
        private void HideWidow()
        {
            EmailDataContext.GetInstance().isNotifyShowing = false;
            objiNotifier = null;
            StopTone();
            base.stayOpenTimer.Stop();
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
            this.Visibility = System.Windows.Visibility.Hidden;
            this.ForceHidden();
        }

        /// <summary>
        /// Handles the <see cref="E:AccessKeyPressed" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AccessKeyPressedEventArgs"/> instance containing the event data.</param>
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && (e.Target == null))
            {
                //if alt key is not in use handle event to prevent behavior without alt key
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Binds the information.
        /// </summary>
        /// <param name="eventInvite">The event invite.</param>
        public void BindInfo(EventInvite eventInvite)
        {
            this.eventInvite = eventInvite;
            ReadContactName();
            KeyValueCollection keyValue = eventInvite.Interaction.InteractionUserData;
            if (emailDetails.EmailCaseData != null)
                emailDetails.EmailCaseData.Clear();
            if (eventInvite.Interaction.InteractionUserData != null)
                foreach (string key in eventInvite.Interaction.InteractionUserData.AllKeys)
                {
                    emailDetails.EmailCaseData.Add(new EmailCaseData() { Key = key, Value = eventInvite.Interaction.InteractionUserData[key].ToString() });
                }

        }

        /// <summary>
        /// Reads the name of the contact.
        /// </summary>
        public void ReadContactName()
        {
            try
            {
                string contactId = eventInvite.Interaction.InteractionUserData.Contains("ContactId") ?
                eventInvite.Interaction.InteractionUserData["ContactId"].ToString() : string.Empty;
                if (!string.IsNullOrEmpty(contactId) && EmailDataContext.GetInstance().IsContactServerActive)
                {
                    List<string> attributeList = new List<string>();
                    attributeList.Add("FirstName");
                    attributeList.Add("LastName");

                    EventGetAttributes eventGetAttribute = ContactServerHelper.RequestGetContactAttribute(contactId, attributeList);
                    if (eventGetAttribute != null)
                    {
                        List<AttributesHeader> attributeHeader = eventGetAttribute.Attributes.Cast<AttributesHeader>().ToList();
                        int count = attributeHeader.Count;
                        contactName = string.Empty;
                        if (attributeHeader.Where(x => x.AttrName.Equals("FirstName")).ToList().Count > 0)
                        {
                            AttributesHeader firstNameHeader = attributeHeader.Where(x => x.AttrName.Equals("FirstName")).SingleOrDefault();
                            if (firstNameHeader != null && firstNameHeader.AttributesInfoList.Count > 0)
                            {
                                contactName += FirstName = firstNameHeader.AttributesInfoList[0].AttrValue.ToString() + " ";
                            }
                        }
                        if (attributeHeader.Where(x => x.AttrName.Equals("LastName")).ToList().Count > 0)
                        {
                            AttributesHeader LastNameHeader = attributeHeader.Where(x => x.AttrName.Equals("LastName")).SingleOrDefault();
                            if (LastNameHeader != null && LastNameHeader.AttributesInfoList.Count > 0)
                            {
                                contactName += LastName = LastNameHeader.AttributesInfoList[0].AttrValue.ToString();
                            }
                        }

                    }
                }
                if (string.IsNullOrEmpty(contactName))
                {

                    contactName = getContactName(eventInvite.Interaction.InteractionUserData);
                    if (string.IsNullOrEmpty(contactName))
                        contactName = "Undefined";
                }
                emailDetails.TitleText = contactName + " - Agent Interaction Desktop";
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns>System.String.</returns>
        private string getContactName(KeyValueCollection keyValue)
        {
            if (keyValue != null)
            {
                string name = string.Empty;
                if (keyValue.ContainsKey("FirstName"))
                {
                    name += FirstName = keyValue["FirstName"].ToString();
                }
                if (keyValue.ContainsKey("LastName"))
                {
                    LastName = keyValue["LastName"].ToString();
                    name += (string.IsNullOrEmpty(name) ? "" : " ") + LastName;
                }
                return name;
            }
            return null;
        }

        /// <summary>
        /// Handles the Expanded event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top - 52;
        }

        /// <summary>
        /// Handles the Collapsed event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top + 98;
        }

        private bool isOnHandling;

        /// <summary>
        /// Handles the Click event of the btnAccept control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (isOnHandling) return;
            isOnHandling = true;
            HideWidow();
            DoEmailAccept();
            isOnHandling = false;
        }

        /// <summary>
        /// Does the email accept.
        /// </summary>
        public void DoEmailAccept()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    //if (!EmailNotifier.needPerformAccept)
                    //    return;
                    //EmailNotifier.needPerformAccept = false;
                    InteractionService interactionService = new InteractionService();
                    Pointel.Interactions.Core.Common.OutputValues acceptInteraction = interactionService.AcceptInteraction(eventInvite.TicketId, eventInvite.Interaction.InteractionId, eventInvite.ProxyClientId);
                    if (acceptInteraction.MessageCode == "200")
                    {
                        EmailMainWindow emailMainWindow = new EmailMainWindow(eventInvite, FirstName, LastName);
                        emailMainWindow.Show();
                        if (!ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer") ||
                            (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer")
                            && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer")).ToLower().Equals("false")))
                            HideWidow();
                    }
                }));

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.ToString());
            }
        }



        /// <summary>
        /// Handles the Click event of the btnReject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (isOnHandling) return;
            isOnHandling = true;
            try
            {
                //EmailNotifier.needPerformAccept = false;
                HideWidow();
                if (emailAutoAnswerThread != null)
                {
                    emailAutoAnswerThread.Abort();
                }
                InteractionService interactionService = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = interactionService.RejectInteraction(eventInvite.TicketId, eventInvite.Interaction.InteractionId, eventInvite.ProxyClientId, eventInvite.Interaction.InteractionUserData);
                if (result.MessageCode == "200")
                {
                    if (EmailDataContext.GetInstance().MessageToClientEmail != null)
                    {
                        EmailDataContext.GetInstance().MessageToClientEmail.PluginInteractionStatus(PluginType.Workbin, IXNState.Closed, false);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as +" + ex.ToString());
            }
            isOnHandling = false;
        }

        /// <summary>
        /// Handles the Loaded event of the TaskbarNotifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TaskbarNotifier_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            //It is the functionality to check Auto aunwswer. if it is enabled perform work related to it.
            if (eventInvite != null)
            {
                //  EmailNotifier.needPerformAccept = true;
                this.Focus();
                this.btnAccept.Focus();
                //if (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer") && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer")).ToLower().Equals("true"))
                //{
                //    DoEmailAccept();
                //}
                //if (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer") && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer")).ToLower().Equals("true"))
                //{
                //    emailAutoAnswerThread = new Thread(new ThreadStart(AcceptEmail));
                //    emailAutoAnswerThread.Start();
                //    if (!ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer.reject") && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer.reject")).ToLower().Equals("true"))
                //    {
                //        btnReject.Visibility = Visibility.Collapsed;
                //        btnAccept.Margin = new Thickness(0, 5, 5, 5);
                //    }
                //}
            }
        }

        /// <summary>
        /// Accepts the email.
        /// </summary>
        private void AcceptEmail()
        {
            Thread.Sleep((ConfigContainer.Instance().AllKeys.Contains("email.auto-answer.timer") ? int.Parse((string)ConfigContainer.Instance().GetValue("email.auto-answer.timer")) : 0) * 1000);
            DoEmailAccept();
        }

        /// <summary>
        /// Handles the KeyUp event of the TaskbarNotifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TaskbarNotifier_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                btnReject_Click(null, null);
        }

        public void PlayTone()
        {
            logger.Info("PlayTone Method Entry");
            if (ConfigContainer.Instance().AllKeys.Contains("email.ringing-bell") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("email.ringing-bell")))
            {
                try
                {
                    // Assign path to mediaplayer
                    string path = System.IO.Path.GetFullPath((((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[0]));
                    mediaPlayer.Open(new Uri(path));

                    // Set Volume to mediaplayer in double valid values from 0.0 to 1.0
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[1]))
                    {
                        double volume;
                        double.TryParse(((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[1], out volume);
                        if (volume > 0)
                            mediaPlayer.Volume = volume;
                        else
                            mediaPlayer.Volume = 1.0;
                    }
                    else
                        mediaPlayer.Volume = 1.0;


                    // Set duration -1 means plays and repeats until an notifier closes, 0 means play the whole sound one time and  > 0 means a time, in milliseconds, to play and repeat the sound.
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[2]))
                    {
                        int secondsforPlaying;
                        int.TryParse(((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[2], out secondsforPlaying);
                        if (secondsforPlaying > 0)
                        {
                            // Timer to stop the mediaplayer in defined seconds example 10 seconds
                            timerforstopplayingtone = new DispatcherTimer();
                            timerforstopplayingtone.Interval = TimeSpan.FromSeconds(secondsforPlaying);
                            timerforstopplayingtone.Tick += timerforstopplayingtone_Tick;
                            timerforstopplayingtone.Start();
                            // plays the audio 
                            mediaPlayer.Play();
                        }
                        else
                            mediaPlayer.Play();
                    }
                    else
                        mediaPlayer.Play();

                    // Assign true for stopping mediaplayer when notifier closes
                    isPlayTone = true;
                }
                catch (Exception ex)
                {
                    isPlayTone = false;
                    logger.Error("Error occurred while opening url for email ringing bell " + ex.Message);
                }
            }
            else
                isPlayTone = false;
            logger.Info("PlayTone Method Exit");
        }

        void timerforstopplayingtone_Tick(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in timerforstopplayingtone_Tick stopping " + ex.Message);
            }
        }

        public void StopTone()
        {
            try
            {
                if (isPlayTone)
                {
                    mediaPlayer.Stop();
                    if (timerforstopplayingtone != null)
                    {
                        timerforstopplayingtone.Stop();
                        timerforstopplayingtone.Tick -= timerforstopplayingtone_Tick;
                        timerforstopplayingtone = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player stopping " + ex.Message);
            }
        }

        void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Stop();

                //Check the duration is -1, then repeat the audio.
                if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[2]) && (((string)ConfigContainer.Instance().GetValue("email.ringing-bell")).Split('|')[2]) == "-1")
                    mediaPlayer.Play();
                else
                    isPlayTone = false;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player ended event " + ex.Message);
            }
        }

        private void TaskbarNotifier_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void TaskbarNotifier_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }
    }
}
