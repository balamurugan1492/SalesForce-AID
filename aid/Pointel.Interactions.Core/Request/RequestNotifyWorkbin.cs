
using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;
namespace Pointel.Interactions.Core.Request
{
    internal class RequestNotifyWorkbin
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        public OutputValues NotifyWorbin(int proxyClientId, string workbinType, string worbinEmpId, string workbinGroupId = null, string WorkbinPlaceId = null, string workbinPlaceGroupId = null)
        {
            OutputValues outPutValues = new OutputValues();
            outPutValues.Message = string.Empty;
            outPutValues.MessageCode = string.Empty;
            outPutValues.ErrorCode = 0;
            KeyValueCollection keyVC = new KeyValueCollection();
            keyVC.Add("event_processing_stopped", 1);
            try
            {
                RequestWorkbinNotifications reqWorkbinNotifications = RequestWorkbinNotifications.Create();
                reqWorkbinNotifications.Workbin = WorkbinInfo.Create(workbinType);
                reqWorkbinNotifications.Workbin.WorkbinAgentId = worbinEmpId;
                if (!string.IsNullOrEmpty(workbinGroupId))
                    reqWorkbinNotifications.Workbin.WorkbinGroupId = workbinGroupId;
                if (!string.IsNullOrEmpty(workbinPlaceGroupId))
                    reqWorkbinNotifications.Workbin.WorkbinPlaceGroupId = workbinPlaceGroupId;
                if (!string.IsNullOrEmpty(WorkbinPlaceId))
                    reqWorkbinNotifications.Workbin.WorkbinPlaceId = WorkbinPlaceId;
                reqWorkbinNotifications.Reason = ReasonInfo.Create();
                reqWorkbinNotifications.ProxyClientId = proxyClientId;
                reqWorkbinNotifications.NotifyPropertyChanges = 1;
                reqWorkbinNotifications.Extension = keyVC;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.InteractionProtocol.Request(reqWorkbinNotifications);

                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case EventAck.MessageId:
                                EventAck _requestWorkbinNotifications = (EventAck)message;
                                logger.Info("------------Request Workbin Notifications-------------");
                                logger.Trace(_requestWorkbinNotifications.ToString());
                                outPutValues.MessageCode = "200";
                                logger.Trace(_requestWorkbinNotifications.ProxyClientId);
                                outPutValues.Message = "Request Workbin Notifications Successful";
                                break;

                            case EventError.MessageId:
                                EventError eventError = (EventError)message;
                                logger.Trace(eventError.ToString());
                                outPutValues.MessageCode = "2001";
                                outPutValues.Message = "NotifyWorbin() : " + Convert.ToString(eventError.ErrorDescription);
                                break;
                        }
                    }
                }
            }
            catch(Exception error)
            {
                logger.Error(error.Message.ToString());
                outPutValues.MessageCode = "2001";
                outPutValues.Message = "NotifyWorbin() : " + error.Message.ToString();
            }

            return outPutValues;
        }
    }
}
