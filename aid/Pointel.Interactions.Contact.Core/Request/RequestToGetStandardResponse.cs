using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestToGetStandardResponse
    {
        #region RequestToGetStandardResponse
        /// <summary>
        /// Gets the standard response.
        /// </summary>
        /// <param name="standardresponseId">The standard response identifier.</param>
        /// <returns></returns>
        public static OutputValues GetStandardResponse(string standardresponseId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetStandardResponse requestGetStandardResponse = new RequestGetStandardResponse();
                requestGetStandardResponse.StandardResponseId = standardresponseId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------GetStandardResponse------------");
                    logger.Info("StandardResponseId    :" + standardresponseId);
                    logger.Info("---------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestGetStandardResponse);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;  
                        output.MessageCode = "200";
                        output.Message = "Get Standard Response Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Standard Response Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetStandardResponse() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Standard Response request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        #endregion RequestToGetStandardResponse
    }
}
