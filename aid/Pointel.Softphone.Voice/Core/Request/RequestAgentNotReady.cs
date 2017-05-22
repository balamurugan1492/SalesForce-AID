namespace Pointel.Softphone.Voice.Core.Request
{
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This class provide to handle agent's NotReady Status
    /// </summary>
    internal class RequestNotReady
    {
        #region Methods

        /// <summary>
        /// Agents the after call work.
        /// </summary>
        /// <returns></returns>
        public static Pointel.Softphone.Voice.Common.OutputValues AgentAfterCallWork()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAgentAfterCallWork = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentNotReady.Create(Settings.GetInstance().ACDPosition, Genesyslab.Platform.Voice.Protocols.TServer.AgentWorkMode.AfterCallWork);

                if (Settings.GetInstance().QueueName.ToLower() != "optional" && Settings.GetInstance().QueueName.ToLower() != "none")
                    requestAgentAfterCallWork.ThisQueue = Settings.GetInstance().QueueName;
                Settings.GetInstance().VoiceProtocol.Send(requestAgentAfterCallWork);
                logger.Info("---------------AfterCallWork------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("Queue:" + Settings.GetInstance().QueueName);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "After Call Work Moved Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Agent After Call Work " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static Pointel.Softphone.Voice.Common.OutputValues AgentNotReady(string reason, string code, bool isSolicited)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAgentNotReady = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent.RequestAgentNotReady.Create(Settings.GetInstance().ACDPosition, Settings.GetInstance().WorkMode);

                var ReasonCode = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();

                //Code Added to check whether send configurable reason key and code/default key and code to tserver
                //27.09.2013 V.Palaniappan
                if (!string.IsNullOrEmpty(Settings.GetInstance().NotReadyKey))
                {
                    ReasonCode.Add(Settings.GetInstance().NotReadyKey, reason);
                }
                if (!string.IsNullOrEmpty(Settings.GetInstance().NotReadyCodeKey))
                {
                    ReasonCode.Add(Settings.GetInstance().NotReadyCodeKey, code);
                }
                if (string.IsNullOrEmpty(Settings.GetInstance().NotReadyKey) && string.IsNullOrEmpty(Settings.GetInstance().NotReadyCodeKey))
                {
                    ReasonCode.Add("Name", reason);
                    ReasonCode.Add("Code", code);
                }
                if (string.IsNullOrEmpty(reason) && string.IsNullOrEmpty(code))
                {
                    ReasonCode = null;
                }
                //End
                if (string.Compare(Settings.GetInstance().NotReadyRequest.ToLower(), "extensions", true) == 0)
                {
                    requestAgentNotReady.Extensions = ReasonCode;
                }
                else
                {
                    requestAgentNotReady.Reasons = ReasonCode;
                }
                //Code Added - To resolve agent not ready issue in client box
                //11.10.2013 V.Palaniappan
                if (Settings.GetInstance().QueueName.ToLower() != "optional" && Settings.GetInstance().QueueName.ToLower() != "none")
                {
                    requestAgentNotReady.ThisQueue = Settings.GetInstance().QueueName;
                }

                logger.Info("---------------AgentNotReady------------------");
                logger.Info("ThisDN:" + Settings.GetInstance().ACDPosition);
                logger.Info("Reason:" + reason);
                logger.Info("Reasoncode:" + code);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Not Ready Successful";

                //End
                if (isSolicited)
                    output.IMessage = Settings.GetInstance().VoiceProtocol.Request(requestAgentNotReady);
                else
                    Settings.GetInstance().VoiceProtocol.Send(requestAgentNotReady);
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Agent Not Ready " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to make an agent to NotReady state.
        /// </summary>
        /// <param name="workMode">Optional - Agent WorkMode</param>
        /// <param name="queue">Optional - Queue</param>
        /// <returns>output</returns>

        #endregion Other
    }
}