using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToGetInteractionContent
    {
       #region RequestToGetInteractionContent
        /// <summary>
        /// Gets the recent interaction list.
        /// </summary>
        /// <param name="universalContactServerProtocol">The universal contact server protocol.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <returns></returns>
        public static IMessage GetInteractionContent(string interactionID,bool isAttachments)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            IMessage message = null;
            try
            {
                RequestGetInteractionContent requestGetInteractionContent = RequestGetInteractionContent.Create();
                requestGetInteractionContent.InteractionId = interactionID;
                requestGetInteractionContent.IncludeAttachments = isAttachments;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestToGetInteractionContent-------------");
                    logger.Info("InteractionId  :" + interactionID);
                    logger.Info("-------------------------------------------------------");
                    message = Settings.UCSProtocol.Request(requestGetInteractionContent);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                    }
                    else
                    {
                        logger.Trace("null response from RequestGetInteractionContent");
                    }
                }
                else
                {
                    logger.Warn("Universal Contact Server protocol is Null or Closed");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while sending Request Get Interaction Content as : " + generalException.ToString());
            }
            return message;
        }
        #endregion RequestToGetInteractionContent
    }
}
