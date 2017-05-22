using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    internal class RequestCancelCallForward
    {
        /// <summary>
        /// Forwards the call cancel.
        /// </summary>
        /// <returns></returns>

        #region ForwardCallCancel

        public static Pointel.Softphone.Voice.Common.OutputValues ForwardCallCancel()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var cancelCallForward = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestCallForwardCancel.Create((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), Genesyslab.Platform.Voice.Protocols.TServer.ForwardMode.Unconditional);
                Settings.GetInstance().VoiceProtocol.Send(cancelCallForward);
                logger.Info("---------------CancelCallForwarded------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                // logger.Info("ConnectionID:" + connID);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Forward Call Cancel Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("RequestCancelCallForward:ForwardCallCancel():" + commonException.ToString());
            }
            return output;
        }

        #endregion ForwardCallCancel
    }
}