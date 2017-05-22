#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

#region AID Namespace

using Pointel.Integration.Core.iSubjects;

#endregion AID Namespace

namespace Pointel.Integration.Core.Util
{
    internal class PipeIntegration : iPipeIntegration
    {
        #region Field Declaration

        private string pipeName = string.Empty;
        private string fileFormat = string.Empty;
        private string delimiter = string.Empty;
        private Dictionary<string, string> parameterName = new Dictionary<string, string>();
        private Dictionary<string, string> parameterValue = new Dictionary<string, string>();
        private string[] _attributeFilter;
        private string[] callDataEventPipeType;

        #endregion Field Declaration

        #region Property

        /// <summary>
        /// Gets or sets the type of the call data event pipe.
        /// </summary>
        /// <value>
        /// The type of the call data event pipe.
        /// </value>
        public string[] CallDataEventPipeType
        {
            get { return callDataEventPipeType; }
            set { callDataEventPipeType = value; }
        }

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
        /// Gets or sets the name of the pipe.
        /// </summary>
        /// <value>
        /// The name of the pipe.
        /// </value>
        public string PipeName
        {
            get { return pipeName; }
            set { pipeName = value; }
        }

        /// <summary>
        /// Gets or sets the file format.
        /// </summary>
        /// <value>
        /// The file format.
        /// </value>
        public string FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
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


        public bool PipeFist
        {
            get;
            set;
        }
    }
}