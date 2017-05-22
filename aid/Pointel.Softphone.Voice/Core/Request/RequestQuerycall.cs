using Pointel.Softphone.Voice.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class used to get the call info
    /// </summary>
    internal class RequestQuerycall
    {
        #region RequestQuerycall

        public static Pointel.Softphone.Voice.Common.OutputValues DoRequestQueryCall(string ThisDN, string connectionId)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(connectionId);
                var requestQueryCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Queries.RequestQueryCall.Create(ThisDN, connID, Genesyslab.Platform.Voice.Protocols.TServer.CallInfoType.StatusQuery);
                Settings.GetInstance().VoiceProtocol.Send(requestQueryCall);
                logger.Info("---------------RequestQuerycall------------------");
                logger.Info("ThisDN:" + ThisDN);
                logger.Info("ConnectionID:" + connectionId);
                logger.Info("-------------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Request for Query call Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoRequestQueryCall : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static IMessage DoRequestQueryCall(string ThisDN, string connectionId, bool isUnsolicited)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            IMessage message=null;
            try
            {
                var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(connectionId);
                var requestQueryCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Queries.RequestQueryCall.Create(ThisDN, connID, Genesyslab.Platform.Voice.Protocols.TServer.CallInfoType.StatusQuery);
                requestQueryCall.CallInfoType = Genesyslab.Platform.Voice.Protocols.TServer.CallInfoType.PartiesQuery;
                message  = Settings.GetInstance().VoiceProtocol.Request(requestQueryCall);
                logger.Info("---------------RequestQuerycall------------------");
                logger.Info("ThisDN:" + ThisDN);
                logger.Info("ConnectionID:" + connectionId);
                logger.Info("-------------------------------------------------");
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoRequestQueryCall : " + commonException.ToString());
                message = null;
            }
            return message;
        }

        #endregion RequestQuerycall
    }
}
