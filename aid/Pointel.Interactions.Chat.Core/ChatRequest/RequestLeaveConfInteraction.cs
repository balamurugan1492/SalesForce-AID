using System;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestLeaveConfInteraction
    {

        #region RequestLeaveConferenceInteraction

        /// <summary>
        /// Leaves the interaction from conference.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="proxyID">The proxy identifier.</param>
        /// <param name="agentID">The agent identifier.</param>
        /// <param name="_chatClosingProtocol">The _chat closing protocol.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues LeaveInteractionFromConference(string interactionID, int proxyID, string agentID)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestLeaveInteraction requestLeaveInteraction = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestLeaveInteraction.Create();
                requestLeaveInteraction.InteractionId = interactionID;
                requestLeaveInteraction.ProxyClientId = proxyID;
                requestLeaveInteraction.Reason = null;
                //ReasonInfo reasonInfo = ReasonInfo.Create();
                //reasonInfo.ReasonDescription = agentID.ToString() + " Leaving from the Conference";
                //requestLeaveInteraction.Reason = reasonInfo;
                requestLeaveInteraction.Extension = null;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage response = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestLeaveInteraction);
                    if (response != null)
                    {
                        switch (response.Id)
                        {
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                logger.Info("------------LeaveInteractionFromConference-------------");
                                logger.Info("InteractionId      :" + interactionID);
                                logger.Info("ProxyClientId      :" + proxyID);
                                logger.Info("AgentId            :" + agentID);
                                logger.Info("-------------------------------------------------------");
                                logger.Trace(response.ToString());
                                output.MessageCode = "200";
                                output.Message = "Leave Interaction From Conference Successful";
                                break;
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                logger.Trace(response.ToString());
                                output.MessageCode = "2001";
                                output.Message = "Don't Leave Interaction From Conference Successful";
                                break;
                        }
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Don't Leave Interaction From Conference Successful";
                    }
                }
                else
                {
                    logger.Warn("LeaveInteractionFromConference() : Interaction Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Leave Interaction From Conference request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
