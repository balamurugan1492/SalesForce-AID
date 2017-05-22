using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToRemoveDocument
    {  
        #region RequestToRemoveDocument
        /// <summary>
        /// Removes the attach document.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        public static OutputValues RemoveAttachDocument(string interactionId, string documentId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestRemoveDocument requestRemoveDocument = new RequestRemoveDocument();
                requestRemoveDocument.InteractionId = interactionId;
                requestRemoveDocument.DocumentId = documentId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------RemoveAttachDocument------------");
                    logger.Info("InteractionId    :" + interactionId);
                    logger.Info("DocumentId    :" + documentId);
                    logger.Info("----------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestRemoveDocument);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response; 
                        output.MessageCode = "200";
                        output.Message = "Remove Attach Document Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Remove Attach Document Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("RemoveAttachDocument() : Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Remove Attach Document request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion

    }
}
