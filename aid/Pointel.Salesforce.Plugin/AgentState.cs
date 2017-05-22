using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Salesforce.Plugin
{
    public class AgentState
    {
        public AgentStatus CurrentAgentStatus { get; set; }
        public bool IsOnCall { get; set; }
    }
}
