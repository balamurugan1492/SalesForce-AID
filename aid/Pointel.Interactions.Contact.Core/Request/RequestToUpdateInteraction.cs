using System;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToUpdateInteraction
    {
        //#region Field Declaration
        //private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        //InteractionAttributes ixnAttributes = null;
        //#endregion


        #region RequestToUpdateInteraction
        public static OutputValues UpdateInteraction(string interactionID, int ownerId, string comment, KeyValueCollection userData, int status, string dtEndDate)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestUpdateInteraction requestUpdateInteraction = Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests.RequestUpdateInteraction.Create();
                InteractionAttributes ixnAttributes = new InteractionAttributes();
                ixnAttributes.Id = interactionID;
                if (!string.IsNullOrEmpty(comment))
                    ixnAttributes.TheComment = comment;
                if (ownerId != 0)
                    ixnAttributes.OwnerId = ownerId;
                ixnAttributes.TenantId = Settings.tenantDBID;
                if (!string.IsNullOrEmpty(dtEndDate))
                    ixnAttributes.EndDate = dtEndDate;
                requestUpdateInteraction.InteractionAttributes = ixnAttributes;
                if (userData != null)
                    requestUpdateInteraction.InteractionAttributes.AllAttributes = userData;
                requestUpdateInteraction.InteractionAttributes.Status = status.Equals(3) ? Statuses.Stopped : Statuses.InProcess;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestToUpdateInteraction-------------");
                    logger.Info("Id  :" + interactionID);
                    logger.Info("TheComment  : " + comment);
                    logger.Info("---------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestUpdateInteraction);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Update Interaction Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Update Interaction Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("UpdateInteraction() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Request To Update Interaction " + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion


        public static IMessage RequestUpdateInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            IMessage iMessage = null;
            RequestUpdateInteraction updateInteraction = new RequestUpdateInteraction();
            if (interactionContent != null)
            {
                updateInteraction.InteractionContent = interactionContent;
            }
            if (interactionAttributes != null)
            {
                updateInteraction.InteractionAttributes = interactionAttributes;
            }
            if (baseEntityAttributes != null)
            {
                updateInteraction.EntityAttributes = baseEntityAttributes;
            }
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                logger.Info("------------RequestUpdateInteraction-------------");
                logger.Info("Interaction Attributes : " + (updateInteraction.InteractionAttributes != null ? "null" : updateInteraction.InteractionAttributes.ToString()));
                logger.Info("Entity Attributes : " + (updateInteraction.InteractionAttributes != null ? "null" : updateInteraction.EntityAttributes.ToString()));
                logger.Info("---------------------------------------------------");
                iMessage = Settings.UCSProtocol.Request(updateInteraction);
                updateInteraction = null;
                logger.Trace("Response : " + (iMessage != null ? "null" : iMessage.ToString()));
            }
            else
            {
                logger.Warn("Error occurred as : Universal Contact Server protocol is Null or Closed");
            }
            return iMessage;
        }
    }
}
