#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    public class OutputValues
    {
        #region Field Declaration
        string messageCode;
        string message;

        private static OutputValues _outPutValues = null;
        #endregion

        /// <summary>
        /// Gets the instance.
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

        #region Property
        public string MessageCode
        {
            get { return messageCode; }
            set { messageCode = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
    }
}
