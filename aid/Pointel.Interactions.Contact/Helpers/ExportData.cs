using System.Collections.Generic;
using System.Collections;

namespace Pointel.Interactions.Contact.Helpers
{
    public class ExportData
    {
        private Dictionary<string, string> data = null;

        public string InteractionID
        {
            get;
            set;
        }

        public bool IsDone
        {
            get;
            set;
        }

        public ExportData()
        {
            data = new Dictionary<string, string>();
        }

        ~ExportData()
        {
            data = null;
        }

        public string this[string keyName]
        {
            get
            {
                return data[keyName];
            }
            set
            {
                if (data.ContainsKey(keyName))
                    data[keyName] = value;
                else
                    data.Add(keyName, value);
            }
        }
        
        public List<string> Keys
        {
            get
            {
                return  new List<string>(data.Keys);
            }
        }

        public int Count
        {
            get
            {
               return data.Count;
            }
        }

    }
}
