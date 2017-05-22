using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle release call
    /// </summary>
    internal class RequestAgentReleaseCall
    {
        /// <summary>
        /// This method used to release the call
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region Release

        public static Pointel.Softphone.Voice.Common.OutputValues Release()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
               "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            var loadUserData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
            try
            {
                var requestReleaseCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestReleaseCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestReleaseCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End

                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestReleaseCall.ConnID = connId;
                Settings.GetInstance().VoiceProtocol.Send(requestReleaseCall);
                logger.Info("---------------Release------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Call Released";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while releasing a call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion

        /// <summary>
        /// Releases the specified connection unique identifier.
        /// </summary>
        /// <param name="connId">The connection unique identifier.</param>
        /// <returns></returns>

        #region Release with ConnId

        public static Pointel.Softphone.Voice.Common.OutputValues Release(string connId)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            var loadUserData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
            try
            {
                var requestReleaseCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestReleaseCall.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestReleaseCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                //foreach (KeyValuePair<string, string> dicUserData in Settings.GetInstance().userData)
                //{
                //    if (!loadUserData.ContainsKey(dicUserData.Key))
                //    {
                //        loadUserData.Add(dicUserData.Key, dicUserData.Value);
                //    }
                //}
                Settings.GetInstance().ConnectionID = connId;
                var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestReleaseCall.ConnID = connID;
                Settings.GetInstance().VoiceProtocol.Send(requestReleaseCall);
                logger.Info("---------------Release------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Call Released";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while releasing a call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Release with ConnId
    }
}