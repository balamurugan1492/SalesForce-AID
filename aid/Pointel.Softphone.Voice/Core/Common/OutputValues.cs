namespace Pointel.Softphone.Voice.Common
{
    using Genesyslab.Platform.Commons.Protocols;

    /// <summary>
    /// This class provides to handle properties Get and Set methods
    /// </summary>
    public class OutputValues
    {
        #region Fields

        private static OutputValues _outPutValues = null;

        private Genesyslab.Platform.Commons.Protocols.IMessage iMessage;
        private string message;
        private string messageCode;

        #endregion Fields

        #region Properties

        public IMessage IMessage
        {
            get { return iMessage; }
            set { iMessage = value; }
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

        #endregion Properties

        #region Methods

        public static OutputValues GetInstance()
        {
            if (_outPutValues == null)
            {
                _outPutValues = new OutputValues();
                return _outPutValues;
            }
            return _outPutValues;
        }

        #endregion Methods

        #region Other

        //=====================================================================
        // Methods
        /// <summary>
        ///  Gets the instance.
        /// </summary>
        /// <returns></returns>

        #endregion Other
    }
}