using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Contact.Core.Common;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestContactSearch
    {
        public static OutputValues GetSearchResult(string query, int maxCount, string searchType)
        {
            OutputValues output = OutputValues.GetInstance();
            RequestSearch requestContactSearch = new RequestSearch();
            requestContactSearch.Query = query;
            requestContactSearch.MaxResults = maxCount;
            requestContactSearch.IndexName = searchType;
            if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
            {
                output.IContactMessage = Settings.UCSProtocol.Request(requestContactSearch);
                if (output.IContactMessage.Id == EventSearch.MessageId)
                {
                    output.MessageCode = "200";
                    output.Message = "Request processed successfully.";
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "Got Error during perform contact search.";
                }
            }
            else
            {
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = "Universal Contact Server protocol is Null or Closed.";
            }
            return output;
        }
    }
}
