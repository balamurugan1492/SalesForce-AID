using System.Collections.Generic;

namespace Pointel.Integration.Core.Util
{
    public  class PortSettings
    {
        #region Data members
        private static PortSettings portSettings = null;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of the call data event port.
        /// </summary>
        /// <value>
        /// The type of the call data event port.
        /// </value>
        public List<string> CallDataEventType
        {
            get;
            set;
        }

        public List<string> CallMedia
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
        public string HostName
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
        public int IncomingPortNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the out going port number.
        /// </summary>
        /// <value>The out going port number.</value>
        public int OutGoingPortNumber
        {
            get;
            set;
        }

        public string SendDataDelimiter
        {
            get;
            set;
        }

        public string ReceiveDataDelimiter
        {
            get;
            set;
        }

        public List<string> ReceiveDatakey
        {
            get;
            set;
        }

        public List<string> SendAttributeKeyName
        {
            get;
            set;
        }

        public List<string> SendAttributeValue
        {
            get;
            set;
        }

        public List<string> SendUserDataName
        {
            get;
            set;
        }

        public List<string> SendUserDataValue
        {
            get;
            set;
        }

        public string ReceiveConnectionIdName
        {
            get;
            set;
        }

        public string WebServiceURL
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="PortSettings"/> class from being created.
        /// </summary>
        private PortSettings()
        {
            HostName = "127.0.0.1";
            OutGoingPortNumber = 8001;
            IncomingPortNumber = 8000;
            SendDataDelimiter = ReceiveDataDelimiter = "&";
            CallDataEventType = new List<string>();
            CallMedia = new List<string>();
            ReceiveDatakey = new List<string>();
            SendAttributeKeyName = new List<string>();
            SendAttributeValue = new List<string>();
            SendUserDataName = new List<string>();
            SendUserDataValue = new List<string>();
            ReceiveConnectionIdName = "referid";
        }

        public static PortSettings GetInstance()
        {
            if (portSettings == null)
                portSettings = new PortSettings();
            return portSettings;
        }

        #endregion

    }
}
