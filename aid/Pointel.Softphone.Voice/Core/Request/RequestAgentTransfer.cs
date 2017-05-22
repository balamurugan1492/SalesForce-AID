namespace Pointel.Softphone.Voice.Core.Request
{
    using System;

    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This class provide to handle consult call
    /// </summary>
    internal class RequestAgentTransfer
    {
        #region Methods

        public static Pointel.Softphone.Voice.Common.OutputValues CompleteTransfer()
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestCompleteTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestCompleteTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestCompleteTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End

                //Code Added to handle connectionid based on Switch Type
                //26/09/2013 V.Palaniappan
                Genesyslab.Platform.Voice.Protocols.ConnectionId connId;
                Genesyslab.Platform.Voice.Protocols.ConnectionId transfConnid;
                if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel") == 0)
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                }
                else
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                    transfConnid = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultConnectionID);
                    requestCompleteTransfer.TransferConnID = transfConnid;
                }
                //End
                requestCompleteTransfer.ConnID = connId;

                Settings.GetInstance().VoiceProtocol.Send(requestCompleteTransfer);
                logger.Info("---------------CompleteTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Call Transfer Completed";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Complete  Transfer call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static Pointel.Softphone.Voice.Common.OutputValues InitiateTransfer(string pOtherDN, Genesyslab.Platform.Commons.Collections.KeyValueCollection reasonCode)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(pOtherDN);

                var requestInitiateTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateTransfer.ConnID = connId;
                requestInitiateTransfer.OtherDN = pOtherDN;
                if (reasonCode != null)
                    requestInitiateTransfer.Reasons = reasonCode;
                //Code Added - V.Palaniappan
                //28.10.2013
                if (requestInitiateTransfer.ThisDN != pOtherDN)
                {
                    Settings.GetInstance().VoiceProtocol.Send(requestInitiateTransfer);
                }
                //End
                logger.Info("---------------InitiateTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("--------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Call Transfer Initiated";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  Transfer call " + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static Pointel.Softphone.Voice.Common.OutputValues InitiateTransfer(string pOtherDN, string location, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(pOtherDN);

                var requestInitiateTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateTransfer.ConnID = connId;
                requestInitiateTransfer.OtherDN = pOtherDN;
                if (!string.IsNullOrEmpty(location))
                    requestInitiateTransfer.Location = location;
                if (userData != null && userData.Count > 0)
                    requestInitiateTransfer.UserData = userData;
                //Code Added - V.Palaniappan
                //28.10.2013
                if (requestInitiateTransfer.ThisDN != pOtherDN)
                {
                    Settings.GetInstance().VoiceProtocol.Send(requestInitiateTransfer);
                }
                //End
                logger.Info("---------------InitiateTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("Location :" + location);
                logger.Info("--------------------------------------------");

                output.MessageCode = "200";
                output.Message = "Call Transfer Initiated";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  Transfer call " + commonException.ToString());

                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        public static Genesyslab.Platform.Commons.Protocols.IMessage InitiateTransferResponse(string pOtherDN, string location, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Genesyslab.Platform.Commons.Protocols.IMessage response = null;
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(pOtherDN);

                var requestInitiateTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateTransfer.ConnID = connId;
                requestInitiateTransfer.OtherDN = pOtherDN;
                if (!string.IsNullOrEmpty(location))
                    requestInitiateTransfer.Location = location;
                if (userData != null && userData.Count > 0)
                    requestInitiateTransfer.UserData = userData;
                //Code Added - V.Palaniappan
                //28.10.2013
                if (requestInitiateTransfer.ThisDN != pOtherDN)
                {
                    response = Settings.GetInstance().VoiceProtocol.Request(requestInitiateTransfer);
                }
                //End
                logger.Info("---------------InitiateTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("Location :" + location);
                logger.Info("--------------------------------------------");

            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  Transfer call " + commonException.ToString());
            }
            return response;
        }

        public static Genesyslab.Platform.Commons.Protocols.IMessage InitiateTransferResponse(string pOtherDN, Genesyslab.Platform.Commons.Collections.KeyValueCollection reasonCode)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Genesyslab.Platform.Commons.Protocols.IMessage response = null;
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(pOtherDN);

                var requestInitiateTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestInitiateTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestInitiateTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                var connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                requestInitiateTransfer.ConnID = connId;
                requestInitiateTransfer.OtherDN = pOtherDN;
                if (reasonCode != null)
                    requestInitiateTransfer.Reasons = reasonCode;
                //Code Added - V.Palaniappan
                //28.10.2013
                if (requestInitiateTransfer.ThisDN != pOtherDN)
                {
                    response = Settings.GetInstance().VoiceProtocol.Request(requestInitiateTransfer);
                }
                //End
                logger.Info("---------------InitiateTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("OtherDN:" + pOtherDN);
                logger.Info("--------------------------------------------");

            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Initiate  Transfer call " + commonException.ToString());

            }
            return response;
        }

        public static Pointel.Softphone.Voice.Common.OutputValues MuteTransfer(string OtherDN, string location, Genesyslab.Platform.Commons.Collections.KeyValueCollection userData, Genesyslab.Platform.Commons.Collections.KeyValueCollection reasons, Genesyslab.Platform.Commons.Collections.KeyValueCollection extensions)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                var requestCompleteTransfer = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestMuteTransfer.Create();
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                requestCompleteTransfer.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //End
                requestCompleteTransfer.OtherDN = OtherDN;

                //Code added by Manikandan on 24/03/2015 to implement Requeue
                if (!string.IsNullOrEmpty(location))
                    requestCompleteTransfer.Location = location;
                if (extensions != null && extensions.Count > 0)
                    requestCompleteTransfer.Extensions = extensions;
                if (reasons != null && reasons.Count > 0)
                    requestCompleteTransfer.Reasons = reasons;
                if (userData != null && userData.Count > 0)
                    requestCompleteTransfer.UserData = userData;
                //End

                //Code Added to handle connectionid based on Switch Type
                //26/09/2013 V.Palaniappan
                Genesyslab.Platform.Voice.Protocols.ConnectionId connId;
                //Genesyslab.Platform.Voice.Protocols.ConnectionId transfConnid;
                if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel") == 0)
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                }
                else
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                    //transfConnid = new ConnectionId(Settings.GetInstance().ConsultConnectionID);
                    //requestCompleteTransfer.TransferConnID = transfConnid;
                }
                //End
                requestCompleteTransfer.ConnID = connId;
                Settings.GetInstance().VoiceProtocol.Request(requestCompleteTransfer);
                logger.Info("---------------CompleteTransfer------------------");
                //Below condition added to decide which DN going to control the call
                //05-14-2013 Palaniappan
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = "Call Transfer Completed";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Mute  Transfer call " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = commonException.Message;
            }
            return output;
        }

        #endregion Methods

        #region Other

        /// <summary>
        /// This method used to initiate transfer
        /// </summary>
        /// <param name="pOtherDN">The otherDN.</param>
        /// <returns>OutputValues.</returns>
        /// <summary>
        /// This method used to complete transfer.
        /// </summary>
        /// <returns>OutputValues.</returns>
        /// <summary>
        /// Mutes the transfer.
        /// </summary>
        /// <param name="OtherDN">The other dn.</param>
        /// <returns></returns>

        #endregion Other
    }
}