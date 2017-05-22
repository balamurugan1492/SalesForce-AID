using Genesyslab.Platform.Contacts.Protocols.ContactServer;

namespace Pointel.Interactions.Contact.Helpers
{
    public class HistoryCriteria
    {
        public string AttributeName;
        public string AttributeValue;
        public string AttributeSubValues;
        public bool IsDate;
        public Operators ComparisonOperator;
        public Prefixes Prefixes;
    }
}
