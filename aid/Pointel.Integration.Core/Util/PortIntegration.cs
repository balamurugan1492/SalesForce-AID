#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

#region AID Namespace

using Pointel.Integration.Core.iSubjects;

#endregion AID Namespace

namespace Pointel.Integration.Core.Util
{
    internal class PortIntegration : iPortIntegration
    {
        #region Field Declaration

        private string status = string.Empty;
        private string hostName = string.Empty;
        private string delimiter = string.Empty;
        private string format = string.Empty;
        private IntegrationAction decision;
        private Dictionary<string, string> parameterName = new Dictionary<string, string>();
        private Dictionary<string, string> parameterValue = new Dictionary<string, string>();
        private string[] _attributeFilter;
        private string[] callDataEventPortType;

        #endregion Field Declaration

        #region Property

        /// <summary>
        /// Gets or sets the attribute filter.
        /// </summary>
        /// <value>
        /// The attribute filter.
        /// </value>
        public string[] AttributeFilter
        {
            get { return _attributeFilter; }
            set { _attributeFilter = value; }
        }

        /// <summary>
        /// Gets or sets the type of the call data event port.
        /// </summary>
        /// <value>
        /// The type of the call data event port.
        /// </value>
        public string[] CallDataEventPortType
        {
            get { return callDataEventPortType; }
            set { callDataEventPortType = value; }
        }

        /// <summary>
        /// Gets or sets the decision.
        /// </summary>
        /// <value>
        /// The decision.
        /// </value>
        public IntegrationAction Decision
        {
            get { return decision; }
            set { decision = value; }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public Dictionary<string, string> ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public Dictionary<string, string> ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }
        
        #endregion Property



        #region iPortIntegration Members


        public int IncomingPortNumber
        {
            get;
            set;
        }

        public int OutGoingPortNumber
        {
            get;
            set;
        }

        public bool EnableDbColumnMap
        {
            get;
            set;
        }

        public bool EnableDataOutgoing
        {
            get;
            set;
        }

        public string DataProvider
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }
        #endregion

    }
}