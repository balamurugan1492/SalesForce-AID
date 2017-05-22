using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToDeleteInteraction
    {
        
        #region RequestToDeleteInteraction
        /// <summary>
        /// Deletes the interaction from ucs.
        /// </summary>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <returns></returns>
        public static OutputValues DeleteInteractionFromUCS(string interactionID)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestDeleteInteraction requestDeleteInteraction = new RequestDeleteInteraction();
                requestDeleteInteraction.InteractionId = interactionID;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------DeleteInteractionFromUCS------------");
                    logger.Info("InteractionId    :" + interactionID);
                    logger.Info("--------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestDeleteInteraction);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Delete Interaction from UCS Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Delete Interaction from UCS UnSuccessful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("DeleteInteractionFromUCS() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Delete Interaction from UCS" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion RequestToDeleteInteraction
    }
}
