using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Common;
using Pointel.Interactions.Core.Util;
using System;

namespace Pointel.Interactions.Core.InteractionManagement
{
    public class ReleaseSnapshot
    {
        public static OutputValues ReleaseSnapshotID(int proxyClientId, int snapshotID)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues result = OutputValues.GetInstance();
            result.Message = string.Empty;
            result.MessageCode = string.Empty;
            result.ErrorCode = 0;

            try
            {
                RequestReleaseSnapshot requestReleaseSnapshot = RequestReleaseSnapshot.Create();
                requestReleaseSnapshot.ProxyClientId = proxyClientId;
                requestReleaseSnapshot.SnapshotId = snapshotID;

                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(requestReleaseSnapshot);
                    if (response != null)
                        if (response.Name != EventError.MessageName)
                        {
                            result.MessageCode = "200";
                            result.Message = "EventAck received";
                            result.IMessage = response;
                        }
                        else
                        {
                            result.MessageCode = "2001";
                            result.Message = "Event error occurred while releasing the snapshot id from interaction server";
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
                logger.Error("Error occurred while releasing the snapshot id" + generalException.ToString());
                result.MessageCode = "2001";
                result.Message = generalException.Message;
            }
            return result;
        }
    }
}
