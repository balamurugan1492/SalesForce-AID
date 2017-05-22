namespace Pointel.Integration.Core.Common
{
    /// <summary>
    /// This class provides to handle properties Get and Set methods
    /// </summary>
    public class OutputValues
    {
        #region Field Declaration

        private string messageCode;
        private string message;

        private static OutputValues _outPutValues = null;

        #endregion Field Declaration

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>

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

        #endregion Single Instance

        #region Properties

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

        #endregion Properties
    }
}