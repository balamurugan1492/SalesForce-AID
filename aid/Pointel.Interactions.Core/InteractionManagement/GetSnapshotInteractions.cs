using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Pointel.Interactions.Core.InteractionManagement
{
    public class GetSnapshotInteractions
    {
        public static OutputValues GetSnapshotInteractionsbyID(int proxyClientId, int snapshotID, int startFrom, int numberOfInteractions)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

            OutputValues result = OutputValues.GetInstance();
            result.Message = string.Empty;
            result.MessageCode = string.Empty;
            result.ErrorCode = 0;

            try
            {
                RequestGetSnapshotInteractions requestGetSnapshotInteractions = RequestGetSnapshotInteractions.Create();
                requestGetSnapshotInteractions.ProxyClientId = proxyClientId;
                requestGetSnapshotInteractions.SnapshotId = snapshotID;
                requestGetSnapshotInteractions.StartFrom = startFrom;
                requestGetSnapshotInteractions.NumberOfInteractions = numberOfInteractions;

                if (Settings.InteractionProtocol != null && (Settings.InteractionProtocol.State == ChannelState.Opened || Settings.InteractionProtocol.State == ChannelState.Opening))
                {
                    IMessage response = Settings.InteractionProtocol.Request(requestGetSnapshotInteractions);
                    if (response != null)
                        if (response.Name != EventError.MessageName)
                        {
                            result.MessageCode = "200";
                            result.Message = " EventSnapshotInteractions received"; ;
                            result.IMessage = response;
                        }
                        else
                        {
                            result.MessageCode = "2001";
                            result.Message = "Event error occurred while getting snapshot interactions from interaction server";
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
                logger.Error("Error occurred while getting snapshot interactions" + generalException.ToString());
                result.MessageCode = "2001";
                result.Message = generalException.Message;
            }
            return result;
        }
    }
}
