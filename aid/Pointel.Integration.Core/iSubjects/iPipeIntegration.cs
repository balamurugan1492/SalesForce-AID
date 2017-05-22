#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

namespace Pointel.Integration.Core.iSubjects
{
    /// <summary>
    /// This Interface used to get or set properties of Pipe Integration
    /// </summary>
    public interface iPipeIntegration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the pipe.
        /// </summary>
        /// <value>
        /// The name of the pipe.
        /// </value>
        string PipeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file format.
        /// </summary>
        /// <value>
        /// The file format.
        /// </value>
        string FileFormat
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

        bool PipeFist
        { get; set; }

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
        /// Gets or sets the type of the call data event pipe.
        /// </summary>
        /// <value>
        /// The type of the call data event pipe.
        /// </value>
        string[] CallDataEventPipeType
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

        # endregion
    }
}