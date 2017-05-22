using Pointel.Softphone.Voice.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle agent logout
    /// </summary>
    internal class RequestLogout
    {
        /// <summary>
        /// This method used to logout user from TServer
        /// </summary>
        /// <param name="workMode">Optional - Agent WorkMode</param>
        /// <param name="queue">Optional - Queue</param>
        /// <returns>output</returns>

        #region LogoutAgent

        public static Pointel.Softphone.Voice.Common.OutputValues LogoutAgent()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAgentLogout = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogout.Create(Settings.GetInstance().ACDPosition);

                if (Settings.GetInstance().QueueName.ToLower() != "optional" && Settings.GetInstance().QueueName.ToLower() != "none")
                    requestAgentLogout.ThisQueue = Settings.GetInstance().QueueName;

                Settings.GetInstance().VoiceProtocol.Send(requestAgentLogout);
                logger.Info("---------------LogoutAgent------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                if (Settings.GetInstance().QueueName.ToLower() == "optional" && Settings.GetInstance().QueueName.ToLower() == "none")
                    logger.Info("Queue:" + Settings.GetInstance().QueueName + " not added");
                else
                    logger.Info("Queue:" + Settings.GetInstance().QueueName);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Logout Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while logout agent " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static IMessage LogoutAgentRequest() 
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            IMessage message = null;
            try
            {
                var requestAgentLogout = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogout.Create(Settings.GetInstance().ACDPosition);

                if (Settings.GetInstance().QueueName.ToLower() != "optional" && Settings.GetInstance().QueueName.ToLower() != "none")
                    requestAgentLogout.ThisQueue = Settings.GetInstance().QueueName;

                message = Settings.GetInstance().VoiceProtocol.Request(requestAgentLogout);
                logger.Info("---------------LogoutAgentRequest------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                if (Settings.GetInstance().QueueName.ToLower() == "optional" && Settings.GetInstance().QueueName.ToLower() == "none")
                    logger.Info("Queue:" + Settings.GetInstance().QueueName + " not added");
                else
                    logger.Info("Queue:" + Settings.GetInstance().QueueName);
                logger.Info("--------------------------------------------------");
                return message;
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while logout agent request" + commonException.ToString());
            }
            return message;
        }

        #endregion LogoutAgent
    }
}