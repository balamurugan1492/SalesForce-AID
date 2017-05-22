namespace Pointel.Softphone.Voice.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.Types;
    using Genesyslab.Platform.Voice.Protocols.TServer.Events;

    using Pointel.Configuration.Manager;
    using Pointel.Softphone.Voice.Common;
    using Pointel.Softphone.Voice.Core.Application;
    using Pointel.Softphone.Voice.Core.ConnectionManager;
    using Pointel.Softphone.Voice.Core.Exceptions;
    using Pointel.Softphone.Voice.Core.Listener;
    using Pointel.Softphone.Voice.Core.Request;
    using Pointel.Softphone.Voice.Core.Util;

    /// <summary>
    /// This class provide to handled softphone request
    /// </summary>
    public class SoftPhone
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion Fields

        #region Methods

        public void AlternateCall(string thisDN)
        {
            try
            {
                RequestAgentAlternateCall.AlternateCall(thisDN);
            }
            catch (Exception commonException)
            {
                logger.Error("Softphone_AlternateCall:" + commonException.ToString());
            }
        }

        public void Answer()
        {
            RequestAnswer.Answer();
        }

        public void CancelConference()
        {
            RequestAgentReconnect.Reconnect(PhoneFunctions.CancelConference);
        }

        public void CancelTransfer()
        {
            RequestAgentReconnect.Reconnect(PhoneFunctions.CancelTransfer);
        }

        public void CompleteConference()
        {
            RequestAgentConference.CompleteConference();
            HoldingFlagStatus(PhoneFunctions.CompleteConference);
        }

        public void CompleteTransfer()
        {
            RequestAgentTransfer.CompleteTransfer();
            HoldingFlagStatus(PhoneFunctions.CompleteTransfer);
        }

        public void DeleteConference(string OtherDN)
        {
            try
            {
                RequestAgentConference.DeleteFromConference(OtherDN);
            }
            catch (Exception commonException)
            {
                logger.Error("Softphone_DeleteConference:" + commonException.ToString());
            }
        }

        public OutputValues Dial(string number, KeyValueCollection reason = null)
        {
            //Input Validation
            CheckException.CheckDialValues(number);
            return RequestAgentDial.Dial(number, reason);
        }

        public void DistributeUserDataEvent(Dictionary<string, string> UserData, string connectionID = "")
        {
            KeyValueCollection getUserdata = GetUpdateUserData(UserData);
            //Input Validation
            CheckException.CheckUserDataValues((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), (connectionID == string.Empty ? Settings.GetInstance().ConnectionID : connectionID), getUserdata);

            RequestUpdateAttachData.DistributeUserEvent((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), (connectionID == string.Empty ? Settings.GetInstance().ConnectionID : connectionID), getUserdata);
        }

        public OutputValues DNDOff()
        {
            return RequestDNDOff.DNDOff();
        }

        public IMessage DNDOffR()
        {
            return RequestDNDOff.DNDOffR();
        }

        public OutputValues DNDOn()
        {
            return RequestDNDOn.DNDOn();
        }

        public OutputValues DtmfSend(string number)
        {
            OutputValues output = new OutputValues() { MessageCode = "201" };
            try
            {
                output = RequestDtmfSend.DtmfSend(number);
            }
            catch (Exception commonException)
            {
                logger.Error("Softphone_DtmfSend:" + commonException.ToString());
            }
            if (output.MessageCode == "200")
                output.Message = "Dtmf Send Successfully...";
            else
                output.Message = "Error occurred while sending Dtmf...";
            return output;
        }

        public void ForwardCallCancel()
        {
            RequestCancelCallForward.ForwardCallCancel();

            //if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel", true) == 0)
            //{
            //    RequestCancelCallForward callForwardCancel = new RequestCancelCallForward();
            //    callForwardCancel.ForwardCallCancel();
            //}
            //else
            //{
            //    RequestCancelCallForward callForwardCancel = new RequestCancelCallForward();
            //    callForwardCancel.ForwardCallCancel();
            //}
        }

        public void ForwardCallSet(string otherDN)
        {
            RequestCallForward.ForwardCallSet(otherDN);

            //if (string.Compare(Settings.GetInstance().SwitchTypeName, "nortel", true) == 0)
            //{
            //    RequestCallForward.ForwardCallSet(otherDN);
            //}
            //else
            //{
            //    RequestCallForward.ForwardCallSet(otherDN);
            //    //Logout();
            //}
        }

        public void GetCallStatus(string connectionId, string partyState)
        {
            if (string.IsNullOrEmpty(Settings.GetInstance().PartyState) && !string.IsNullOrEmpty(partyState))
                Settings.GetInstance().PartyState = partyState;
            if (string.IsNullOrEmpty(Settings.GetInstance().ConnectionID) && !string.IsNullOrEmpty(connectionId))
                Settings.GetInstance().ConnectionID = connectionId;
            RequestQuerycall.DoRequestQueryCall(Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN), Settings.GetInstance().ConnectionID);
        }

        public IMessage GetCallStatusInfo(string connectionId)
        {
            IMessage message = RequestQuerycall.DoRequestQueryCall(Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN), connectionId, true);
            return message;
        }

        public void GetServerInfo()
        {
            RequestServerQuery.DoRequestQueryServer();
        }

        public void Hold()
        {
            RequestAgentHold.Hold();
        }

        public OutputValues Initialize(string place, string userName, ConfService configObject, string tServerApplicationName, string agentLoginId, string agentPassword,
            CfgSwitch switchType)
        {
            Settings.GetInstance().AgentLoginID = agentLoginId;
            Settings.GetInstance().Switch = switchType;
            Settings.GetInstance().SwitchTypeName = switchType.Type == CfgSwitchType.CFGLucentDefinityG3 ?
                "avaya" :
                ((switchType.Type == CfgSwitchType.CFGNortelDMS100 || switchType.Type == CfgSwitchType.CFGNortelMeridianCallCenter) ? "nortel" : "avaya");
            var output = OutputValues.GetInstance();
            var connect = new VoiceConnectionManager();
            var read = new ReadConfigObjects();
            //Print DLL Info
            try
            {
                Assembly assemblyVersion = Assembly.LoadFrom(Environment.CurrentDirectory + @"\Pointel.Softphone.Voice.dll");
                if (assemblyVersion != null)
                {
                    logger.Debug("*********************************************");
                    logger.Debug(assemblyVersion.GetName().Name + " : " + assemblyVersion.GetName().Version);
                    logger.Debug("*********************************************");
                }
            }
            catch (Exception versionException)
            {
                logger.Error("Error occurred while getting the version of the SoftPhone library " + versionException.ToString());
            }

            try
            {
                //ConnectionSettings.comObject = configObject;
                //Get Place details
                Settings.GetInstance().PlaceName = place;
                Settings.GetInstance().UserName = userName;
                output = read.ReadPlaceObject();
                //Read Person Details
                //output = read.ReadPersonObject(userName);
                read.ReadApplicationObject(tServerApplicationName, agentLoginId);
            }
            catch (Exception inputException)
            {
                logger.Error("Error occurred while login into SoftPhone " + inputException);
            }

            //Input Validation
            CheckException.CheckLoginValues(place, userName);
            if (output.MessageCode == "200")
            {
                //Register with TServer
                output = connect.ConnectTServer(Settings.GetInstance().PrimaryApplication, Settings.GetInstance().SecondaryApplication);

                if (output.MessageCode != "200")
                {
                    logger.Debug("Protocol is not opened, try to connect with server config keys");
                    if (Settings.GetInstance().VoiceProtocol != null && Settings.GetInstance().VoiceProtocol.State != ChannelState.Opened)
                    {
                        if (!string.IsNullOrEmpty(Settings.GetInstance().PrimaryTServerName))
                        {
                            logger.Debug("Primary TServer name : " + Settings.GetInstance().PrimaryTServerName);
                            Settings.GetInstance().PrimaryApplication = read.ReadApplicationLevelServerDetails(Settings.GetInstance().PrimaryTServerName);
                        }
                        if (!string.IsNullOrEmpty(Settings.GetInstance().SecondaryTServerName))
                        {
                            logger.Debug("Secondary TServer name : " + Settings.GetInstance().SecondaryTServerName);
                            Settings.GetInstance().SecondaryApplication = read.ReadApplicationLevelServerDetails(Settings.GetInstance().SecondaryTServerName);

                            if (Settings.GetInstance().PrimaryApplication == null && Settings.GetInstance().SecondaryApplication != null)
                            {
                                logger.Debug("Primary server is not configured, Secondary server is assigned to Primary server");
                                Settings.GetInstance().PrimaryApplication = Settings.GetInstance().SecondaryApplication;
                            }
                        }
                        else
                        {
                            logger.Debug("secondary application name is not configured");
                            if (Settings.GetInstance().SecondaryApplication == null)
                            {
                                logger.Debug("Secondary server is not configured, primary server is assigned to secondary server");
                                Settings.GetInstance().SecondaryApplication = Settings.GetInstance().PrimaryApplication;
                            }
                        }

                        //connect with server names from options tab
                        output = connect.ConnectTServer(Settings.GetInstance().PrimaryApplication, Settings.GetInstance().SecondaryApplication);
                    }
                    else
                    {
                        return output;
                    }
                }
            }

            return output;
        }

        public void InitiateConference(string number, KeyValueCollection userData)
        {
            //Input Validation
            CheckException.CheckDialValues(number);

            RequestAgentConference.InitiateConference(number, userData);
            if ((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)) != number)
            {
                HoldingFlagStatus(PhoneFunctions.IntiateConference);
            }
        }

        public void InitiateConference(string otherDn, string location, KeyValueCollection userData, KeyValueCollection reasons, KeyValueCollection extensions)
        {
            //Input Validation
            CheckException.CheckDialValues(otherDn);

            RequestAgentConference.InitiateConference(otherDn, location, userData, reasons, extensions);
            if ((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)) != otherDn)
            {
                HoldingFlagStatus(PhoneFunctions.IntiateConference);
            }
        }

        public void InitiateTransfer(string number, KeyValueCollection reasonCode)
        {
            //Input Validation
            CheckException.CheckDialValues(number);

            logger.Info("InitiateTransfer(" + number + ")");

            if ((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)) != number)
            {
                HoldingFlagStatus(PhoneFunctions.InitiateTransfer);
                logger.Debug("Set HoldingFlagStatus(PhoneFunctions.InitiateTransfer)");
            }

            RequestAgentTransfer.InitiateTransfer(number, reasonCode);

            ////TERR_INV_CALD_DN
            //OutputValues errorOutput = new OutputValues();
            //errorOutput.MessageCode = "71";
            //errorOutput.Message = "TERR_INV_CALD_DN" + " : " + "Invalid Called DN.";
            //if (Settings.GetInstance().IsEnableInitiateTransfer)
            //    Settings.GetInstance().IsEnableInitiateTransfer = false;
            //if (Settings.GetInstance().IsEnableInitiateConference)
            //    Settings.GetInstance().IsEnableInitiateConference = false;
            //VoiceManager.messageToClient.NotifyErrorMessage(errorOutput);
        }

        public void InitiateTransfer(string number, string location, KeyValueCollection userData)
        {
            //Input Validation
            CheckException.CheckDialValues(number);

            logger.Info("InitiateTransfer(" + number + ")");
            //Code Added - V.Palaniappan
            //28.10.2013
            if ((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)) != number)
            {
                HoldingFlagStatus(PhoneFunctions.InitiateTransfer);
                logger.Info("Set HoldingFlagStatus(PhoneFunctions.InitiateTransfer)");
            }
            //End

            RequestAgentTransfer.InitiateTransfer(number, location, userData);
        }

        public OutputValues Login(string place, string userName, string workMode, string queue, string agentLoginId, string agentPassword)
        {
            logger.Debug("SoftPhone Login:");
            logger.Debug("*********************************************");
            logger.Debug("Place: " + place + Environment.NewLine + "UserName: " + userName + Environment.NewLine + "WorkMode: "
                + workMode + Environment.NewLine + "Queue: " + queue + Environment.NewLine + "AgentLoginId: " + agentLoginId);
            logger.Debug("*********************************************");
            var output = new OutputValues();
            //Input Validation
            CheckException.CheckLoginValues(place, userName);

            //Authenticate User
            Settings.GetInstance().QueueName = queue;

            CheckException.CheckDN(Settings.GetInstance().ACDPosition, Settings.GetInstance().ExtensionDN, Settings.GetInstance().PlaceName);
            EventRegistered evenReg = null;
            string fullName = "";
            if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
            {
                var message = RegisterDNRequest(Settings.GetInstance().ExtensionDN);
                if (message != null)
                {
                    switch (message.Id)
                    {
                        case EventRegistered.MessageId:
                            evenReg = message as EventRegistered;
                            string AID = string.IsNullOrEmpty(evenReg.AgentID) ? "" : evenReg.AgentID;
                            if (CheckPlcaeTaken(AID, out fullName))
                            {
                                if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                else
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ACDPosition);
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                output.MessageCode = "2004";
                                output.Message = "Place is already taken by " + fullName + ".";
                                return output;
                            }
                            AssignPreAgentStatus(evenReg);
                            AssignPreCallStatus(evenReg);
                            Settings.GetInstance().IsDNRegistered = true;
                            break;

                        case EventError.MessageId:
                            Settings.GetInstance().IsDNRegistered = false;
                            break;
                    }
                }
                Register(Settings.GetInstance().ExtensionDN);
            }
            else
            {
                var message = RegisterDNRequest(Settings.GetInstance().ACDPosition);
                if (message != null)
                {
                    switch (message.Id)
                    {
                        case EventRegistered.MessageId:
                            evenReg = message as EventRegistered;
                            string AID = string.IsNullOrEmpty(evenReg.AgentID) ? "" : evenReg.AgentID;
                            if (CheckPlcaeTaken(AID, out fullName))
                            {
                                if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                else
                                {
                                    Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ACDPosition);
                                    //Pointel.Softphone.Voice.Core.Request.RequestUnRegisterPlace.UnRegisterDN(Settings.GetInstance().ExtensionDN);
                                }
                                output.MessageCode = "2004";
                                output.Message = "Place is already taken by " + fullName + ".";
                                return output;
                            }
                            AssignPreAgentStatus(evenReg);
                            Settings.GetInstance().IsDNRegistered = true;
                            break;

                        case EventError.MessageId:
                            Settings.GetInstance().IsDNRegistered = false;
                            break;
                    }
                }
                IMessage message1 = RegisterDNRequest(Settings.GetInstance().ExtensionDN);
                if (message1 != null)
                {
                    switch (message1.Id)
                    {
                        case EventRegistered.MessageId:
                            evenReg = message as EventRegistered;
                            AssignPreCallStatus(evenReg);
                            Settings.GetInstance().IsDNRegistered = true;
                            break;

                        case EventError.MessageId:
                            Settings.GetInstance().IsDNRegistered = false;
                            break;
                    }
                }
                Register(Settings.GetInstance().ACDPosition);
                Register(Settings.GetInstance().ExtensionDN);
            }
            if (!Settings.GetInstance().ISAlreadyLogin && Settings.GetInstance().IsDNRegistered)
            {
                output = RequestLogin.LoginAgent(workMode, queue, agentLoginId, agentPassword);
            }
            else if (Settings.GetInstance().ISAlreadyLogin)
            {
                output.MessageCode = "201";
                output.Message = "Agent Login Success";
            }
            if (output.MessageCode == "200")
                output.Message = "Agent Login Success";

            return output;
        }

        public OutputValues Logout()
        {
            return RequestLogout.LogoutAgent();
        }

        public void MergeCall(string type)
        {
            try
            {
                RequestAgentMergeCall.MergeCall(type);
            }
            catch (Exception commonException)
            {
                logger.Error("Softphone_MergeCall:" + commonException.ToString());
            }
        }

        public void MuteTransfer(string OtherDN, string location, KeyValueCollection userData, KeyValueCollection reasons, KeyValueCollection extensions)
        {
            RequestAgentTransfer.MuteTransfer(OtherDN, location, userData, reasons, extensions);
            HoldingFlagStatus(PhoneFunctions.CompleteTransfer);
        }

        public OutputValues NotReady(string reason, string code, bool isSolicited = false)
        {
            return RequestNotReady.AgentNotReady(reason, code, isSolicited);
        }

        /// <summary>
        ///  This method used to make agent not ready ACW
        /// </summary>
        /// <returns></returns>
        public OutputValues NotReadyACW()
        {
            return RequestNotReady.AgentAfterCallWork();
        }

        public OutputValues Ready()
        {
            return RequestReadyAgent.AgentReady();
        }

        public void Redirect(string routeOtherDN)
        {
            CheckException.CheckDialValues(routeOtherDN);
            RequestAgentRedirectCall.Redirect(routeOtherDN);
        }

        /// <summary>
        /// Refines the place.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="placeName">Name of the place.</param>
        /// <param name="agentLoginID">The agent login identifier.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="agentPassword">The agent password.</param>
        /// <param name="workMode">The work mode.</param>
        /// <returns></returns>
        public OutputValues RefinePlace(string userName, string placeName, string agentLoginID, string queueName, string agentPassword, string workMode)
        {
            var output = OutputValues.GetInstance();
            var read = new ReadConfigObjects();
            try
            {
                Settings.GetInstance().PlaceName = placeName;
                output = read.ReadPlaceObject();
                if (output.MessageCode == "200")
                {
                    if (Settings.GetInstance().VoiceProtocol != null && Settings.GetInstance().VoiceProtocol.State == ChannelState.Opened)
                    {
                        output = Login(placeName, userName, workMode, queueName, agentLoginID, agentPassword);
                    }
                }
                return output;
            }
            catch (Exception generalException)
            {
                output.MessageCode = "2001";
                output.Message = "Error occurred in Softphone RefinePlace() : " + generalException.ToString();
                logger.Error("Softphone_RefinePlace:" + generalException.ToString());
                return output;
            }
        }

        public OutputValues Register(string dn)
        {
            return RequestRegisterPlace.RegisterDN(dn);
        }

        public IMessage RegisterDNRequest(string dn)
        {
            return RequestRegisterPlace.RequestRegisterDN(dn);
        }

        public void Release()
        {
            RequestAgentReleaseCall.Release();
        }

        /// <summary>
        /// Requests the logout agent.
        /// </summary>
        /// <returns></returns>
        public IMessage RequestLogoutAgent()
        {
            return RequestLogout.LogoutAgentRequest();
        }

        /// <summary>
        /// Requests the un register dn.
        /// </summary>
        /// <param name="dn">The dn.</param>
        /// <returns></returns>
        public IMessage RequestUnRegisterDN()
        {
            IMessage message = null;

            if (Settings.GetInstance().ACDPosition == Settings.GetInstance().ExtensionDN)
            {
                message = RequestUnRegisterPlace.UnRegisterDNRequest(Settings.GetInstance().ExtensionDN);
            }
            else
            {
                message = RequestUnRegisterPlace.UnRegisterDNRequest(Settings.GetInstance().ACDPosition);
                message = RequestUnRegisterPlace.UnRegisterDNRequest(Settings.GetInstance().ExtensionDN);
            }
            return message;
        }

        public void Retrieve()
        {
            RequestAgentRetrieve.Retrieve();
        }

        public void SingleStepConference(string otherDN)
        {
            RequestAgentConference.SingleStepConference(otherDN);
        }

        public void SingleStepConference(string otherDN, string location, KeyValueCollection userData)
        {
            RequestAgentConference.SingleStepConference(otherDN, location, userData);
        }

        public void Subscribe(ISoftphoneListener listener, SoftPhoneSubscriber list)
        {
            VoiceManager subscribe = VoiceManager.GetInstance();

            subscribe.Subscribe(listener, list);
        }

        public OutputValues UnRegister(string place)
        {
            return RequestUnRegisterPlace.UnRegisterDN(place); ;
        }

        /// <summary>
        /// Updates the ocs call data.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        public OutputValues UpdateOCSCallData(KeyValueCollection userData)
        {
            return RequestUpdateAttachData.DistributeUserEvent(userData);
        }

        public void UpdateUserData(Dictionary<string, string> data, string connectionID = "")
        {
            KeyValueCollection getUserdata = GetUpdateUserData(data);
            //Input Validation
            CheckException.CheckUserDataValues((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), (connectionID == string.Empty ? Settings.GetInstance().ConnectionID : connectionID), getUserdata);

            RequestUpdateAttachData.UpdateAttachData((Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                          Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)), (connectionID == string.Empty ? Settings.GetInstance().ConnectionID : connectionID), getUserdata);
        }

        private static KeyValueCollection GetUpdateUserData(Dictionary<string, string> UpdateUserData)
        {
            KeyValueCollection holdingUpdateUserData = new KeyValueCollection();
            foreach (string key in UpdateUserData.Keys)
            {
                if (!holdingUpdateUserData.ContainsKey(key))
                {
                    if (key == Settings.GetInstance().DispositionCollectionKey && !string.IsNullOrEmpty(UpdateUserData[Settings.GetInstance().DispositionCollectionKey]))
                    {
                        var dict = UpdateUserData[Settings.GetInstance().DispositionCollectionKey].Split(';').Select(s => s.Split(':')).ToDictionary(a => a[0].Trim().ToString(), a => a[1].Trim().ToString());
                        KeyValueCollection tempKC = new KeyValueCollection();
                        if (dict == null && dict.Count == 0) return null;
                        foreach (string key1 in dict.Keys)
                        {
                            tempKC.Add(key1, dict[key1]);
                        }
                        holdingUpdateUserData.Add(key, tempKC);
                    }
                    else
                        holdingUpdateUserData.Add(key, UpdateUserData[key].ToString());
                }
            }
            return holdingUpdateUserData;
        }

        private void AssignPreAgentStatus(EventRegistered eReg)
        {
            if (eReg != null && eReg.Extensions.ContainsKey("AgentStatus"))
            {
                int state = Convert.ToInt32(eReg.Extensions["AgentStatus"]);
                if (state > 0)
                {
                    Settings.GetInstance().ISAlreadyLogin = true;
                }
                else
                {
                    Settings.GetInstance().ISAlreadyLogin = false;
                }
            }
        }

        private void AssignPreCallStatus(EventRegistered eReg)
        {
            if (eReg != null && eReg.Extensions.Contains("status"))
            {
                int status = Convert.ToInt32(eReg.Extensions["status"]);
                if (status == 1)
                {
                    if (eReg.Extensions.ContainsKey("conn-1"))
                        Settings.GetInstance().ConnectionID = eReg.Extensions["conn-1"].ToString();
                    if (eReg.Extensions.ContainsKey("ps-1"))
                    {
                        Settings.GetInstance().PartyState = eReg.Extensions["ps-1"].ToString();
                    }
                }
            }
        }

        private bool CheckPlcaeTaken(string agentLoginId, out string Name)
        {
            Name = "";
            try
            {
                if (!string.IsNullOrEmpty(agentLoginId) && agentLoginId != Settings.GetInstance().AgentLoginID)
                {
                    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgSwitch cfgswich = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgSwitch>
                        (new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgSwitchQuery() { Name = Settings.GetInstance().Switch.Name });
                    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin agentLoginID =
                        ConfigContainer.Instance().ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgAgentLogin>
                        (new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentLoginQuery() { LoginCode = agentLoginId, SwitchDbid = cfgswich.DBID });
                    Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPerson person = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgPerson>(new Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgPersonQuery() { LoginDbid = agentLoginID.DBID, SwitchDbid = cfgswich.DBID });
                    Name = person.LastName + " " + person.FirstName;
                    return true;
                }
            }
            catch (Exception ex)
            { }
            return false;
        }

        private void HoldingFlagStatus(PhoneFunctions value)
        {
            switch (value)
            {
                case PhoneFunctions.InitiateTransfer:
                    Settings.GetInstance().IsEnableInitiateTransfer = true;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;

                case PhoneFunctions.IntiateConference:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = true;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;

                case PhoneFunctions.CompleteTransfer:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = true;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    //Below condition added to re-solve issue REG - PHN 16
                    //04-15-2013 Ram
                    Settings.GetInstance().IsCustomerReleaseTransferCallAfterEstablish = false;
                    Settings.GetInstance().IsCustomerReleaseTransferCallBeforeEstablish = false;
                    //End
                    break;

                case PhoneFunctions.CompleteConference:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = true;
                    //Below condition added to re-solve issue REG - PHN 19
                    //04-17-2013 Ram
                    Settings.GetInstance().IsCustomerReleaseConfCallAfterEstablish = false;
                    Settings.GetInstance().IsCustomerReleaseConfCallBeforeEstablish = false;
                    //End
                    break;

                default:
                    Settings.GetInstance().IsEnableInitiateTransfer = false;
                    Settings.GetInstance().IsEnableCompleteTransfer = false;
                    Settings.GetInstance().IsEnableInitiateConference = false;
                    Settings.GetInstance().IsEnableCompleteConference = false;
                    break;
            }
        }

        #endregion Methods

        #region Other

        //Below methods added for register and unregister broadcast DN only
        //Smoorthy - 08-01-2014
        /// <summary>
        /// Registers the specified place.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <returns></returns>
        /// <summary>
        /// Un the register.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <returns></returns>
        /// <summary>
        /// This method used to register and login agent with TServer.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="workMode">The work mode.</param>
        /// <param name="queue">The queue.</param>
        /// <returns>OutputValues</returns>
        /// <summary>
        /// This method used to Initialize TServer.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="workMode">The work mode.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="configObject">The configuration object.</param>
        /// <param name="tServerApplicationName">Name of the application.</param>
        /// <param name="agentLoginId">The agent login unique identifier.</param>
        /// <param name="agentPassword">The agent password.</param>
        /// <returns>
        /// OutputValues
        /// </returns>
        /// <summary>
        /// This method used to logout agent from TServer
        /// </summary>
        /// <summary>
        /// This method used to make agent ready to accept voice calls
        /// </summary>
        /// <summary>
        /// This method used to make agent not ready on switch to not accept voice calls
        /// </summary>
        /// <summary>
        /// DNDs the configuration.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// DNDs the configuration.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// This method used to dial outbound call
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        /// <summary>
        /// This method used to answer inbound/consult/internal calls
        /// </summary>
        /// <summary>
        /// This method used to release inbound/internal/consult/outbound calls
        /// </summary>
        /// <summary>
        /// Redirects the specified route other dn.
        /// </summary>
        /// <param name="routeOtherDN">The route other dn.</param>
        /// <summary>
        /// This method used to hold inbound/internal/consult/outbound calls
        /// </summary>
        /// <summary>
        /// This method used to retrieve inbound/internal/consult/outbound calls
        /// </summary>
        /// <summary>
        /// This method used to initiate 2-step transfer calls
        /// </summary>
        /// <param name="number">Transfer Number</param>
        /// <summary>
        /// This method used to complete transfer call accept by the third party agent
        /// </summary>
        /// <summary>
        /// This method used to cancel before or after initiated transfer call accept by the third party agent and
        /// connect agent with customer call
        /// </summary>
        /// <summary>
        /// This method used to initiate 2-step conference calls
        /// </summary>
        /// <param name="number">Conference Number</param>
        /// <summary>
        /// This method used to complete conference call accept by the third party agent
        /// </summary>
        /// <summary>
        /// Singles the step conference.
        /// </summary>
        /// <param name="otherDN">The other dn.</param>
        /// <summary>
        /// This  method used to cancel before or after initiated conference call accept by the third party agent and
        /// connect agent with customer call
        /// </summary>
        /// <summary>
        /// Forwards the call set.
        /// </summary>
        /// <param name="otherDN">The other dn.</param>
        /// <summary>
        /// Forwards the call cancel.
        /// </summary>
        /// <summary>
        /// Gets the call status.
        /// </summary>
        //Code Added For getting Call status
        //SMoorthy - 30-05-2014
        //End
        /// <summary>
        /// This method used to modify/delete active call user data
        /// </summary>
        /// <param name="data"></param>
        /// <summary>
        /// Distributes the user data event.
        /// </summary>
        /// <param name="UserData">The user data.</param>
        /// <summary>
        /// This Subscribe method used to deliver incoming call user data and current call control status on UI
        /// </summary>
        /// <param name="listener">Soft phone listener which gives control to subscriber to receive user data and control status</param>
        /// <summary>
        /// This method used to handled Holdings the flag status.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <summary>
        /// This method used to Convert Userdata from dictionary to KeyValueCollection
        /// </summary>
        /// <param name="value">The UpdateUserData.</param>
        /// <summary>
        /// Alternates the call.
        /// </summary>
        /// <param name="thisDN">The this dn.</param>
        /// <summary>
        /// Merges the call.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <summary>
        /// Deletes the conference.
        /// </summary>
        /// <param name="OtherDN">The other dn.</param>
        /// <summary>
        /// DTMFs the send.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <summary>
        /// Mutes the transfer[Single Step Transfer].
        /// </summary>
        /// <param name="OtherDN">The other dn.</param>

        #endregion Other
    }
}