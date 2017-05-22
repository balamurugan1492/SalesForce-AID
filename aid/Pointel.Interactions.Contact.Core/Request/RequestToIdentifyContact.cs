using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToIdentifyContact
    {
        #region RequestToIdentifyContact
        public static OutputValues IdentifyContact(string mediaType, int tenantId, KeyValueCollection userData)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestIdentifyContact requestIdentifyContact = RequestIdentifyContact.Create();
                requestIdentifyContact.TenantId = tenantId;
                requestIdentifyContact.MediaType = mediaType;
                requestIdentifyContact.CreateContact = true;
                requestIdentifyContact.OtherFields = userData;
                logger.Info("------------RequestIdentifyContact-------------");
                logger.Info(requestIdentifyContact.ToString());
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.UCSProtocol.Request(requestIdentifyContact);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Identify Contact Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Identify Contact Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("RequestToIdentifyContact() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Request To Identify Contact " + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion RequestToIdentifyContact
    }
}
