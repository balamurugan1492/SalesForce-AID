using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestAgentStopInteraction
    {
        #region RequestStopInteraction
        /// <summary>
        /// Stops the interaction process.
        /// </summary>
        /// <param name="stopInteractionId">The stop interaction identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues StopInteractionProcess(string stopInteractionId, int proxyId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestStopProcessing requestStopProcessing = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestStopProcessing.Create();
                requestStopProcessing.InteractionId = stopInteractionId;
                requestStopProcessing.ProxyClientId = proxyId;
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.ReasonInfo reasonInfo = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.ReasonInfo.Create();
                reasonInfo.ReasonSystemName = "Stop Processing Reason Normal";
                reasonInfo.ReasonDescription = "Normal";
                requestStopProcessing.Reason = reasonInfo;
                logger.Info("Trying to Stop the interaction :" + stopInteractionId);
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Send(requestStopProcessing);
                    logger.Info("------------StopChatSession-------------");
                    logger.Info("InteractionId  :" + stopInteractionId);
                    logger.Info("ProxyClientId    :" + proxyId);
                    logger.Info("------------------------------------------------");
                    output.MessageCode = "200";
                    output.Message = "Stop Chat Session Successful";
                }
                else
                {
                    logger.Warn("StopInteractionProcess() : Interaction Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Stop Chat Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}