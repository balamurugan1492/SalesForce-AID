using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;
namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToInsertContact
    {
        #region Request Delete of Contact
        public static OutputValues RequestInsertContact(int tenantId,AttributesList attribute)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();            
            try
            {
                RequestInsert reqInsertContact = new RequestInsert();
                reqInsertContact.TenantId = tenantId;
                reqInsertContact.Attributes = attribute;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestInsertContact-------------");
                    logger.Info("Tenant ID  : " + tenantId);
                    logger.Info("---------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(reqInsertContact);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Contact Inserted Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Can't Insert Contact Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("RequestDeleteContact() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Insert Contact " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }

            return output;
        }

        #endregion

    }
}
