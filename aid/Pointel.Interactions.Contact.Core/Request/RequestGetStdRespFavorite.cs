using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetStdRespFavorite
    {
        #region RequestGetStdRespFavorite
        /// <summary>
        /// Gets the agent favorite response.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns></returns>
        public static OutputValues GetAgentFavoriteResponse(NullableInt agentId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestGetAgentStdRespFavorites requestGetAgentStdRespFavorites = new RequestGetAgentStdRespFavorites();
                requestGetAgentStdRespFavorites.AgentId = agentId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------GetAgentFavoriteResponse---------");
                    logger.Info("AgentId    :" + agentId);
                    logger.Info("------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestGetAgentStdRespFavorites);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Get Agent Standard Response Favorites Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get Agent Standard Response Favorites Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetAgentFavoriteResponse() : Universal Contact Server Protocol is Null");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Agent Standard Response Favorites request" + generalException.ToString()); 
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
