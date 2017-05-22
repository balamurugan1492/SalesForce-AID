using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToUnmergeContact
    {
        public static OutputValues UNMergeContact(string contactId)
        {
            OutputValues result = new OutputValues();
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                RequestUnMergeContacts requestUnMergeContacts = new RequestUnMergeContacts();
                //Attribute to Mere contact. 
                requestUnMergeContacts.ContactId = contactId;
                result.IContactMessage = Settings.UCSProtocol.Request(requestUnMergeContacts);
                if (result.IContactMessage != null && result.IContactMessage.Id == EventUnMergeContacts.MessageId)
                {
                    result.MessageCode = "200";
                    result.Message = "MergeContact Successful.";
                }
                else
                {
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
