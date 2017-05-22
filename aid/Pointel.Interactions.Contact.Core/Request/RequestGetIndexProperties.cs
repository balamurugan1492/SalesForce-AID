using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;
using Genesyslab.Platform.Commons.Protocols;

using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class GetIndexProperties
    {
        public static OutputValues GetResult()
        {
            OutputValues result = new OutputValues();
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                RequestGetIndexProperties requestGetIndexProperties = new RequestGetIndexProperties();
                IMessage response = Settings.UCSProtocol.Request(requestGetIndexProperties);
                if (response != null && response.Id==EventGetIndexProperties.MessageId)
                {
                    result.IContactMessage = response;
                    result.MessageCode = "200";
                    result.Message = "Get Index properties Successful.";
                }
                else
                {
                    result.IContactMessage = null;
                    result.MessageCode = "2001";
                    result.Message = "Error Occurred while GetIndexProperties.";
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
