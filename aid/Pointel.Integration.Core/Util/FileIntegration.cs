#region System Namespace

using System.Collections.Generic;

#endregion System Namespace

using Pointel.Integration.Core.iSubjects;

namespace Pointel.Integration.Core.Util
{
    internal class FileIntegration : iFileIntegration
    {
        #region Field Declaration

        private string fileName = string.Empty;
        private string directory = string.Empty;
        private string fileNameFacet = string.Empty;
        private string directoryFacet = string.Empty;
        private string fileFormat = string.Empty;
        private string delimiter = string.Empty;
        private string _contentType = string.Empty;
        private Dictionary<string, string> parameterName = new Dictionary<string, string>();
        private Dictionary<string, string> parameterValue = new Dictionary<string, string>();
        private string[] callDataEventFileType;
        private string[] _attributeFilter;
        private bool _enableView = false;

        #endregion Field Declaration

        #region Property

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable view].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableView
        {
            get { return _enableView; }
            set { _enableView = value; }
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
        /// Gets or sets the type of the call data event file.
        /// </summary>
        /// <value>
        /// The type of the call data event file.
        /// </value>
        public string[] CallDataEventFileType
        {
            get { return callDataEventFileType; }
            set { callDataEventFileType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
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
        /// Gets or sets the file name facet.
        /// </summary>
        /// <value>
        /// The file name facet.
        /// </value>
        public string FileNameFacet
        {
            get { return fileNameFacet; }
            set { fileNameFacet = value; }
        }

        /// <summary>
        /// Gets or sets the directory path facet.
        /// </summary>
        /// <value>
        /// The directory path facet.
        /// </value>
        public string DirectoryPathFacet
        {
            get { return directoryFacet; }
            set { directoryFacet = value; }
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
    }
}