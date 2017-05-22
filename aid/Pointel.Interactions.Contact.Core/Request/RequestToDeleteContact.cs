using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;
namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToDeleteContact
    {

         #region Request Delete of Contact
            public static OutputValues RequestDeleteContact(string contactId)
            {
                Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
                OutputValues output = OutputValues.GetInstance();
                RequestDelete reqDeleteContact = new RequestDelete();
                try
                {
                    reqDeleteContact.ContactId = contactId;
                    if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                    {
                        logger.Info("------------RequestDeleteContact-------------");
                        logger.Info("ContactId  : " + contactId);
                        logger.Info("---------------------------------------------------");
                        IMessage message = Settings.UCSProtocol.Request(reqDeleteContact);
                        if (message != null)
                        {
                            logger.Trace(message.ToString());
                            output.IContactMessage = message;
                            output.MessageCode = "200";
                            output.Message = "Contact Deleted Successfully";
                        }
                        else
                        {
                            output.IContactMessage = null;
                            output.MessageCode = "2001";
                            output.Message = "Can't Delete Contact Successfully";
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
                    logger.Error("Error occurred while Delete Contact " + generalException.ToString());
                    output.MessageCode = "2001";
                    output.Message = generalException.Message;
                }

                return output;
            }

            #endregion

     }

}
