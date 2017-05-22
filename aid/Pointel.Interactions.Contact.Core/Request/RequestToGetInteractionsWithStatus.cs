using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToGetInteractionsWithStatus
    {
        #region RequestToGetInteractionsWithStatus
        /// <summary>
        /// Gets the interactions count.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public static OutputValues GetInteractionsCount(string mediaType)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetInteractionsWithStatus requestGetInteractionsWithStatus = new RequestGetInteractionsWithStatus();
                requestGetInteractionsWithStatus.MediaType = mediaType;
                requestGetInteractionsWithStatus.InteractionType = "Inbound";
                requestGetInteractionsWithStatus.InteractionSubtype = "InboundNew";
                requestGetInteractionsWithStatus.Status = new Genesyslab.Platform.Contacts.Protocols.ContactServer.NullableStatuses(Statuses.InProcess);
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------GetInteractionsCount---------");
                    logger.Info("MediaType    :" + mediaType);
                    logger.Info("-------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestGetInteractionsWithStatus);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Get Interaction Count Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Interaction Count Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetInteractionsCount() : Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Interaction Count request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;

        }
        #endregion
    }
}
