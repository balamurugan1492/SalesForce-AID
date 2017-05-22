using Pointel.Softphone.Voice.Core.Util;
using Genesyslab.Platform.Commons.Collections;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle consult call
    /// </summary>
    internal class RequestAgentConference
    {
        /// <summary>
        /// This method is used to initiate conference.
        /// </summary>
        /// <param name="pOtherDN">The other DN.</param>
        /// <returns>OutputValues.</returns>

        # region InitiateConference

        public static Pointel.Softphone.Voice.Common.OutputValues InitiateConference(string pOtherDN, KeyValueCollection reasonCode)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance(); 
            try
            {
                var requestInitiateConf = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateConference.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateConf.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateConf.ConnID = connId;
                requestInitiateConf.OtherDN = pOtherDN;
                if (reasonCode != null)
                    requestInitiateConf.Reasons = reasonCode; 
                logger.Info("---------------Initiaite Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("--------------------------------------------");
                if (requestInitiateConf.ThisDN != pOtherDN)
                {
                    Settings.GetInstance().VoiceProtocol.Send(requestInitiateConf);
                }

                output.MessageCode = "200";
                output.Message = "Initiated Conference";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  conference call " + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static Pointel.Softphone.Voice.Common.OutputValues InitiateConference(string pOtherDN, string location, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData, Genesyslab.Platform.Commons.Collections.KeyValueCollection reasons, Genesyslab.Platform.Commons.Collections.KeyValueCollection extensions)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestInitiateConf = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateConference.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateConf.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateConf.ConnID = connId;
                requestInitiateConf.OtherDN = pOtherDN;

                if (!string.IsNullOrEmpty(location))
                    requestInitiateConf.Location = location;
                if (userData != null && userData.Count > 0)
                    requestInitiateConf.UserData = userData;
                if (reasons != null && reasons.Count > 0)
                    requestInitiateConf.Reasons = reasons;
                if (extensions != null && extensions.Count > 0)
                    requestInitiateConf.Extensions = extensions;


                logger.Info("---------------Initiaite Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("--------------------------------------------");
                if (requestInitiateConf.ThisDN != pOtherDN)
                {
                    Settings.GetInstance().VoiceProtocol.Send(requestInitiateConf);
                }

                output.MessageCode = "200";
                output.Message = "Initiated Conference";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  conference call " + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion

        /// <summary>
        /// This method is used to complete Conference.
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region CompleteConference

        public static Pointel.Softphone.Voice.Common.OutputValues CompleteConference()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestCompleteConf = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestCompleteConference.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestCompleteConf.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                var confConnId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultConnectionID);
                requestCompleteConf.ConnID = connId;
                requestCompleteConf.ConferenceConnID = confConnId;
                logger.Info("---------------Complete Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                Settings.GetInstance().VoiceProtocol.Send(requestCompleteConf);

                output.MessageCode = "200";
                output.Message = "Call Conference Completed";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Complete  Conference call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion

        /// <summary>
        /// Deletes from conference.
        /// </summary>
        /// <param name="OtherDN">The other dn.</param>
        /// <returns></returns>

        #region DeleteFromConference

        public static Pointel.Softphone.Voice.Common.OutputValues DeleteFromConference(string OtherDN)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestDeleteConf = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestDeleteFromConference.Create();
                requestDeleteConf.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestDeleteConf.ConnID = connId;
                requestDeleteConf.OtherDN = OtherDN;
                logger.Info("---------------Delete From Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + OtherDN);
                logger.Info("--------------------------------------------");
                if (requestDeleteConf.ThisDN != OtherDN)
                {
                    Settings.GetInstance().VoiceProtocol.Send(requestDeleteConf);
                }

                output.MessageCode = "200";
                output.Message = "Delete Conference";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Delete from conference" + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion DeleteFromConference

        /// <summary>
        /// Singles the step conference.
        /// </summary>
        /// <param name="otherDN">The other dn.</param>
        /// <returns></returns>

        # region SingleStepConference

        public static Pointel.Softphone.Voice.Common.OutputValues SingleStepConference(string otherDN)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestSinglestep = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestSingleStepConference.Create();
                requestSinglestep.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                requestSinglestep.ConnID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestSinglestep.OtherDN = otherDN;
                //requestSinglestep.Location = "Nortel1000";
                //requestSinglestep.UserData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                //requestSinglestep.Reasons = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                //requestSinglestep.Extensions = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();

                logger.Info("---------------Single Step Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("--------------------------------------------");
                Settings.GetInstance().VoiceProtocol.Send(requestSinglestep);

                output.MessageCode = "200";
                output.Message = "Call Conference Completed";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Complete  Conference call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion


        # region SingleStepConference with multiple parameter

        public static Pointel.Softphone.Voice.Common.OutputValues SingleStepConference(string otherDN, string location, KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestSinglestep = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestSingleStepConference.Create();
                requestSinglestep.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                requestSinglestep.ConnID = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestSinglestep.OtherDN = otherDN;
                if (!string.IsNullOrEmpty(location))
                    requestSinglestep.Location = location;
                if (userData != null && userData.Count > 0)
                    requestSinglestep.UserData = userData;
                //requestSinglestep.Location = "Nortel1000";
                //requestSinglestep.UserData = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                //requestSinglestep.Reasons = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                //requestSinglestep.Extensions = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();

                logger.Info("---------------Single Step Conference------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("--------------------------------------------");
                Settings.GetInstance().VoiceProtocol.Send(requestSinglestep);

                output.MessageCode = "200";
                output.Message = "Call Conference Completed";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Complete  Conference call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        # endregion
    }
}