using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToMergeContact
    {
        public static OutputValues MergeContact(int tenantId,int agentId,string sourceContactId,string destinationContactId,string description,string reason)
        {
            OutputValues result = new OutputValues();
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                RequestMergeContacts requestMergeContacts = new RequestMergeContacts();
                //Attribute to Mere contact.
                requestMergeContacts.TenantId = tenantId;
                requestMergeContacts.AgentId = agentId;
                requestMergeContacts.SourceContactId = sourceContactId;
                requestMergeContacts.DestinationContactId = destinationContactId;
                requestMergeContacts.Reason = reason;
                requestMergeContacts.Description = description;

                IMessage response = Settings.UCSProtocol.Request(requestMergeContacts);
                if (response != null && response.Id == EventMergeContacts.MessageId)
                {
                    result.IContactMessage = response;
                    result.MessageCode = "200";
                    result.Message = "MergeContact Successful.";
                }
                else
                {
                    result.IContactMessage = null;
                    result.MessageCode = "2001";
                    result.Message = "Error Occurred while MergeContact.";
                }
            }
            else
            {
                result.IContactMessage = null;
                result.MessageCode = "2001";
                result.Message = "Universal Contact Server protocol is Null or Closed";
            }
            return result;

        }
    }
}
