#region System Namespaces

#endregion System Namespaces

#region Genesys Namespaces

using Genesyslab.Platform.Commons.Protocols;

#endregion Genesys Namespaces

#region AID Namespace

using Pointel.Integration.Core.iSubjects;

#endregion AID Namespace

namespace Pointel.Integration.Core.Util
{
    internal class CallData : iCallData
    {
        #region Field Declaration

        private iFileIntegration _filePop;
        private iPortIntegration _portPop;
        private iPipeIntegration _pipePop;
        private iCrmDbIntegration _crmDbPop;
        private IMessage _voiceEvent;

        #endregion Field Declaration

        #region iCallData Properties

        /// <summary>
        /// Gets or sets the voice event.
        /// </summary>
        /// <value>
        /// The voice event.
        /// </value>
        public IMessage EventMessage
        {
            get { return _voiceEvent; }
            set { _voiceEvent = value; }
        }

        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        public iFileIntegration FileData
        {
            get
            {
                return _filePop;
            }
            set
            {
                _filePop = value;
            }
        }

        /// <summary>
        /// Gets or sets the port data.
        /// </summary>
        /// <value>
        /// The port data.
        /// </value>
        public iPortIntegration PortData
        {
            get
            {
                return _portPop;
            }
            set
            {
                _portPop = value;
            }
        }

        /// <summary>
        /// Gets or sets the pipe data.
        /// </summary>
        /// <value>
        /// The pipe data.
        /// </value>
        public iPipeIntegration PipeData
        {
            get { return _pipePop; }
            set { _pipePop = value; }
        }


        /// <summary>
        /// Gets or sets the CRM database data.
        /// </summary>
        /// <value>
        /// The CRM database data.
        /// </value>
        public iCrmDbIntegration CrmDbData
        {
            get { return _crmDbPop; }
            set { _crmDbPop = value; }
        }

        #endregion iCallData Properties

        public MediaType MediaType
        {
            get;
            set;
        }
    }
}