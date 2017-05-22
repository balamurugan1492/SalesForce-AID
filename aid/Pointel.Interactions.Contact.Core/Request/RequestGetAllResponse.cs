using System;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetAllResponse
    {

        #region RequestGetAllResponse
        /// <summary>
        /// Gets the content of the response.
        /// </summary>
        /// <param name="tenantID">The tenant unique identifier.</param>
        /// <returns></returns>
        public static OutputValues GetResponseContent(int tenantID) 
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID"); 
            OutputValues output = OutputValues.GetInstance();
            try
            {
                logger.Debug("Getting the AllAttributes from the UCS server");
                RequestGetAllCategories getAllCategories = new RequestGetAllCategories();
                getAllCategories.TenantId = tenantID;
                getAllCategories.Language = "English";
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("--------GetResponseContent---------");
                    logger.Info("TenantId    :" + tenantID);
                    logger.Info("-----------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(getAllCategories);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Get  All Response Content Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Get All Response Content Successful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetResponseContent() : Universal Contact Server protocol is Null..");
                }
               

            }
            catch (Exception ex)
            {
                output.IContactMessage = null;
                output.Message = "Error occurred while get all standard response ";
                output.MessageCode = "2001";
                logger.Error("GetResponseContent()" + ex.ToString());
            }
            return output;
        }

        #endregion RequestGetAllResponse

    }
}
