namespace Pointel.Softphone.Voice.Core.Request
{
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This class provide to reconnect a call
    /// </summary>
    internal class RequestAgentReconnect
    {
        #region Methods

        public static Pointel.Softphone.Voice.Common.OutputValues Reconnect(PhoneFunctions pStatus)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestReconnectCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestReconnectCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestReconnectCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                var reconnectConnID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultConnectionID);
                //requestReconnectCall.ConnID = connId;
                //requestReconnectCall.PreviousConnID = reconnectConnID;

                requestReconnectCall.ConnID = reconnectConnID;
                requestReconnectCall.PreviousConnID = connId;
                Settings.GetInstance().VoiceProtocol.Send(requestReconnectCall);
                logger.Info("---------------Reconnect------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + reconnectConnID);
                logger.Info("PreviousConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                switch (pStatus)
                {
                    case PhoneFunctions.CancelTransfer:

                        output.MessageCode = "200";
                        output.Message = "Call Reconnected";
                        break;

                    case PhoneFunctions.CancelConference:
                        output.MessageCode = "200";
                        output.Message = "Call Reconnected";
                        break;
                }
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while reconnect the  call " + commonException.ToString());
                switch (pStatus)
                {
                    case PhoneFunctions.CancelTransfer:

                        output.MessageCode = "2001";
                        output.Message = commonException.Message;
                        break;

                    case PhoneFunctions.CancelConference:

                        output.MessageCode = "2001";
                        output.Message = commonException.Message;
                        break;
                }
            }
            return output;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to reconnect the call once an agent has intitiate Transfer/Conference
        /// </summary>
        /// <param name="pStatus">The status.</param>
        /// <returns>OutputValues.</returns>

        #endregion Other
    }
}