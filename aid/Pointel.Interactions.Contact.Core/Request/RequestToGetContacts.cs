using System;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToGetContacts
    {
        #region RequestToGetContacts
        /// <summary>
        /// Gets the contact list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public static OutputValues GetContactList(int tenantId, List<string> attributeNames,int startIndex)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetContacts requestGetContacts = new RequestGetContacts();
                requestGetContacts.IndexOfFirst = startIndex;
                StringList stringList = new StringList();
                for (int count = 0; count < attributeNames.Count; count++)
                {
                    stringList.Add(attributeNames[count].ToString());
                }
                //stringList.Add("EmailAddress");
                //stringList.Add("FirstName");
                //stringList.Add("LastName");
                requestGetContacts.AttributeList = stringList;
                requestGetContacts.TenantId = tenantId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------GetContacts-------------");
                    logger.Info("TenantId    :" + tenantId);
                    logger.Info("AttributeList: " + stringList);
                    logger.Info("-------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestGetContacts);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Get Contacts Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Contacts Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetContactList() : Universal Contact Server Protocol is Null");
                }

            }
            catch (Exception generalException)
            {

                logger.Error("Error Occurred while Get Contacts request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;

        }
        #endregion
    }
}
