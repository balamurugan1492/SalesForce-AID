using System;
using System.IO;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestAddAttachDocument
    {
        #region RequestAddAttachDocument

        public static OutputValues AddAttachDocumentbyDocID(string interactionId, string documentId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestAddDocument requestAddDocument = new RequestAddDocument();
                requestAddDocument.InteractionId = interactionId;
                requestAddDocument.DocumentId = documentId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------AddAttachDocument by DocID------------");
                    logger.Info("InteractionId    :" + interactionId);
                    logger.Info("DocumentId    :" + documentId);
                    logger.Info("----------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestAddDocument);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Add Attach Document Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Add Attach Document UnSuccessful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("AddAttachDocumentbyDocID() : Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Add Attach Document by DocumentID request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Adds the attach document.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static OutputValues AddAttachDocument(string interactionId, string filePath)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                string fileName = string.Empty;
                RequestAddDocument requestDocument = new RequestAddDocument();
                requestDocument.InteractionId = interactionId;

                requestDocument.Content = GetContent(filePath.ToString());
                requestDocument.MimeType = GetMimeType(filePath.ToString());
                fileName = Path.GetFileName(filePath.ToString());
                int index = fileName.IndexOf('~');
                if (index > 0)
                {
                    fileName = fileName.Substring(0, index);
                    requestDocument.TheName = fileName;
                }
                else
                {
                    requestDocument.TheName = fileName;
                }
                var size = GetSize(filePath);
                requestDocument.TheSize = size;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestAddDocument-------------");
                    logger.Info("InteractionId  :" + interactionId);
                    logger.Info("FilePath    :" + filePath);
                    logger.Info("TheSize :" + size == null ? "null" : size.ToString());
                    logger.Info("-------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestDocument);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;                        
                        output.MessageCode = "200";
                        output.Message = "Add Attach Document Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Could not be Add Attach Document Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Add Attach Document request " + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns></returns>
        private static Int32 GetSize(string fileUrl)
        {
            var fileinfo = new FileInfo(fileUrl);
            return (Int32)fileinfo.Length;
        }


        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns></returns>
        private static byte[] GetContent(string fileUrl)
        {
            FileStream fs = File.OpenRead(fileUrl.ToString());
            try
            {
                byte[] allbytes = new byte[fs.Length];
                fs.Read(allbytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return allbytes;
            }
            finally
            {
                fs.Close();
            }            
        }


        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns></returns>
        private static string GetMimeType(string fileUrl)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileUrl).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        #endregion
    }
}
