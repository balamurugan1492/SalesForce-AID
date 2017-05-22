using System;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetContactInteractionList
    {
        #region RequestGetContactInteractionList
        /// <summary>
        /// Gets the contacts.
        /// </summary>
        /// <param name="contactId">The contact identifier.</param>
        /// <param name="media">The media.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public static OutputValues GetContacts(SearchCriteriaCollection searchCriteriaCollection, string contactID, int tenantId, int pagemaxSize, List<string> attributesNames)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetInteractionsForContact requestGetContacts = RequestGetInteractionsForContact.Create();
                requestGetContacts.DataSource = new NullableDataSourceType(DataSourceType.Main);
                requestGetContacts.SortCriteria = new SortCriteriaCollection();
                SortCriteriaCollection sortCC = new SortCriteriaCollection();
                SortCriteria sortc = new SortCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, SortOperator = new NullableSortMode(SortMode.Descending), SortIndex = 0 };
                sortCC.Add(sortc);
                requestGetContacts.SortCriteria = sortCC;
                requestGetContacts.SearchCriteria = searchCriteriaCollection;
                StringList stringList = new StringList();
                if (attributesNames != null && attributesNames.Count > 0)
                    for (int index = 0; index < attributesNames.Count; index++)
                        stringList.Add(attributesNames[index]);
                requestGetContacts.AttributeList = stringList;
                requestGetContacts.TenantId = tenantId;
                requestGetContacts.ContactId = contactID;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("---------------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestGetContacts);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Get Interactions For Contact Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Interactions For Contact Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetContacts() : Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Interactions For Contact request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;

        }
        #endregion
    }
}
