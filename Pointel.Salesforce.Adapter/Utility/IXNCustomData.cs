using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using System;

namespace Pointel.Salesforce.Adapter.Utility
{
    public class IXNCustomData
    {
        public string EventName { get; set; }

        public string InteractionId { get; set; }

        public IMessage InteractionEvent { get; set; }

        public InteractionProperties OpenMediaInteraction { get; set; }

        public KeyValueCollection UserData { get; set; }

        public SFDCCallType InteractionType { get; set; }

        public InteractionAttributes IXN_Attributes { get; set; }

        public Tuple<string, string> DispositionCode { get; set; }

        public BaseEntityAttributes EntityAttributes { get; set; }

        public AttachmentList AttachmentLists { get; set; }

        public InteractionContent InteractionContents { get; set; }

        public string Duration { get; set; }

        public DateTime StartDate { get; set; }

        public string InteractionNotes { get; set; }

        public MediaType MediaType { get; set; }
    }
}