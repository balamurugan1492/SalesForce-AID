/*
 * ======================================================
 * Pointel.Interaction.Workbin.WorkbinService
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Interaction.Workbin.Utility;
using Pointel.Interactions.Core;
using Pointel.Interactions.IPlugins;
using Pointel.Configuration.Manager;
using System.Windows.Threading;
using System.Windows;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using System.Threading;

namespace Pointel.Interaction.Workbin
{
    internal delegate void WorkbinChangedEvent(string agentId, string workbinName, string interactionId, WorkbinContentOperation operation);
    internal delegate void ContactServerNotificationHandler();
    internal delegate void InteractionServerLoginNotification();
    internal delegate void InteractionClosedNotification(string interactionId);

    public class WorkbinService : IWorkbinPlugin
    {
        #region Field Declaration

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private WorkbinUtility emailSettings = WorkbinUtility.Instance();
        internal static event WorkbinChangedEvent WorkbinChangedEvent;

        internal static event ContactServerNotificationHandler ContactServerNotificationHandler;

        internal static event InteractionServerLoginNotification InteractionServerLoginNotification;

        internal static event InteractionClosedNotification InteractionClosedEvent;

        #endregion Field Declaration

        /// <summary>
        /// Shows the work bin form.
        /// </summary>
        /// <returns></returns>
        public object ShowWorkBinForm()
        {
            Pointel.Interaction.Workbin.Controls.WorkbinUserControl usercontrolWorkbin = new Pointel.Interaction.Workbin.Controls.WorkbinUserControl();
            return usercontrolWorkbin;
        }


        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="getresponse">The received response.</param>
        public void GetResponse(string Name, string getresponse)
        {
            try
            {
                //  string OldText = emailDetails.emailReply.htmlEditor.ContentHtml;
                string html = getresponse; ;
                html = html.Replace("\r\n", "<br />");

                //emailDetails.emailReply.htmlEditor.ContentHtml = string.Empty;
                //emailDetails.emailReply.htmlEditor.ContentHtml = "\r\n" + html + "<br />" + "<br />" + OldText;
            }
            catch (Exception exception)
            {
                logger.Error(" GetResponse" + exception.ToString());
            }
        }


        /// <summary>
        /// Workbins the flag.
        /// </summary>
        /// <param name="isEnableWorkbin">if set to <c>true</c> [is enable workbin].</param>
        public void WorkbinFlag(bool isEnableWorkbin)
        {
            WorkbinUtility.Instance().IsWorkbinEnable = isEnableWorkbin;
        }

        /// <summary>
        /// Notifies the interaction protocol.
        /// </summary>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        public void NotifyInteractionProtocol(InteractionServerProtocol ixnProtocol)
        {
            //if (ixnProtocol == null || WorkbinUtility.Instance().IxnServerProtocol == ixnProtocol) return;
            //WorkbinUtility.Instance().IxnServerProtocol = ixnProtocol;
        }

        /// <summary>
        /// Shows the Contact directory address.
        /// </summary>
        /// <param name="to">The automatic.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        public void ShowContactDirectoryAddress(string to, string cc, string bcc)
        {
            try
            {

            }
            catch (Exception exception)
            {
                logger.Error("ContactDirectory_responseContactDirectoryNotify" + exception.ToString());
            }
            to = string.Empty;
            bcc = string.Empty;
            cc = string.Empty;
        }



        public void NotifyContactProtocol(UniversalContactServerProtocol ucsProtocol)
        {
            //if (ucsProtocol == null || WorkbinUtility.Instance().UcsProtocol == ucsProtocol) return;
            //WorkbinUtility.Instance().UcsProtocol = ucsProtocol;
        }



        #region IWorkbinPlugin Members


        public void InitializeWorkBin(string placeId, int proxyId, IPluginCallBack listener)
        {
            try
            {
                logger.Info("**********************************************************************************************************************");
                logger.Info("Pointel.Interaction.Workbin :" + Assembly.GetExecutingAssembly().GetName().Version);
                logger.Info("***********************************************************************************************************************");
                logger.Info("Retrieving Values from Application Email in AID Start");
                WorkbinUtility.Instance().PlaceID = placeId;
                WorkbinUtility.Instance().IxnProxyID = proxyId;
                WorkbinUtility.Instance().configListener = listener;
                WorkbinUtility.Instance().messageToClientEmail = listener;
                Pointel.Interaction.Workbin.ApplicationDetails.ApplicationDetails.LoadWorkbin();
                InteractionService.InteractionServerNotifier += InteractionService_InteractionServerNotifier;
            }
            catch (Exception exception)
            {
                logger.Error("StartEmailService" + exception.ToString());
            }

        }

        void InteractionService_InteractionServerNotifier(bool isOpen)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
            {
                ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).NotifyInteractionServerState(isOpen);
            }
        }

        #endregion


        public void NotifyWorkbinContentChanged(IMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(delegate
          {
              if (WorkbinService.WorkbinChangedEvent != null)
              {
                  EventWorkbinContentChanged eventWorkbinChanged = message as EventWorkbinContentChanged;
                  WorkbinService.WorkbinChangedEvent.Invoke(eventWorkbinChanged.Interaction["InteractionAgentId"].ToString(),
                      eventWorkbinChanged.Interaction.InteractionWorkbinTypeId, eventWorkbinChanged.Interaction.InteractionId, eventWorkbinChanged.WorkbinContentOperation);
              }
          }), DispatcherPriority.ContextIdle, new object[0]);
        }

        public void NotifyEmailLoginState(bool isLoggedIn = true)
        {

        }

        public void NotifyContactProtocolState(bool isLoggedIn = true)
        {

        }

        public List<string> GetPersonalWorkbinList()
        {
            return new List<string>(WorkbinUtility.Instance().PersonalWorkbinList.Keys);
        }

        public List<string> GetTeamAgentList()
        {
            return ApplicationDetails.ApplicationDetails.GetMyTeamAgentId();
        }

        public List<string> GetTeamWorkbinList()
        {
            return new List<string>(WorkbinUtility.Instance().TeamWorkbinList.Keys);
        }

        public void NotifyAgentLogin(bool isLogedin, int? proxyId = null)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(delegate
          {
              try
              {
                  if (proxyId != null)
                      WorkbinUtility.Instance().IxnProxyID = (int)proxyId;
                  //WorkbinUtility.Instance().IsAgentLoginIXN = isLogedin;
                  if (!isLogedin) return;
                  ApplicationDetails.ApplicationDetails.LoadWorkbin();
                  if (WorkbinService.InteractionServerLoginNotification != null)
                      WorkbinService.InteractionServerLoginNotification.Invoke();
              }
              catch (Exception ex)
              {
                  logger.Error("Error occurred as " + ex.Message);
              }

          }), DispatcherPriority.ContextIdle, new object[0]);

        }




        public bool NotifyEmailOpen(string interactionId)
        {
            try
            {
                KeyValueCollection kvpInteractionId = new KeyValueCollection();
                kvpInteractionId.Add("id", interactionId);
                InteractionService interactionServices = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = interactionServices.PullInteraction(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID, kvpInteractionId);
                if (result.MessageCode.Equals("200"))
                {
                    if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Email))
                    {
                        ((IEmailPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Email]).NotifyEmailInteraction(result.IMessage);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            return false;
        }


        public void NotifyContactServerState(bool isOpen = false)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(delegate
            {
                WorkbinUtility.Instance().IsContactServerActive = isOpen;
                if (WorkbinService.ContactServerNotificationHandler != null)
                    WorkbinService.ContactServerNotificationHandler.Invoke();
            }), DispatcherPriority.ContextIdle, new object[0]);
        }

        public void UpdateSelectedWorkbin()
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("CfgPerson"))
                {
                    CfgPerson person = ConfigContainer.Instance().GetValue("CfgPerson") as CfgPerson;
                    if (!person.UserProperties.ContainsKey("agent.ixn.desktop"))
                        person.UserProperties.Add("agent.ixn.desktop", new KeyValueCollection());
                    KeyValueCollection option = person.UserProperties["agent.ixn.desktop"] as KeyValueCollection;
                    if (!string.IsNullOrEmpty(WorkbinUtility.Instance().SelectedWorkbinName))
                    {
                        if (!option.ContainsKey("workbins.workbin-selected"))
                            option.Add("workbins.workbin-selected", WorkbinUtility.Instance().SelectedWorkbinName);
                        else
                            option["workbins.workbin-selected"] = WorkbinUtility.Instance().SelectedWorkbinName;
                    }                   
                    person.UserProperties["agent.ixn.desktop"] = option;
                    person.Save();
                    person = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error when updating selected workbin " + ex.ToString());
            }
        }


        public void NotifyEmailClose(string interactionId)
        {
            throw new NotImplementedException();
        }
    }
}