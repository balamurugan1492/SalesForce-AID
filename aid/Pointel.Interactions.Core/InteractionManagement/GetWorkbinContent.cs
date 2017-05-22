using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.Commons.Protocols;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;

namespace Pointel.Interactions.Core.InteractionManagement
{
   public class GetWorkbinContent
    {
        #region Data Members
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
      "AID");
        
        #endregion

        public  OutputValues getWorkbinContent(string agentId, string placeId, string workbinName, int proxyId)
       {
           OutputValues output = new OutputValues();
           output.Message = string.Empty;
           output.MessageCode = string.Empty;
           output.ErrorCode = 0;
           try
            {
                RequestGetWorkbinContent getWorkbinContent1 = null;
                WorkbinInfo winfo = null;
                getWorkbinContent1 = RequestGetWorkbinContent.Create();
                getWorkbinContent1.ProxyClientId = proxyId;
                winfo = WorkbinInfo.Create();
                winfo.WorkbinAgentId = agentId;
                if (!string.IsNullOrEmpty(placeId))
                    winfo.WorkbinPlaceId = placeId;
                winfo.WorkbinTypeId = workbinName;
                getWorkbinContent1.Workbin = winfo;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(getWorkbinContent1);
                    if (response.Id.Equals(EventWorkbinContent.MessageId))
                    {
                        output.MessageCode = "200";
                        output.IMessage = response;
                        output.Message = "Workbin Content retrieved successfully";
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.IMessage = response;
                        output.Message = "Eorror occurred while retrieve workbin content";
                    }
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "Interaction server is not active";
                }
            }
            catch (Exception ex)
            {
                OutputValues.GetInstance().Message = "GetWorkbinContent " + ex.Message;
                OutputValues.GetInstance().MessageCode = "2001";
            }
           return output;
        }
    }
}
