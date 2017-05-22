namespace Pointel.Softphone.Voice.Core.Request
{
    using Genesyslab.Platform.Commons.Protocols;

    using Pointel.Softphone.Voice.Core.Util;

    internal class RequestDNDOff
    {
        #region Methods

        public static Pointel.Softphone.Voice.Common.OutputValues DNDOff()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var rquestDNDOff = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestSetDNDOff.Create(Settings.GetInstance().ACDPosition);
                Settings.GetInstance().VoiceProtocol.Send(rquestDNDOff);
                logger.Info("---------------DNDOff------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "DNDOff Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while DNDOff " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static IMessage DNDOffR()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            try
            {
                var rquestDNDOff = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestSetDNDOff.Create(Settings.GetInstance().ACDPosition);
                var imessage = Settings.GetInstance().VoiceProtocol.Request(rquestDNDOff);
                if (imessage != null)
                {
                    logger.Info("---------------DNDOff------------------");
                    logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                    logger.Info("--------------------------------------------");
                    logger.Trace(imessage.ToString());
                }
                return imessage;
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while DNDOff " + commonException.ToString());
            }
            return null;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to move an agent to DNDOff state
        /// </summary>
        /// <returns></returns>

        #endregion Other
    }
}