using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle agent login
    /// </summary>
    internal class RequestLogin
    {
        /// <summary>
        /// This method used to login user in TServer
        /// </summary>
        /// <param name="workMode">Optional - Agent WorkMode</param>
        /// <param name="queue">Optional - Queue</param>
        /// <returns>output</returns>

        #region LoginAgent

        public static Pointel.Softphone.Voice.Common.OutputValues LoginAgent(string workMode, string queue, string agentLoginId, string agentPassword)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogin requestAgentLogin = null;
            try
            {
                if (workMode.ToLower() == "optional" || workMode.ToLower() == "none")//Sends work mode from CME
                    requestAgentLogin = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogin.Create(Settings.GetInstance().ACDPosition, Settings.GetInstance().WorkMode);
                else
                    requestAgentLogin = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentLogin.Create(Settings.GetInstance().ACDPosition, FindWorkMode(workMode));
                if (queue.ToLower() != "optional" && queue.ToLower() != "none")
                {
                    requestAgentLogin.ThisQueue = queue;
                }
                requestAgentLogin.AgentID = agentLoginId;
                requestAgentLogin.Password = agentPassword;
                Settings.GetInstance().VoiceProtocol.Send(requestAgentLogin);
                logger.Info("---------------LoginAgent------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("Work mode:" + Settings.GetInstance().WorkMode);
                if (queue.ToLower() == "optional" && queue.ToLower() == "none")
                    logger.Info("Queue:" + queue + " Not added");
                else
                    logger.Info("Queue:" + queue);
                logger.Info("Agent ID:" + requestAgentLogin.AgentID);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Login Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while login agent " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion LoginAgent

        /// <summary>
        /// This method used to get agent work mode tye
        /// </summary>
        /// <param name="workMode">work mode in string</param>
        /// <returns>AgentWorkMode</returns>

        #region FindWorkMode

        private static Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode FindWorkMode(string workMode)
        {
            var result = Settings.GetInstance().WorkMode;

            foreach (int value in System.Enum.GetValues(typeof(Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode)))
            {
                if (string.Compare((System.Enum.GetName(typeof(Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode), value)), workMode, true) == 0)
                {
                    result = (Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode)value;
                }
            }
            return result;
        }

        #endregion FindWorkMode
    }
}