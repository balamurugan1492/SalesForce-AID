
using Genesyslab.Platform.Commons.Protocols;
namespace Pointel.Interactions.Core.Common
{
    /// <summary>
    /// This class provides to handle properties Get and Set methods
    /// </summary>
    public class OutputValues
    {
        #region Field Declaration
        private string messageCode;
        private string message;
        private string mediaStatus;
        private string mediaType;
        private int errorCode;
        private int proxyID;
        private static OutputValues _outPutValues = null;
        #endregion

        #region Single Instance
        public static OutputValues GetInstance()
        {
            if (_outPutValues == null)
            {
                _outPutValues = new OutputValues();
                return _outPutValues;
            }
            return _outPutValues;
        }
        #endregion       

        #region Properties

        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        /// <summary>
        ///Gets or sets the message code.
        /// </summary>
        /// <value>
        /// The message code.
        /// </value>
        public string MessageCode
        {
            get { return messageCode; }
            set { messageCode = value; }
        }

        /// <summary >
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public int ProxyID
        {
            get { return proxyID; }
            set { proxyID = value; }
        }
        public string MediaStatus
        {
            get { return mediaStatus; }
            set { mediaStatus = value; }
        }
        public string MediaType
        {
            get { return mediaType; }
            set { mediaType = value; }
        }

        public IMessage IMessage
        {
            get;
            set;
        }
        #endregion

    }
}
