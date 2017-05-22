using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle Alternate call
    /// </summary>
    internal class RequestAgentAlternateCall
    {
        // <summary>
        /// This method is used to do Alternate Call.
        /// </summary>
        /// <returns>OutputValues.</returns>
        # region AlternateCall

        public static Pointel.Softphone.Voice.Common.OutputValues AlternateCall(string passingDN)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
            var outputValues = new Pointel.Softphone.Voice.Common.OutputValues();
            Genesyslab.Platform.Voice.Protocols.ConnectionId connId = null;
            Genesyslab.Platform.Voice.Protocols.ConnectionId prevConnId = null;
            //Below variable Added to identify whether Aletrnate Button has clicked in EventRetrieved Event in Voicemanager Class
            //to change the connectionID
            //04/12/2013 - V.Palaniappan
            Settings.GetInstance().IsAlternateCallClicked = true;
            //End
            try
            {
                var alternateCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestAlternateCall.Create();
                alternateCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultConnectionID);
                prevConnId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                alternateCall.ConnID = connId;
                alternateCall.PreviousConnID = prevConnId;
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("PreviousConnID:" + prevConnId);
                logger.Info("--------------------------------------------");
                Settings.GetInstance().VoiceProtocol.Send(alternateCall);
                outputValues.MessageCode = "200";
                outputValues.Message = "AlternateCall Successful";
                Settings.GetInstance().ConnectionID = connId.ToString();
                Settings.GetInstance().ConsultationConnID = prevConnId.ToString();

                logger.Debug("Settings.GetInstance().ConnectionID : " + Settings.GetInstance().ConnectionID);
                logger.Debug("Settings.GetInstance().ConsultationConnID : " + Settings.GetInstance().ConsultationConnID);

                ////04/12/2013 - V.Palaniappan
                //Settings.GetInstance().alternatetoggle++;
                ////End
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while doing Alternate call " + commonException.ToString());
                outputValues.MessageCode = "2001";
                outputValues.Message = commonException.Message;
            }
            return outputValues;
        }

        # endregion
    }
}