namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;

    internal class RequestHoldInteraction
    {
        #region Methods

        public static Pointel.Interactions.Chat.Core.General.OutputValues HoldInteraction(int proxyID, string sessionID, KeyValueCollection extensions)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestHold requestHold = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestHold.Create();

                requestHold.InteractionId = sessionID;
                requestHold.ProxyClientId = proxyID;
                if (extensions != null && extensions.Count > 0)
                {
                    extensions.Add("IsHold", "1");
                    requestHold.Extension = extensions;
                }
                //if(!extensions.ContainsKey(""))
                //{

                //}
                //requestHold.Extension = extensions;
                requestHold.Reason = ReasonInfo.Create("AwaitingInfo", "Ennava");
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestHold);
                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                logger.Info("------------RequestHold-------------");
                                logger.Info("SessionID  :" + sessionID);
                                logger.Info("ProxyID    :" + proxyID);
                                logger.Info("------------------------------------");
                                logger.Info(requestHold.ToString());
                                output.MessageCode = "200";
                                output.Message = "Interaction Hold Successful.";
                                break;

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "HoldInteraction() : " + LoginErrorDescription;
                                break;
                        }
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Chat Media Hold Interaction UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("HoldInteraction() : IXN Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Hold Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        #endregion Methods
    }
}