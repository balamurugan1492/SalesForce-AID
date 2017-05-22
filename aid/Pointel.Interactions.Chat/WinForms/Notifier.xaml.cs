using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Pointel.Interactions.Chat.Core;
using Pointel.Interactions.Chat.Core.General;
using Pointel.Interactions.Chat.HandleInteractions;
using Pointel.Interactions.Chat.Helpers;
using Pointel.Interactions.Chat.Settings;
using Pointel.Interactions.IPlugins;
using System.Windows.Media;
using System.Windows.Threading;
using Pointel.Configuration.Manager;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Pointel.Interactions.Chat.WinForms
{
    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier : TaskbarNotifier.TaskbarNotifier, TaskbarNotifier.INotifier
    {
        #region Field Declarations
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        public event NotifChatAction ChatAction;
        public delegate void NotifChatAction(string action, string interactionId);
        private string _interactionId = string.Empty;

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

        #endregion Field Declarations

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifier"/> class.
        /// </summary>
        /// 
        public Notifier()
        {
            logger.Info("chat notify constructor");
            InitializeComponent();
            logger.Info("chat notify constructor -- Initialize Component");
            MessageIconImageSource.Source = new BitmapImage(new Uri("pack://application:,,,/Pointel.Interactions.Chat;component/Images/Chat.png"));
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            if (!_chatDataContext.isEnableChatReject && !_chatDataContext.isEnableChatAccept)
            {
                _chatDataContext.isEnableChatAccept = true;
                _chatDataContext.isEnableChatReject = false;
            }
            if (!_chatDataContext.isEnableChatReject && _chatDataContext.isEnableChatAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Accept";
                btnRight.Style = (Style)FindResource("CallButton");
            }
            if (!_chatDataContext.isEnableChatAccept && _chatDataContext.isEnableChatReject)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
            if (_chatDataContext.isEnableChatReject && _chatDataContext.isEnableChatAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Visible;
                btnLeft.Content = "_Accept";
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
            mediaPlayer = new System.Windows.Media.MediaPlayer();
            mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;
        }



        ~Notifier()
        {
            mediaPlayer = null;
        }


        #endregion Constructor

        #region Window Events

        /// <summary>
        /// Handles the Click event of the Right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                base.stayOpenTimer.Stop();
                base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
            }));
            try
            {
                Button tempbtn = sender as Button;
                if (tempbtn.Content.ToString().Contains("Reject"))
                {
                    ChatAction.Invoke("Reject", _interactionId);
                }
                else if (tempbtn.Content.ToString().Contains("Accept"))
                {
                    ChatAction.Invoke("Accept", _interactionId);
                }
                else if (tempbtn.Content.ToString().Contains("Cancel"))
                {
                    this.ForceHidden();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Right_Click() : " + generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the Left control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                base.stayOpenTimer.Stop();
                base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
            }));
            try
            {
                Button tempbtn = sender as Button;
                if (tempbtn.Content.ToString().Contains("Accept"))
                {
                    ChatAction.Invoke("Accept", _interactionId);
                }
                else if (tempbtn.Content.ToString().Contains("Reject"))
                {
                    ChatAction.Invoke("Reject", _interactionId);
                }
                else if (tempbtn.Content.ToString().Contains("Show"))
                {
                    var windows = Application.Current.Windows.OfType<Window>().Where(x => x.Name == "ChatWindow");
                    foreach (var win in windows)
                    {
                        if (win != null && win.Tag.ToString() == _interactionId)
                        {
                            if (win.WindowState == WindowState.Minimized)
                                win.WindowState = WindowState.Normal;
                            if (!win.IsActive)
                                win.Activate();
                            win.Topmost = true;
                            win.Topmost = false;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Left_Click() : " + generalException.ToString());
            }
        }

        #endregion Window Events

        #region Controls Events
        /// <summary>
        /// Called when [access key pressed].
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

        #endregion Controls Events

        #region Callable Function
        public void ReloadUI(bool isrejectCallDisplayed, string interactionId, bool isChatDelayNotification)
        {
            _interactionId = interactionId;
            if (!isrejectCallDisplayed)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Accept";
                btnRight.Style = (Style)FindResource("CallButton");
            }
            else
            {
                if (!_chatDataContext.isEnableChatReject && !_chatDataContext.isEnableChatAccept)
                {
                    _chatDataContext.isEnableChatAccept = true;
                    _chatDataContext.isEnableChatReject = false;
                }
                if (!_chatDataContext.isEnableChatReject && _chatDataContext.isEnableChatAccept)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Hidden;
                    btnRight.Content = "_Accept";
                    btnRight.Style = (Style)FindResource("CallButton");
                }
                if (!_chatDataContext.isEnableChatAccept && _chatDataContext.isEnableChatReject)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Hidden;
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
                if (_chatDataContext.isEnableChatReject && _chatDataContext.isEnableChatAccept)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Visible;
                    btnLeft.Content = "_Accept";
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
                else
                {
                    btnLeft.Visibility = System.Windows.Visibility.Visible;
                    btnLeft.Content = "_Accept";
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
            }
            if (!isChatDelayNotification)
            {
                ChatNotifier.Visibility = Visibility.Collapsed;
                CaseDataExpander.Visibility = Visibility.Visible;
            }
            else
            {
                ChatNotifier.Visibility = Visibility.Visible;
                CaseDataExpander.Visibility = Visibility.Collapsed;
                btnLeft.Visibility = System.Windows.Visibility.Visible;
                btnLeft.Content = "_Show";
                btnLeft.Style = (Style)FindResource("CallButton");
                btnRight.Content = "_Cancel";
                btnRight.Visibility = System.Windows.Visibility.Visible;
                btnRight.Style = (Style)FindResource("RejectButton");
            }
        }

        #endregion Callable Function

        #region Ringing Bell
        public void PlayTone()
        {
            logger.Info("PlayTone Method Entry");
            if (ConfigContainer.Instance().AllKeys.Contains("chat.ringing-bell") && !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")))
            {
                try
                {
                    // Assign path to mediaplayer
                    string path = System.IO.Path.GetFullPath((((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[0]));
                    // Assign path to mediaplayer
                    mediaPlayer.Open(new Uri(path));

                    // Set Volume to mediaplayer in double valid values from 0.0 to 1.0
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[1]))
                    {
                        double volume;
                        double.TryParse(((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[1], out volume);
                        if (volume > 0)
                            mediaPlayer.Volume = volume;
                        else
                            mediaPlayer.Volume = 1.0;
                    }
                    else
                        mediaPlayer.Volume = 1.0;


                    // Set duration -1 means plays and repeats until an notifier closes, 0 means play the whole sound one time and  > 0 means a time, in milliseconds, to play and repeat the sound.
                    if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[2]))
                    {
                        int secondsforPlaying;
                        int.TryParse(((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[2], out secondsforPlaying);
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
                    logger.Error("Error occurred while opening url for chat ringing bell " + ex.Message);
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
                if (!string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[2]) && (((string)ConfigContainer.Instance().GetValue("chat.ringing-bell")).Split('|')[2]) == "-1")
                    mediaPlayer.Play();
                else
                    isPlayTone = false;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in media player ended event " + ex.Message);
            }
        }
        #endregion


        public void Notify()
        {
            if (ChatAction != null)
                ChatAction.Invoke("Accept", _interactionId);
        }

        private void chatNotifier_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void chatNotifier_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
                e.Handled = true;
        }
    }
}
