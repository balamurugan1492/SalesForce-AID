using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToGetDocument
    {
        #region RequestToGetDocument
        /// <summary>
        /// Gets the attach document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        public static OutputValues GetAttachDocument(string documentId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetDocument requestGetDocument = new RequestGetDocument();
                
                requestGetDocument.DocumentId = documentId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------GetAttachDocument---------");
                    logger.Info("DocumentId    :" + documentId);
                    logger.Info("----------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestGetDocument);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Get Attach Document Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Attach Document Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetAttachDocument() : Universal Contact Server Protocol is Null");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Attach Document request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;

        }
        #endregion
    }
}
