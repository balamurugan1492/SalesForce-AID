#region Genesys Namespace

using Genesyslab.Platform.Commons.Protocols;

#endregion Genesys Namespace

namespace Pointel.Integration.Core.iSubjects
{
    public enum MediaType
    {
        Voice,
        Email,
        Chat,
        SMS
    }

    /// <summary>
    /// This interface used to get or set properties of call data
    /// </summary>
    public interface iCallData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the voice event.
        /// </summary>
        /// <value>
        /// The voice event.
        /// </value>
        IMessage EventMessage 
        {
            get; 
            set;
        }

        MediaType MediaType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        iFileIntegration FileData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port data.
        /// </summary>
        /// <value>
        /// The port data.
        /// </value>
        iPortIntegration PortData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pipe data.
        /// </summary>
        /// <value>
        /// The pipe data.
        /// </value>
        iPipeIntegration PipeData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CRM database data.
        /// </summary>
        /// <value>
        /// The CRM database data.
        /// </value>
        iCrmDbIntegration CrmDbData
        {
            get;
            set;
        }

        #endregion Properties
    }
}