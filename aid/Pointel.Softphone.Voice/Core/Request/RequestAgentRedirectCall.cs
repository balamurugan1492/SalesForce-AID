using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle redirect call
    /// </summary>
    internal class RequestAgentRedirectCall
    {
        /// <summary>
        /// This method used to redirect the call
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region Redirect

        public static Pointel.Softphone.Voice.Common.OutputValues Redirect(string routeOtherDN)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                if (!string.IsNullOrEmpty(routeOtherDN))
                {
                    var requestRedirectCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestRedirectCall.Create();
                    requestRedirectCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                            Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                    requestRedirectCall.ConnID = connId;
                    requestRedirectCall.OtherDN = routeOtherDN;
                    Settings.GetInstance().VoiceProtocol.Send(requestRedirectCall);
                    logger.Info("---------------Redirect------------------");
                    logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                            Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                    logger.Info("ConnectionID:" + connId);
                    logger.Info("OtherDN:"+routeOtherDN);
                    logger.Info("--------------------------------------------");

                    output.MessageCode = "200";
                    output.Message = "Call Redirected";
                }
                else
                {
                    logger.Info("---------------Redirect------------------");
                    logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                            Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                    logger.Info("ConnectionID:" + connId);
                    logger.Info("OtherDN:" + routeOtherDN);
                    logger.Info("--------------------------------------------");
                    output.MessageCode = "2001";
                    output.Message = "Other DN is null or empty sending Call Redirect";
                }
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while redirectiong a call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion
    }
}