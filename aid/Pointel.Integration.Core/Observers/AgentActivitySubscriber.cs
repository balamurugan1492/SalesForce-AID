using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Integration.Core.Agent_Activities;
using Pointel.Integration.Core.iSubjects;
using Pointel.Integration.Core.Providers;
using Pointel.Integration.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pointel.Integration.Core.Observers
{
    class AgentActivitySubscriber : IObserver<iCallData>
    {
        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
      "AID");
        private Settings setting = Settings.GetInstance();
        private AgentActivities agentActivities = new AgentActivities();
        bool isAlreadyLogin = false;
        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void Subscribe(CallDataProviders provider)
        {
            cancellation = provider.Subscribe(this);

        }
        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>

        #region Unsubscribe
        public virtual void Unsubscribe()
        {
            //  cancellation.Dispose();
        }

        #endregion Unsubscribe

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(iCallData value)
        {

            //if (!(ConfigContainer.Instance().AllKeys.Contains("pipe.event.data-type")
            //                && ConfigContainer.Instance().GetAsString("pipe.event.data-type").Equals(value.VoiceEvent.Name)))
            //    return;
            if (value == null || value.EventMessage == null
                || !(value.EventMessage.Name == EventAgentLogin.MessageName
                || value.EventMessage.Name == EventAgentNotReady.MessageName
                || value.EventMessage.Name == EventAgentReady.MessageName
                || value.EventMessage.Name == EventAgentLogout.MessageName))
            {
                return;
            }
            Thread agentActivityThread = new Thread(delegate()
            {
                try
                {
                    switch (value.EventMessage.Id)
                    {
                        case EventAgentLogin.MessageId:
                            EventAgentLogin eventAgentLogin = (EventAgentLogin)value.EventMessage;
                            logger.Info("Agent Activity EventAgentLogin Entry");
                            if (!isAlreadyLogin)
                            {
                                isAlreadyLogin = true;
                                agentActivities.InsertAgentLoginActivity();                               
                            }
                            logger.Info("Agent Activity EventAgentLogin Exit");
                            break;

                        case EventAgentNotReady.MessageId:
                            EventAgentNotReady eventAgentNReady = (EventAgentNotReady)value.EventMessage;
                            logger.Info("Agent Activity EventAgentNotReady Entry");
                            if (!isAlreadyLogin)
                            {
                                isAlreadyLogin = true;
                                agentActivities.InsertAgentLoginActivity();                              
                            }

                            // Get reason from Event
                            string reason = string.Empty;
                            if (eventAgentNReady.Reasons != null && eventAgentNReady.Reasons.Count > 0
                                && eventAgentNReady.Reasons.AllKeys.Contains(ConfigContainer.Instance().GetAsString("not-ready.key-name")))
                            {
                                reason = eventAgentNReady.Reasons[ConfigContainer.Instance().GetAsString("not-ready.key-name")].ToString();
                            }
                            // Insert into DB
                            agentActivities.InsertAgentNotReadyActivity(reason);
                            logger.Info("Agent Activity EventAgentNotReady Exit");
                            break;

                        case EventAgentReady.MessageId:
                            logger.Info("Agent Activity EventAgentReady Entry");
                            EventAgentReady eventAgentReady = (EventAgentReady)value.EventMessage;
                            if (!isAlreadyLogin)
                            {
                                agentActivities.InsertAgentLoginActivity();
                                isAlreadyLogin = true;
                            }
                            agentActivities.UpdateAgentReadyActivity();
                            logger.Info("Agent Activity EventAgentReady Exit");
                            break;

                        case EventAgentLogout.MessageId:
                            logger.Info("Agent Activity EventAgentLogout Entry");
                            EventAgentLogout eventAgentLogout = (EventAgentLogout)value.EventMessage;
                            agentActivities.InsertAgentLogoutActivity();
                            isAlreadyLogin = false;
                            logger.Info("Agent Activity EventAgentLogout Exit");
                            break;

                    }

                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while insert/update the agent activity" + generalException.ToString());
                }

            });
            agentActivityThread.Start();
        }
    }
}
