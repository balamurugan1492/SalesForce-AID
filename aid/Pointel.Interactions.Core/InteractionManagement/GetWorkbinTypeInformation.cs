using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Interactions.Core.InteractionManagement
{
    public  class GetWorkbinTypeInformation
    {
        public static OutputValues GetWorkbinTypes(int tenantId,int proxyId)
        {
            OutputValues result = OutputValues.GetInstance();
            result.Message = string.Empty;
            result.MessageCode = string.Empty;
            result.ErrorCode = 0;
            RequestWorkbinTypesInfo workbinTypeInfoRequest = RequestWorkbinTypesInfo.Create();
            /**
             * Tenant and client proxy id is manditory field to it.
             */
            workbinTypeInfoRequest.TenantId =  tenantId;
            workbinTypeInfoRequest.ProxyClientId = proxyId;
            if (Settings.InteractionProtocol != null && (Settings.InteractionProtocol.State == ChannelState.Opened || Settings.InteractionProtocol.State == ChannelState.Opening))
            {
                IMessage response = Settings.InteractionProtocol.Request(workbinTypeInfoRequest);
                if (response!=null)
                if (response.Name != EventError.MessageName)
                {
                    result.MessageCode = "200";
                    result.Message = "EventWorkbinTypesInfo received"; ;
                    result.IMessage = response;
                }
                else
                {
                    result.MessageCode = "2001";
                    result.Message = "Event error occurred while request workbin type information from interaction server";
                    result.IMessage = response;
                }
            }
            else
            {
                result.MessageCode = "2001";
                result.Message = "Interaction Server protocol not initialized or not in open state";
            }

            return result;

        }
    }
}
