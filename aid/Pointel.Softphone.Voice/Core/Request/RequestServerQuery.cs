using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Softphone.Voice.Core.Util;

namespace Pointel.Softphone.Voice.Core.Request
{
    internal class RequestServerQuery
    {
        #region RequestQueryServer

        public static Pointel.Softphone.Voice.Common.OutputValues DoRequestQueryServer()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestQueryServer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Queries.RequestQueryServer.Create();                
                Settings.GetInstance().VoiceProtocol.Send(requestQueryServer);
                logger.Info("---------------RequestQueryServer------------------");

                logger.Info("---------------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Request for Query Server Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoRequestQueryServer : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion RequestQueryServer
    }
}
