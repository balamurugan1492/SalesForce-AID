namespace Pointel.Softphone.Voice.Core.Request
{
    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Softphone.Voice.Core.Util;

    internal class RequestAgentDial
    {
        #region Methods

        public static Pointel.Softphone.Voice.Common.OutputValues Dial(string pOtherDN, KeyValueCollection reason)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(pOtherDN);

                var requestMakeCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestMakeCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestMakeCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                           Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                requestMakeCall.OtherDN = pOtherDN;
                requestMakeCall.MakeCallType = Genesyslab.Platform.Voice.Protocols.TServer.MakeCallType.Regular;
                if (reason != null)
                    requestMakeCall.Reasons = reason;
                Settings.GetInstance().VoiceProtocol.Send(requestMakeCall);
                logger.Info("---------------Dial------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                           Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Make Call Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Dial A Call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method is used to Dial the specified otherDN.
        /// </summary>
        /// <param name="pOtherDN">The otherDN.</param>
        /// <returns>OutputValues.</returns>

        #endregion Other
    }
}