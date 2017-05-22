/*
* =====================================
* Pointel.Configuration.Manager.Core.Common
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
namespace Pointel.Configuration.Manager.Common
{
    public class OutputValues
    {
        #region Field Declaration
        string messageCode;
        string message;

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

        #endregion
    }
}
