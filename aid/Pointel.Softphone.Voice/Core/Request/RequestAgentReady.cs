using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle agent's ready state
    /// </summary>
    internal class RequestReadyAgent
    {
        /// <summary>
        /// This method used to move an agent to ready state
        /// </summary>
        /// <param name="workMode">Optional - Agent WorkMode</param>
        /// <param name="queue">Optional - Queue</param>
        /// <returns>output</returns>

        #region AgentReady

        public static Pointel.Softphone.Voice.Common.OutputValues AgentReady()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAgentReady = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentReady.Create(Settings.GetInstance().ACDPosition, Settings.GetInstance().WorkMode);

                if (Settings.GetInstance().QueueName.ToLower() != "optional" && Settings.GetInstance().QueueName.ToLower() != "none")
                {
                    requestAgentReady.ThisQueue = Settings.GetInstance().QueueName;
                }
            
                Settings.GetInstance().VoiceProtocol.Send(requestAgentReady);
                logger.Info("---------------AgentReady------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("Workmode:" + Settings.GetInstance().WorkMode);
                logger.Info("--------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Ready Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while login agent " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion AgentReady
    }
}