using Pointel.Interactions.Contact.Settings;

namespace Pointel.Interactions.Contact.Helpers
{
    public class Attribute
    {
        private bool _isPrimary = false;
        bool firstTimeUpdated = false;
        public string AttributeId
        {
            set;
            get;

        }
        public ContactDataContext.AttributeType Type
        {
            set;
            get;
        }
        public string AttributeName
        {
            set;
            get;
        }
        public string AttributeValue
        {
            set;
            get;
        }
        public string Description
        {
            set;
            get;
        }
        public bool Isprimary
        {
            get
            {
                return _isPrimary;
            }
            set
            {
                _isPrimary=value;
                if (firstTimeUpdated)
                {
                    IsAltered = true;
                }
                else
                {
                    firstTimeUpdated = true;
                }
               
            }
        }
        public bool IsAltered
        {
            get;
            set;
        }       
        public bool IsMandatory
        {
            get;
            set;
        }
        
    }
}
