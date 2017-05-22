using System.Collections.Generic;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestContactLists
    {
        public static OutputValues GetContactList(int tenantDbId, SearchCriteriaCollection SearchCriteria, StringList attributeList, int pageMaxSize )
        {
            OutputValues result = new OutputValues();
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                RequestContactListGet requestContactListGet = new RequestContactListGet();
                //  requestContactListGet.AttributeList = attributeList;
                requestContactListGet.TenantId = tenantDbId;
                requestContactListGet.ContactCount = true;
                if (pageMaxSize > 0)
                    requestContactListGet.PageMaxSize = pageMaxSize;
                else
                    requestContactListGet.PageMaxSize = 100;
                requestContactListGet.SearchCriteria = SearchCriteria;
                IMessage response = Settings.UCSProtocol.Request(requestContactListGet);
                if (response != null && response.Id == EventContactListGet.MessageId)
                {
                    result.IContactMessage = response;
                    result.MessageCode = "200";
                    result.Message = "Get ContactList Successful.";
                }
                else
                {
                    result.IContactMessage = null;
                    result.MessageCode = "2001";
                    result.Message = "Error Occurred while ContactListGet.";
                }
            }
            else
            {
                result.IContactMessage = null;
                result.MessageCode = "2001";
                result.Message = "Contact server is not active.";
            }
            return result;
        }
    }
}

