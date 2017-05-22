#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

namespace Pointel.Integration.Core.iSubjects
{
    /// <summary>
    /// This interface used to get or set properties of File integration
    /// </summary>
    public interface iFileIntegration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable view].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view]; otherwise, <c>false</c>.
        /// </value>
        bool EnableView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file name facet.
        /// </summary>
        /// <value>
        /// The file name facet.
        /// </value>
        string FileNameFacet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the directory path facet.
        /// </summary>
        /// <value>
        /// The directory path facet.
        /// </value>
        string DirectoryPathFacet
        {
            get;
            set;
        }

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
        /// Gets or sets the type of the call data event file.
        /// </summary>
        /// <value>
        /// The type of the call data event file.
        /// </value>
        string[] CallDataEventFileType
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