#region Header

/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on :
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

#endregion Header

namespace Pointel.Interactions.Email.Listener
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Email.DataContext;
    using Pointel.Interactions.Email.Forms;
    using Pointel.Interactions.IPlugins;

    #region Delegates

    internal delegate void ContactServerNotificationHandler();

    internal delegate void VoiceStatusNotification(bool isEnabled);
    #endregion Delegates

    /// <summary>
    /// Class EmailInteractionListener.
    /// </summary>
    class EmailInteractionListener : IEmailPlugin
    {
        #region Fields

        public static IPluginCallBack messageToClientEmail = null;

        public EmailNotifier _taskbarNotifier = null;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
         "AID");

        #endregion Fields

        #region Events

        internal static event ContactServerNotificationHandler ContactServerNotificationHandler;
        internal static event VoiceStatusNotification VoiceStatusNotification;

        #endregion Events

        #region Methods

        public void CheckAutoAnswer(EventInvite eventInvite)
        {
            try
            {
                // Check if it is Autoanswer or not
                if (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer")
                           && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer")).ToLower().Equals("true"))
                {
                    //Get auto answer timer value from CME and show notifier when timer > 0
                    int seconds = 0;
                    int.TryParse(ConfigContainer.Instance().AllKeys.Contains("email.auto-answer.timer") ?
                        ((string)ConfigContainer.Instance().GetValue("email.auto-answer.timer")) : "0", out seconds);

                    if (seconds > 0)
                    {
                        // Show or Hide reject button when email.auto-answer is set to true and the value of email.auto-answer.timer > 0
                        if (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer-reject") && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer-reject")).ToLower().Equals("true"))
                            _taskbarNotifier.btnReject.Visibility = Visibility.Visible;
                        else
                            _taskbarNotifier.btnReject.Visibility = Visibility.Collapsed;

                        _taskbarNotifier.objiNotifier = (TaskbarNotifier.INotifier)_taskbarNotifier;
                        _taskbarNotifier.isAutoAnswerTimer = true;
                        //Converting seconds to milliseconds
                        _taskbarNotifier.StayOpenMilliseconds = seconds * 1000;
                        _taskbarNotifier.OpeningMilliseconds = 1000;
                        _taskbarNotifier.HidingMilliseconds = 500;

                        ShowEmailNotifier(eventInvite);
                    }
                    else
                    {
                        _taskbarNotifier.objiNotifier = null;
                        _taskbarNotifier.isAutoAnswerTimer = false;
                        _taskbarNotifier.StayOpenMilliseconds = 1000000000;
                        _taskbarNotifier.OpeningMilliseconds = 1000;
                        _taskbarNotifier.HidingMilliseconds = 500;
                        _taskbarNotifier.btnReject.Visibility = Visibility.Visible;
                        //Auto Answer
                        _taskbarNotifier.eventInvite = eventInvite;
                        _taskbarNotifier.ReadContactName();
                        _taskbarNotifier.DoEmailAccept();

                    }
                }
                else
                {
                    _taskbarNotifier.StayOpenMilliseconds = 1000000000;
                    _taskbarNotifier.OpeningMilliseconds = 1000;
                    _taskbarNotifier.HidingMilliseconds = 500;
                    _taskbarNotifier.btnReject.Visibility = Visibility.Visible;
                    ShowEmailNotifier(eventInvite);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in CheckAutoAnswer " + ex.Message);
            }
        }

        /// <summary>
        /// Initializes the email.
        /// </summary>
        /// <param name="confObject">The conf object.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="mediaPlugins">The media plugins.</param>
        /// <param name="agentInfo">The agent information.</param>
        public void InitializeEmail(Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService confObject, IPluginCallBack listener,
            Dictionary<string, string> agentInfo)
        {
            LoadNotifier();
            if (agentInfo.Keys.Contains("UserName"))
                EmailDataContext.GetInstance().Username = agentInfo["UserName"];
            if (agentInfo.Keys.Contains("Place"))
                EmailDataContext.GetInstance().PlaceName = agentInfo["Place"]; ;
            if (agentInfo.Keys.Contains("EmployeeID"))
                EmailDataContext.GetInstance().AgentID = agentInfo["EmployeeID"];
            if (agentInfo.Keys.Contains("ApplicationName"))
                EmailDataContext.GetInstance().ApplicationName = agentInfo["ApplicationName"];
            if (agentInfo.Keys.Contains("TenantDBID"))
                EmailDataContext.GetInstance().TenantDbId = Convert.ToInt32(agentInfo["TenantDBID"]);
            if (agentInfo.Keys.Contains("ProxyClientID"))
                EmailDataContext.GetInstance().ProxyClientID = Convert.ToInt32(agentInfo["ProxyClientID"]);

            EmailDataContext.GetInstance().MessageToClientEmail = listener;
            EmailDataContext.GetInstance().ConfigurationServer = confObject;

            //Implemented the thread call to read configuaration details.
            //Thread configurationThread = new Thread(new ThreadStart(ApplicationUtil.ApplicationReader.ReadConfigurationData));
            //configurationThread.Start();

            //Normal method call to read configuaration details.
            // ApplicationUtil.ApplicationReader.ReadConfigurationData();
            // GetFromAddressfromPopClient();

            //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state) { _chatListener.Chat_Session(output2.RequestJoinIMessage, interactionId, _taskbarNotifier); }), null);

            var thread = new Thread(() =>
            {
                Pointel.Interactions.Email.Helper.EmailServerDetails.GetFromAddress();
            });
            thread.Start();
        }

        /// <summary>
        /// Determines whether [is email opened] [the specified interaction identifier].
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <returns><c>true</c> if [is email opened] [the specified interaction identifier]; otherwise, <c>false</c>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsEmailOpened(string interactionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the notifier.
        /// </summary>
        public void LoadNotifier()
        {
            try
            {
                _taskbarNotifier = new EmailNotifier();
                //_taskbarNotifier.StayOpenMilliseconds = 5000;
                //_taskbarNotifier.OpeningMilliseconds = 500;
                //_taskbarNotifier.HidingMilliseconds = 1000;
                _taskbarNotifier.Show();
                _taskbarNotifier.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
                _taskbarNotifier.ForceHidden();
            }
            catch (Exception exception)
            {
                logger.Error("Error in LoadNotifier" + exception.Message);
            }
        }

        /// <summary>
        /// Notifies the contact protocol.
        /// </summary>
        /// <param name="ucsProtocol">The ucs protocol.</param>
        public void NotifyContactProtocol(Genesyslab.Platform.Contacts.Protocols.UniversalContactServerProtocol ucsProtocol)
        {
        }

        /// <summary>
        /// Notifies the state of the contact protocol.
        /// </summary>
        /// <param name="isLoggedIn">if set to <c>true</c> [is logged in].</param>
        public void NotifyContactProtocolState(bool isLoggedIn = true)
        {
        }

        /// <summary>
        /// Notifies the state of the contact server.
        /// </summary>
        /// <param name="isOpen">if set to <c>true</c> [is open].</param>
        public void NotifyContactServerState(bool isOpen = false)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    EmailDataContext.GetInstance().IsContactServerActive = isOpen;
                    if (EmailInteractionListener.ContactServerNotificationHandler != null)
                        EmailInteractionListener.ContactServerNotificationHandler.Invoke();
                }), DispatcherPriority.ContextIdle, new object[0]);
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                NotifyContactServerState(isOpen);
                logger.Error("Error in NotifyContactServerState :" + ex.Message);
            }
        }

        /// <summary>
        /// Notifies the email interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        public void NotifyEmailInteraction(Genesyslab.Platform.Commons.Protocols.IMessage message)
        {
            try
            {
                switch (message.Id)
                {
                    case EventInvite.MessageId:

                        // Newly Added
                        // Start
                        CheckAutoAnswer(message as EventInvite);
                        // Stop

                        //Old Code
                        // Start
                        //  EventInvite eventInvite = message as EventInvite;
                        //if (ConfigContainer.Instance().AllKeys.Contains("email.enable.auto-answer")
                        //    && ((string)ConfigContainer.Instance().GetValue("email.enable.auto-answer")).ToLower().Equals("true"))
                        //{
                        //    //EmailNotifier.needPerformAccept = true;
                        //    _taskbarNotifier.eventInvite = eventInvite;
                        //    _taskbarNotifier.ReadContactName();
                        //    _taskbarNotifier.DoEmailAccept();
                        //}
                        //else
                        //    ShowEmailNotifier(eventInvite);
                        // Stop

                        break;

                    case EventRevoked.MessageId:
                        EventRevoked eventRevoked = message as EventRevoked;
                        if (_taskbarNotifier != null)
                        {
                            _taskbarNotifier.BindInfo(eventRevoked);
                        }

                        break;

                    case EventAck.MessageId:
                        EventAck eventAck = message as EventAck;

                        break;
                    case EventPulledInteractions.MessageId:
                        EventPulledInteractions puledIteraction = message as EventPulledInteractions;
                        if (puledIteraction.Interactions != null && puledIteraction.Interactions.Count > 0)
                        {
                            string[] keys = puledIteraction.Interactions.AllKeys;
                            var _interactionID = keys[0];
                            if (_interactionID != null)
                            {
                                EmailMainWindow mailWindow = new EmailMainWindow(puledIteraction);
                                mailWindow.Show();
                            }
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                logger.Error("NotifyEmailInteraction" + exception.ToString());
            }
        }

        /// <summary>
        /// Intimates the selected agent and consult type
        /// </summary>
        /// <param name="isLoggedIn">if set to <c>true</c> [is logged in].</param>
        public void NotifyEmailLoginState(bool isLoggedIn = true)
        {
        }

        // Reply All - True
        /// <summary>
        /// Notifies the email reply.
        /// </summary>
        /// <param name="parentIxnId">The parent ixn identifier.</param>
        /// <param name="isReplyAll">if set to <c>true</c> [is reply all].</param>
        public void NotifyEmailReply(string parentIxnId, bool isReplyAll = false)
        {
            EmailMainWindow emailMainWindow = new EmailMainWindow(parentIxnId, isReplyAll);
            emailMainWindow.Show();
        }

        /// <summary>
        /// Notifies the interaction protocol.
        /// </summary>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        public void NotifyInteractionProtocol(Genesyslab.Platform.OpenMedia.Protocols.InteractionServerProtocol ixnProtocol)
        {
        }

        /// <summary>
        /// Notifies the ixn status.
        /// </summary>
        /// <param name="isConnected">if set to <c>true</c> [is connected].</param>
        /// <param name="proxyClientID">The proxy client identifier.</param>
        public void NotifyIXNStatus(bool isConnected, int? proxyClientID = null)
        {
            if (proxyClientID != null)
                EmailDataContext.GetInstance().ProxyClientID = (int)proxyClientID;
        }

        /// <summary>
        /// Notifies the new email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="contactID">The contact identifier.</param>
        public void NotifyNewEmail(string emailAddress, string contactID, string outboundIXNID = null)
        {
            EmailMainWindow emailMainWindow = new EmailMainWindow(emailAddress, contactID, outboundIXNID);
            emailMainWindow.Show();
        }

        public void NotifyPlace(string place)
        {
            EmailDataContext.GetInstance().PlaceName = place;
        }

        public void NotifyVoiceMediaStatus(bool isVoiceEnabled)
        {
            EmailDataContext.GetInstance().IsVoiceMediaEnabled = isVoiceEnabled;
            if (EmailInteractionListener.VoiceStatusNotification != null)
                EmailInteractionListener.VoiceStatusNotification.Invoke(isVoiceEnabled);
        }

        /// <summary>
        /// Shows the Contact directory address.
        /// </summary>
        /// <param name="to">The automatic.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        public void ShowContactDirectoryAddress(string to, string cc, string bcc)
        {
        }

        /// <summary>
        /// Shows the email notifier.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ShowEmailNotifier(EventInvite message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                try
                {
                    _taskbarNotifier.PlayTone();
                    _taskbarNotifier.BindInfo(message);
                    _taskbarNotifier.SetInitialLocations(false);
                    _taskbarNotifier.Notify(0, false);
                    EmailDataContext.GetInstance().isNotifyShowing = true;
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred as " + ex.Message);
                }
            }));
        }

        #endregion Methods

        #region Other

        //public void InitializeEmail(string username, string placeName, string agentId, Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService confObject, int tenantDbId, string applicationName, Dictionary<WINPosition, double> windowPostioning, IPluginCallBack listener, System.Collections.Hashtable mediaPlugins)
        //{
        //    LoadNotifier();
        //    EmailDataContext.GetInstance().Username = username;
        //    EmailDataContext.GetInstance().PlaceName = placeName;
        //    EmailDataContext.GetInstance().AgentID = agentId;
        //    EmailDataContext.GetInstance().ApplicationName = applicationName;
        //    EmailDataContext.GetInstance().TenantDbId = tenantDbId;
        //    EmailDataContext.GetInstance().HtPlugin = mediaPlugins;
        //    EmailDataContext.GetInstance().MessageToClientEmail = listener;
        //    EmailDataContext.GetInstance().ConfigurationServer = confObject;
        //    //Implemented the thread call to read configuaration details.
        //    //Thread configurationThread = new Thread(new ThreadStart(ApplicationUtil.ApplicationReader.ReadConfigurationData));
        //    //configurationThread.Start();
        //    //Normal method call to read configuaration details.
        //    ApplicationUtil.ApplicationReader.ReadConfigurationData();
        //}

        #endregion Other
    }
}