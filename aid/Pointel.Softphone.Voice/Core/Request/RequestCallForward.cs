using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    internal class RequestCallForward
    {
        /// <summary>
        /// Forwards the call set.
        /// </summary>
        /// <param name="otherDN">The other dn.</param>
        /// <returns></returns>

        #region ForwardCallSet

        public static Pointel.Softphone.Voice.Common.OutputValues ForwardCallSet(string otherDN)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var setCallForward = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestCallForwardSet.Create((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), otherDN, Genesyslab.Platform.Voice.Protocols.TServer.ForwardMode.Unconditional);
                Settings.GetInstance().VoiceProtocol.Send(setCallForward);
                logger.Info("---------------CallForwarded------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                logger.Info("OtherDN:" + otherDN);
                //End
                // logger.Info("ConnectionID:" + connID);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Forward Call Set Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("RequestCallForward:ForwardCall:" + commonException.ToString());
            }
            return output;
        }

        #endregion ForwardCallSet
    }
}