using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle retrieve call
    /// </summary>
    internal class RequestAgentRetrieve
    {
        /// <summary>
        /// This method used to retrieve call if an agent has put the call on hold.
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region Retrieve

        public static Pointel.Softphone.Voice.Common.OutputValues Retrieve()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var retrieveCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestRetrieveCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                retrieveCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                Genesyslab.Platform.Voice.Protocols.ConnectionId connId = null;
                if (Settings.GetInstance().IsAlternateCallClicked)
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultationConnID);
                    //Settings.GetInstance().IsAlternateCallClicked = false;
                }
                else
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                }
                retrieveCall.ConnID = connId;
                Settings.GetInstance().VoiceProtocol.Send(retrieveCall);
                logger.Info("---------------Retrieve------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Call Retrieved";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while retrieving a call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion
    }
}