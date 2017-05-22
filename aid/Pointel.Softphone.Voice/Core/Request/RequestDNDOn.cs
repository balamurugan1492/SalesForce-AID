using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    internal class RequestDNDOn
    {
        /// <summary>
        /// This method used to move an agent to DNDOn state
        /// </summary>
        /// <returns></returns>

        #region DNDOff

        public static Pointel.Softphone.Voice.Common.OutputValues DNDOn()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var rquestDNDOn = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestSetDNDOn.Create(Settings.GetInstance().ACDPosition);
                Settings.GetInstance().VoiceProtocol.Send(rquestDNDOn);
                logger.Info("---------------DNDOn------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "DNDOn Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while DNDOn " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion DNDOff
    }
}