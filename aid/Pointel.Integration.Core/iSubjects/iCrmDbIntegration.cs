#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

namespace Pointel.Integration.Core.iSubjects
{
    /// <summary>
    /// This interface used to get or set properties of CRM DE Integration
    /// </summary>
    public interface iCrmDbIntegration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>
        /// The directory path.
        /// </value>
        string DirectoryPath
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
        /// Gets or sets the type of the call data event database.
        /// </summary>
        /// <value>
        /// The type of the call data event database.
        /// </value>
        string[] CallDataEventDBType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CRM database format.
        /// </summary>
        /// <value>
        /// The CRM database format.
        /// </value>
        string CrmDbFormat
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
        /// Gets or sets the connection SQL path.
        /// </summary>
        /// <value>
        /// The connection SQL path.
        /// </value>
        string ConnectionSqlPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection oracle path.
        /// </summary>
        /// <value>
        /// The connection oracle path.
        /// </value>
        string ConnectionOraclePath
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