using System.Collections.Generic;

namespace Pointel.Interactions.DispositionCodes.Utilities
{
    public class DispositionData
    {
        private Dictionary<string, string> _dispostionCollection = new Dictionary<string, string>();
        public Dictionary<string,string> DispostionCollection
        {
            get { return _dispostionCollection; }
            set { _dispostionCollection = value; }
        }

        private string _dispostionCode = string.Empty;
        public string DispostionCode
        {
            get { return _dispostionCode; }
            set { _dispostionCode = value; }
        }
        private string _interactionID = string.Empty;
        public string InteractionID
        {
            get { return _interactionID; }
            set { _interactionID = value; }
        }
    }
}
