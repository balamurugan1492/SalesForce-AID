/*
  Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using Genesyslab.Platform.Commons.Collections;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Voice
{
    internal class VoiceMakeOutbound
    {
        #region Field Declaration

        private Log logger;
        private static VoiceMakeOutbound makeOutbound = null;
        private VoiceMakeConsultation makeconsultation = null;

        #endregion Field Declaration

        #region constructor

        private VoiceMakeOutbound()
        {
            this.logger = Log.GenInstance();
            makeconsultation = VoiceMakeConsultation.GetInstance();
        }

        #endregion constructor

        #region GetInstacnce

        public static VoiceMakeOutbound GetInstance()
        {
            if (makeOutbound == null)
            {
                makeOutbound = new VoiceMakeOutbound();
            }
            return makeOutbound;
        }

        #endregion GetInstacnce

        #region Make Outbound/Consult Call

        public void MakeVoiceCall(string phoneNumber, string recordType, string recordId)
        {
            try
            {
                this.logger.Info("MakeVoiceCall : Dialing outbound/consult for PhoneNumber :" + phoneNumber);

                if (Settings.AgentDetails.IsAgentOnCall > 0)
                {
                    try
                    {
                        this.logger.Info("Agent is on Call - Adapter initiating Consult call");
                        if (Settings.SFDCOptions.EnableConsultDialingFromSFDC)
                        {
                            if (Settings.SFDCOptions.ConsultVoiceDialPlanPrefix != null)
                                phoneNumber = Settings.SFDCOptions.ConsultVoiceDialPlanPrefix + phoneNumber;
                            else
                                this.logger.Info("Consult Dial Plan Prefix is Empty");

                            makeconsultation.DialVoiceConsultation(phoneNumber, recordType, recordId);
                        }
                        else
                        {
                            this.logger.Error("MakeVoiceCall : Dialing Consult Call From Adapter is disabled ");
                        }
                    }
                    catch (Exception generalException)
                    {
                        this.logger.Error("MakeVoiceCall : Error Occurred while making call : " + generalException.Message);
                    }
                }
                else
                {
                    try
                    {
                        this.logger.Info("MakeVoiceCall : Agent is not on Call");
                        if (Settings.AgentDetails.TServer != null)
                        {
                            try
                            {
                                if (Settings.AgentDetails.AgentVoiceMediaStatus == CurrentAgentStatus.Ready || (Settings.SFDCOptions.EnableClickToDialOnNotReady && (Settings.AgentDetails.AgentVoiceMediaStatus == CurrentAgentStatus.NotReady ||
                                    Settings.AgentDetails.AgentVoiceMediaStatus == CurrentAgentStatus.NotReadyActionCode ||
                                    Settings.AgentDetails.AgentVoiceMediaStatus == CurrentAgentStatus.NotReadyAfterCallWork)))
                                {
                                    MakeOutBoundCall(phoneNumber, recordType, recordId);
                                }
                                else
                                {
                                    this.logger.Info("MakeVoiceCall : Could not make outbound call for the number : " + phoneNumber);
                                    this.logger.Info("MakeVoiceCall : Agent Current Status : " + Settings.AgentDetails.AgentVoiceMediaStatus.ToString());
                                    this.logger.Info("MakeVoiceCall : Is Adapter configured to dial Outbound call on Not Ready ? : " + Settings.SFDCOptions.EnableClickToDialOnNotReady.ToString());
                                }
                            }
                            catch (Exception generalError)
                            {
                                this.logger.Error("MakeVoiceCall : Error Occurred while making outboud call " + generalError.ToString());
                            }
                        }
                        else
                        {
                            this.logger.Info("MakeVoiceCall : Can not make outbound call for the number : " + phoneNumber + "  because the voice media is not available for the agent ");
                        }
                    }
                    catch (System.Exception generalException)
                    {
                        this.logger.Error("MakeVoiceCall : Error occured : " + generalException.ToString());
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("MakeVoiceCall : Error occured while passing user data " + generalException.ToString());
            }
        }

        #endregion Make Outbound/Consult Call

        #region Make OutboundCall

        private void MakeOutBoundCall(string phoneNumber, string recordType, string recordId)
        {
            try
            {
                this.logger.Info("MakeOutBoundCall : Dialing Outbound Call from Adapter....");
                if (Settings.AgentDetails.TServer != null && Settings.AgentDetails.TServer.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    if (Settings.SFDCOptions.OutboundVoiceDialPlanPrefix != null)
                        phoneNumber = Settings.SFDCOptions.OutboundVoiceDialPlanPrefix + phoneNumber;
                    else
                        this.logger.Info("MakeOutBoundCall : Outbound Dial Plan Prefix is Empty");

                    var requestMakeCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestMakeCall.Create();
                    requestMakeCall.ThisDN = Settings.AgentDetails.ThisDN;
                    requestMakeCall.OtherDN = phoneNumber;
                    requestMakeCall.MakeCallType = Genesyslab.Platform.Voice.Protocols.TServer.MakeCallType.Regular;
                    KeyValueCollection reason = new KeyValueCollection();
                    reason.Add("ClickToDial", phoneNumber + "," + recordType + "," + recordId);
                    requestMakeCall.Reasons = reason;
                    Settings.AgentDetails.TServer.Send(requestMakeCall);
                }
                else
                {
                    this.logger.Info("MakeOutBoundCall : Can not make Outbound Call because T Server Protocol null or Channel closed ");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("MakeOutBoundCall : Error occured while making outbound call : " + generalException.ToString());
            }
        }

        #endregion Make OutboundCall
    }
}