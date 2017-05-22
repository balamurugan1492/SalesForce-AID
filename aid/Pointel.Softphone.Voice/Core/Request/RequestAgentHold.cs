

using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle call on hold
    /// </summary>
    internal class RequestAgentHold
    {

        /// <summary>
        /// This method used to put the call on hold
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region Hold

        public static Pointel.Softphone.Voice.Common.OutputValues Hold()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestHoldCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestHoldCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestHoldCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestHoldCall.ConnID = connId;
                Settings.GetInstance().VoiceProtocol.Send(requestHoldCall);
                logger.Info("---------------Hold------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Call OnHold";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Holding a call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion
    }
}