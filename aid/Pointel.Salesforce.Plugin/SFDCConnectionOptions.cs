namespace Pointel.Salesforce.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Protocols;

    public class SFDCConnectionOptions
    {
        #region Properties

        public List<CfgAgentGroup> AgentGroups
        {
            get;
            set;
        }

        public CfgApplication Application
        {
            get;
            set;
        }

        public IConfService ConfService
        {
            get;
            set;
        }

        public bool EnableSubcriberLog
        {
            get;
            set;
        }

        public CfgPerson Person
        {
            get;
            set;
        }

        public string ThisDN
        {
            get;
            set;
        }

        public IProtocol TserverProtocol
        {
            get;
            set;
        }

        public IProtocol UCSProtocol
        {
            get;
            set;
        }

        #endregion Properties
    }
}