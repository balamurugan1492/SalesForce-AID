using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle update userdata
    /// </summary>
    internal class RequestUpdateAttachData
    {
        /// <summary>
        /// This method used to modify/delete active call user data
        /// </summary>
        /// <param name="ThisDN">ThisDN</param>
        /// <param name="connectionID">ConnectionID</param>
        /// <param name="userData">UserData</param>

        #region UpdateAttachData

        public static Pointel.Softphone.Voice.Common.OutputValues UpdateAttachData(string ThisDN, string connectionId,
                                               Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                if (!string.IsNullOrEmpty(connectionId) && !string.IsNullOrEmpty(ThisDN) && userData != null)
                {
                    var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(connectionId);
                    var requestUpdateUserData = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Userdata.RequestUpdateUserData.Create(ThisDN, connID, userData);
                    var cProperties = Genesyslab.Platform.Voice.Protocols.TServer.CommonProperties.Create();
                    cProperties.ConnID = connID;
                    cProperties.UserData = userData;
                    cProperties.UserEvent = 85;
                    cProperties.ThisDN = ThisDN;
                    Settings.GetInstance().VoiceProtocol.Send(requestUpdateUserData);
                    logger.Info("---------------UpdateAttachData------------------");
                    logger.Info("ThisDN:" + ThisDN);
                    logger.Info("ConnectionID:" + connectionId);
                    logger.Info("UserData:" + userData);
                    logger.Info("-------------------------------------------------");

                    output.MessageCode = "200";
                    output.Message = "UserData has been Updated";
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "UserData has not been Updated";
                    logger.Info("---------------UpdateAttachData------------------");
                    logger.Info("ThisDN:" + ThisDN);
                    logger.Info("ConnectionID:" + connectionId);
                    logger.Info("UserData:" + userData);
                    logger.Info("-------------------------------------------------");
                }
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoUpdateAttachData : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion UpdateAttachData

        # region DistributeUserEvent

        /// <summary>
        /// Distributes the user event.
        /// </summary>
        /// <param name="ThisDN">The this dn.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public static Pointel.Softphone.Voice.Common.OutputValues DistributeUserEvent(string ThisDN, string connectionId,
                                              Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                if (!string.IsNullOrEmpty(connectionId) && !string.IsNullOrEmpty(ThisDN) && userData != null)
                {
                    var connID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(connectionId);

                    var cProperties = Genesyslab.Platform.Voice.Protocols.TServer.CommonProperties.Create();
                    cProperties.ConnID = connID;
                    cProperties.UserData = userData;
                    cProperties.UserEvent = 85;
                    cProperties.ThisDN = ThisDN;

                    var requestDistribute = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Special.RequestDistributeUserEvent.Create(ThisDN, cProperties);

                    Settings.GetInstance().VoiceProtocol.Send(requestDistribute);

                    logger.Info("---------------DistributeUserEvent------------------");
                    logger.Info("ThisDN:" + ThisDN);
                    logger.Info("ConnectionID:" + connectionId);
                    logger.Info("UserData:" + userData);
                    logger.Info("---------------------------------------------------");

                    output.MessageCode = "200";
                    output.Message = "DistributeUserEvent has been sent";
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "DistributeUserEvent has not been sent";
                    logger.Info("---------------DistributeUserEvent------------------");
                    logger.Info("ThisDN:" + ThisDN);
                    logger.Info("ConnectionID:" + connectionId);
                    logger.Info("UserData:" + userData);
                    logger.Info("----------------------------------------------------");
                    logger.Warn("DistributeUserEvent has not been sent");
                }
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoDistributeUserEvent : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }


        /// <summary>
        /// Distributes the user event.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public static Pointel.Softphone.Voice.Common.OutputValues DistributeUserEvent(Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                if (userData != null)
                {
                    var cProperties = Genesyslab.Platform.Voice.Protocols.TServer.CommonProperties.Create();
                    cProperties.UserData = userData;
                    cProperties.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));

                    var requestDistribute = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Special.RequestDistributeUserEvent.Create((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), cProperties);

                    Settings.GetInstance().VoiceProtocol.Send(requestDistribute);

                    logger.Info("---------------DistributeUserEvent------------------");
                    logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                    logger.Info("UserData:" + userData);
                    logger.Info("---------------------------------------------------");

                    output.MessageCode = "200";
                    output.Message = "DistributeUserEvent has been sent";
                }
                else
                {
                    output.MessageCode = "2001";
                    output.Message = "DistributeUserEvent has not been sent";
                    logger.Info("---------------DistributeUserEvent------------------");
                    logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                    logger.Info("UserData:" + userData);
                    logger.Info("----------------------------------------------------");
                    logger.Warn("DistributeUserEvent has not been sent");
                }
            }
            catch (System.Exception commonException)
            {
                logger.Error("DoDistributeUserEvent : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }
        # endregion
    }
}