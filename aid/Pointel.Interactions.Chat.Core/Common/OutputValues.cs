using Genesyslab.Platform.Commons.Protocols;

namespace Pointel.Interactions.Chat.Core.General
{
    public class OutputValues
    {

        #region Field Declaration
        string messageCode;
        string message;
        private static OutputValues _outPutValues = null;
        IMessage iMessage = null;
        #endregion

        #region Single Instance

        /// <summary>
        ///  Gets the instance.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets or sets the request join attribute message.
        /// </summary>
        /// <value>
        /// The request join attribute message.
        /// </value>
        public IMessage RequestJoinIMessage
        {
            get { return iMessage; }
            set { iMessage = value; }
        }    
        
        #endregion

    }
}
