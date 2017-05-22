using System.Collections.Generic;
using System.Xml;

namespace Pointel.Salesforce.Adapter.Utility
{
    public class SFDCTaskData
    {
        public string InteractionId { get; set; }
        public List<XmlElement> TaskFields { get; set; }
        public string ObjectName { get; set; }
        public SFDCCallType CallType { get; set; }
        public string RecordId { get; set; }
        public int RetryAttempt { get; set; }
        public TaskAction Action { get; set; }
    }

    public enum TaskAction
    {
        None,
        TaskCreate,
        TaskUpdate,
        TaskAppend,
        RecordCreate,
        RecordUpdate
    }
}