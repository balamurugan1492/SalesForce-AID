using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    public class AdSearch
    {
        public SearchFieldType SearchFields { get; set; }
        public string LookupObjects { get; set; }
        public string LookupFields { get; set; }
        public ADSearchDataType SearchDataType { get; set; }
        public bool EnableCustomQuery { get; set; }
        public string CustomQuery { get; set; }
        public int SearchDataLength { get; set; }
        public string SearchDelimiter { get; set; }
        public List<string> SKipSearchData { get; set; }
        public bool EnableFilterResults { get; set; }
        public string FilterResultFields { get; set; }
        public string[] SearchDataFormat { get; set; }

        public string ResponseFields { get; set; }
    }

    public enum ADSearchDataType
    {
        None,
        Numeric,
        AlphaNumeric
    }

    public enum SearchFieldType
    {
        All,
        Phone,
        Name,
        Email,
        Sidebar,
        Custom
    }
}