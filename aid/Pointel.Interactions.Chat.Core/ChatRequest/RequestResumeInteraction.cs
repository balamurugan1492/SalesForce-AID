using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestResumeInteraction
    {
        public static Pointel.Interactions.Chat.Core.General.OutputValues ResumeInteraction(int proxyID, string sessionID, KeyValueCollection extensions, ReasonInfo reason)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestResume requestResume = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestResume.Create();
                requestResume.InteractionId = sessionID;
                requestResume.ProxyClientId = proxyID;
                requestResume.Extension = extensions;
                requestResume.Reason = reason;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage message = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestResume);
                    if (message != null)
                    {
                        switch (message.Id)
                        {
                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId:
                                logger.Info("------------RequestResume-------------");
                                logger.Info("SessionID  :" + sessionID);
                                logger.Info("ProxyID    :" + proxyID);
                                logger.Info("--------------------------------------");
                                logger.Info(requestResume.ToString());
                                output.MessageCode = "200";
                                output.Message = "Interaction Resume Successful.";
                                break;

                            case Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError.MessageId:
                                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError eventError = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventError)message;
                                string LoginErrorCode = Convert.ToString(eventError.ErrorCode);
                                string LoginErrorDescription = Convert.ToString(eventError.ErrorDescription);
                                logger.Trace(eventError.ToString());
                                output.MessageCode = "2001";
                                output.Message = "ResumeInteraction() : " + LoginErrorDescription;
                                break;
                        }
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Chat Media Resume Interaction UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("ResumeInteraction() : IXN Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Resume Interaction request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
    }
}
