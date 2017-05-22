using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle answer call
    /// </summary>
    internal class RequestAnswer
    {
        /// <summary>
        /// This method used to answer the call
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region Answer

        public static Pointel.Softphone.Voice.Common.OutputValues Answer()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAnswerCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestAnswerCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestAnswerCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestAnswerCall.ConnID = connID;

                Settings.GetInstance().VoiceProtocol.Send(requestAnswerCall);
                logger.Info("---------------Answer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connID);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Answer Call Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Answering a Call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion

        /// <summary>
        /// Answers the specified connection unique identifier.
        /// </summary>
        /// <param name="connId">The connection unique identifier.</param>
        /// <returns></returns>

        #region Answer

        public static Pointel.Softphone.Voice.Common.OutputValues Answer(string connId)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestAnswerCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestAnswerCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestAnswerCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                Settings.GetInstance().ConnectionID = connId;
                var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestAnswerCall.ConnID = connID;

                Settings.GetInstance().VoiceProtocol.Send(requestAnswerCall);
                logger.Info("---------------Answer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connID);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Answer Call Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Answering a Call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Answer
    }
}