using System;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetAllAttributes
    {
        #region RequestGetAllAttributes
        /// <summary>
        /// Gets the attribute values.
        /// </summary>
        /// <param name="contactId">The contact identifier.</param>
        /// <param name="strList">The string list.</param>
        /// <returns></returns>
        public static OutputValues GetAttributeValues(string contactId, List<string> attributeNames) 
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetAttributes requestGetAttributes = new RequestGetAttributes();
                StringList stringList = new StringList();
                for (int count = 0; count < attributeNames.Count; count++)
                {
                    stringList.Add(attributeNames[count].ToString());
                }
                //stringList.Add("PhoneNumber");
                //stringList.Add("EmailAddress");
                requestGetAttributes.AttributeList = stringList;
                requestGetAttributes.ContactId = contactId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {

                    logger.Info("------------GetAttributeValues-------------");
                    logger.Info("ContactId  :" + contactId);
                    logger.Info("AttributeList    :" + stringList);
                    logger.Info("-------------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestGetAttributes);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Get All Attributes Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get All Attributes Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetAttributeValues() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Attribute Values request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
