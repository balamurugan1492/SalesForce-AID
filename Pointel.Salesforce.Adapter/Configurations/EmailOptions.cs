namespace Pointel.Salesforce.Adapter.Configurations
{
    using System;
    using System.ComponentModel;

    internal class EmailOptions : CommonOptions
    {
        public bool CanCreateProfileActivityforInbNoRecord { get; set; }

        public bool CanCreateProfileActivityforOutNoRecord { get; set; }

        public bool InbCanCreateMultimatchActivity { get; set; }

        public bool InbCanCreateNorecordActivity { get; set; }

        public bool InbCanPopupMultimatchActivity { get; set; }

        public bool InbCanPopupNorecordActivity { get; set; }

        public string InbMultiMatchRecordAction { get; set; }

        public string InbNoMatchRecordAction { get; set; }

        public bool OutboundCanCreateLog { get; set; }

        public bool OutboundCanUpdateLog { get; set; }

        public bool OutboundFailureCanCreateLog { get; set; }

        public string OutboundFailurePopupEvent { get; set; }

        public string OutboundPopupEvent { get; set; }

        public string OutboundSearchAttributeKeys { get; set; }

        public string OutboundSearchUserDataKeys { get; set; }

        public string OutboundUpdateEvent { get; set; }

        public bool OutCanCreateMultimatchActivity { get; set; }

        public bool OutCanCreateNorecordActivity { get; set; }

        public bool OutCanPopupMultimatchActivity { get; set; }
        public bool OutCanPopupNorecordActivity { get; set; }

        public string OutMultiMatchRecordAction { get; set; }

        public string OutNoMatchRecordAction { get; set; }

        public override string ToString()
        {
            string str = string.Empty;
            try
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
                {
                    string name = descriptor.Name;
                    object obj2 = descriptor.GetValue(this);
                    str = str + string.Format("{0} = {1}\n", name, obj2);
                }
            }
            catch (Exception)
            {
                return "";
            }
            return str;
        }
    }
}