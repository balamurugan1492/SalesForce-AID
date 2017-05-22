using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.Core.InteractionManagement
{
    public class TakeSnapshot
    {        
        public static OutputValues TakeSnapshotID(int proxyClientId, string condition)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues result = OutputValues.GetInstance();
            result.Message = string.Empty;
            result.MessageCode = string.Empty;
            result.ErrorCode = 0;

            try
            {
                RequestTakeSnapshot requestTakeSnapshot = RequestTakeSnapshot.Create();
                requestTakeSnapshot.CheckInteractionsState = false;
                requestTakeSnapshot.Condition = condition;
                requestTakeSnapshot.Lock = false;
                requestTakeSnapshot.ProxyClientId = proxyClientId;
            
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(requestTakeSnapshot);
                    if (response != null)
                        if (response.Name != EventError.MessageName)
                        {
                            result.MessageCode = "200";
                            result.Message = "EventSnapshotTaken received";
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
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while transfer the interaction" + generalException.ToString());
                result.MessageCode = "2001";
                result.Message = generalException.Message;
            }
            return result;
        }
    }
}
