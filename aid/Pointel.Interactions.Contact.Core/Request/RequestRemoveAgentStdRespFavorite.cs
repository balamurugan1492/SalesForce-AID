using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestRemoveAgentStdRespFavorite
    {
        #region RequestRemoveAgentStdRespFavorite
        /// <summary>
        /// Removes the agent favorite response.
        /// </summary>
        /// <param name="stdResponse">The standard response.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns></returns>
        public static OutputValues RemoveAgentFavoriteResponse(string stdResponse, string agentId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestDeleteAgentStdRespFavorite requestDeleteAgentStdRespFavorite = new RequestDeleteAgentStdRespFavorite();
                requestDeleteAgentStdRespFavorite.StandardResponse = stdResponse;
                int? agentid = int.Parse(agentId.ToString());
                requestDeleteAgentStdRespFavorite.AgentId = agentid;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------RemoveAgentFavoriteResponse------------");
                    logger.Info("StandardResponse    :" + stdResponse);
                    logger.Info("AgentId    :" + agentId);
                    logger.Info("----------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestDeleteAgentStdRespFavorite);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Delete Agent Standard Response Favorite Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Delete Agent Standard Response Favorite Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("RemoveAgentFavoriteResponse() : Universal Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Delete Agent Standard Response Favorite request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
