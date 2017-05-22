#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

namespace Pointel.Integration.Core.iSubjects
{
    /// <summary>
    /// This interface used to get or set properties of Port Integration
    /// </summary>
    public interface iPortIntegration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the decision.
        /// </summary>
        /// <value>
        /// The decision.
        /// </value>
       IntegrationAction Decision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attribute filter.
        /// </summary>
        /// <value>
        /// The attribute filter.
        /// </value>
        string[] AttributeFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the call data event port.
        /// </summary>
        /// <value>
        /// The type of the call data event port.
        /// </value>
        string[] CallDataEventPortType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        string HostName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        int IncomingPortNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the out going port number.
        /// </summary>
        /// <value>The out going port number.</value>
        int OutGoingPortNumber
        {
            get;
            set;
        }

        bool EnableDbColumnMap
        {
            get;
            set;
        }

        bool EnableDataOutgoing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        string Delimiter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        Dictionary<string, string> ParameterName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        Dictionary<string, string> ParameterValue
        {
            get;
            set;
        }
        #endregion Properties
    }
}