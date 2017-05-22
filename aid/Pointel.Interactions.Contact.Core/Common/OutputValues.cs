using Genesyslab.Platform.Commons.Protocols;

namespace Pointel.Interactions.Contact.Core.Common
{
    public class OutputValues
    {
        #region Field Declaration
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

        /// <summary>
        ///Gets or sets the message code.
        /// </summary>
        /// <value>
        /// The message code.
        /// </value>
        public string MessageCode;

        /// <summary >
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message;

        /// <summary>
        /// Gets or sets the attribute contact message.
        /// </summary>
        /// <value>
        /// The attribute contact message.
        /// </value>
        public IMessage IContactMessage;
        #endregion
    }
}
