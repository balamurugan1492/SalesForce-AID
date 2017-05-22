
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToInsertInteraction
    {
        public static IMessage RequestInsertInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            IMessage iMessage = null;
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                logger.Info("------------RequestInsertInteraction-------------");
                RequestInsertInteraction insertInteraction = new RequestInsertInteraction();

                if (interactionContent != null)
                {
                    insertInteraction.InteractionContent = interactionContent;
                    logger.Info("Interaction Content : " + (insertInteraction.InteractionContent != null ? "null" : insertInteraction.InteractionContent.ToString()));
                }
                if (interactionAttributes != null)
                {
                    insertInteraction.InteractionAttributes = interactionAttributes;
                    logger.Info("Interaction Attributes : " + (insertInteraction.InteractionAttributes != null ? "null" : insertInteraction.InteractionAttributes.ToString()));
                }
                if (baseEntityAttributes != null)
                {
                    insertInteraction.EntityAttributes = baseEntityAttributes;
                    logger.Info("Entity Attributes : " + (insertInteraction.InteractionAttributes != null ? "null" : insertInteraction.EntityAttributes.ToString()));
                }
                logger.Info("---------------------------------------------------");
                iMessage = Settings.UCSProtocol.Request(insertInteraction);
                logger.Trace("Response : " +(iMessage != null ? "null" : iMessage.ToString()));
            }
            else
            {
                logger.Warn("Error occurred as : Universal Contact Server protocol is Null or Closed");
            }
            return iMessage;
        }
    }
}
