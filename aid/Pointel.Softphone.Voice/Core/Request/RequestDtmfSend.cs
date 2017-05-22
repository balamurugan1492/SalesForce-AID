using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This Class used to Send Dtmf Request
    /// </summary>
    internal class RequestDtmfSend
    {
        /// <summary>
        /// DTMFs the send.
        /// </summary>
        /// <returns></returns>

        #region DtmfSend

        public static Pointel.Softphone.Voice.Common.OutputValues DtmfSend(string dtmfDigit)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                var requestSendDtmf = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dtmf.RequestSendDtmf.Create(Settings.GetInstance().ACDPosition, connId, dtmfDigit, new Genesyslab.Platform.Commons.Collections.KeyValueCollection(), new Genesyslab.Platform.Commons.Collections.KeyValueCollection());
                Settings.GetInstance().VoiceProtocol.Send(requestSendDtmf);
                logger.Info("---------------DtmfSend------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Dtmf Send Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while DtmfSend " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion DtmfSend
    }
}