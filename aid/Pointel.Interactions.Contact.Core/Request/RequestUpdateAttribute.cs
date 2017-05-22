using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{

    #region RequestInsertUpdateDeleteAttribute
    /// <summary>
    /// This request is intended for updating contact attributes. New attributes can be added or existing attributes can be updated or removed.
    /// </summary>
    public class RequestUpdateAttribute
    {
        #region Request Update All type of attribute 
        public static OutputValues RequestUpdateAllAttribute(string contactId, int tenantId, AttributesList insertedAttributes, AttributesList updatedAttributes, DeleteAttributesList deletedAttributes)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            RequestUpdateAttributes reqUpdateAttributes = new RequestUpdateAttributes();
            
            StringList stringList = new StringList();
            try
            {
                reqUpdateAttributes.ContactId = contactId;
                reqUpdateAttributes.TenantId = tenantId;
                if(insertedAttributes != null)
                    reqUpdateAttributes.InsertAttributes = insertedAttributes;
                if (updatedAttributes != null)
                    reqUpdateAttributes.UpdateAttributes = updatedAttributes;
                if (deletedAttributes != null)
                reqUpdateAttributes.DeleteAttributes = deletedAttributes;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestUpdateAllAttribute-------------");
                    logger.Info("TenantId  :" + tenantId);
                    logger.Info("ContactId  : " + contactId);
                    logger.Info("---------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(reqUpdateAttributes);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Updated Attributes Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Update Attribute Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("RequestDeleteAttribute() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Request To Update Attribute " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            
            return output;
        }

        #endregion

    }
    #endregion RequestInsertUpdateDeleteAttribute
}
