#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

#region AID Namespace

using Pointel.Integration.Core.iSubjects;

#endregion AID Namespace

namespace Pointel.Integration.Core.Util
{
    internal class CrmDbIntegration : iCrmDbIntegration
    {
        #region Declarations

        private string directory = string.Empty;
        private string crmDbfileFormat = string.Empty;
        private string delimiter = string.Empty;
        private string connectionsqlpath = string.Empty;
        private string connectionOraclpath = string.Empty;
        private Dictionary<string, string> parameterName = new Dictionary<string, string>();
        private Dictionary<string, string> parameterValue = new Dictionary<string, string>();
        private string[] _attributeFilter;
        private string[] callDataEventDBType;

        #endregion Declarations

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
        /// Gets or sets the type of the call data event database.
        /// </summary>
        /// <value>
        /// The type of the call data event database.
        /// </value>
        public string[] CallDataEventDBType
        {
            get { return callDataEventDBType; }
            set { callDataEventDBType = value; }
        }

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>
        /// The directory path.
        /// </value>
        public string DirectoryPath
        {
            get { return directory; }
            set { directory = value; }
        }

        /// <summary>
        /// Gets or sets the connection SQL path.
        /// </summary>
        /// <value>
        /// The connection SQL path.
        /// </value>
        public string ConnectionSqlPath
        {
            get { return connectionsqlpath; }
            set { connectionsqlpath = value; }
        }

        /// <summary>
        /// Gets or sets the connection oracle path.
        /// </summary>
        /// <value>
        /// The connection oracle path.
        /// </value>
        public string ConnectionOraclePath
        {
            get { return connectionOraclpath; }
            set { connectionOraclpath = value; }
        }

        /// <summary>
        /// Gets or sets the CRM database format.
        /// </summary>
        /// <value>
        /// The CRM database format.
        /// </value>
        public string CrmDbFormat
        {
            get { return crmDbfileFormat; }
            set { crmDbfileFormat = value; }
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
    }
}