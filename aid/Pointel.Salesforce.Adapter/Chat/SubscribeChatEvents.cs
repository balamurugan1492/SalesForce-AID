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

using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.Utility;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Pointel.Salesforce.Adapter.Chat
{
    internal class SubscribeChatEvents
    {
        #region Field Declaration

        /// <summary>
        /// Fields for the class SubscribeChatEvents
        /// </summary>
        private Log logger = null;

        private EventSessionInfo sessionInfo = null;
        private EventInvite eventInvite = null;
        private readonly object _lockObject = new object();
        private IDictionary<string, DateTime> CallDurationData = new Dictionary<string, DateTime>();
        public IDictionary<string, string> FinishedCallDuration = new Dictionary<string, string>();
        private string LastSessionId = string.Empty;
        private string callDuration = "00 hr 00 min 00 sec";
        private XmlNodeList newParty = null;
        private XmlDocument xml = new XmlDocument();
        private XmlNodeList RootNode = null;
        private IDictionary<string, EventInvite> ChatIXNCollection = new Dictionary<string, EventInvite>();
        private bool IsConsultReceived = false;

        #endregion Field Declaration

        #region SubscribeAgentChatEvents

        /// <summary>
        /// Constructor of the Class SubscribeChatEvents
        /// </summary>
        public SubscribeChatEvents()
        {
            try
            {
                logger = Log.GenInstance();
            }
            catch
            {
                logger.Error("SubscribeChatEvents : Error occoured at Constructor");
            }
        }

        #endregion SubscribeAgentChatEvents

        #region RecieveChatEvents

        /// <summary>
        ///  Receives Chat Events
        /// </summary>
        /// <param name="events"></param>
        public void ReceiveChatEvents(IMessage events)
        {
            try
            {
                if (Settings.SFDCOptions.SFDCPopupPages != null)
                {
                    if (events != null)
                    {
                        switch (events.Id)
                        {
                            case EventInvite.MessageId:
                                eventInvite = (EventInvite)events;
                                if (eventInvite.Interaction.InteractionType == "Inbound")
                                {
                                    if (eventInvite.VisibilityMode.ToString() != "Coach" && eventInvite.VisibilityMode.ToString() != "Conference")
                                    {
                                        IsConsultReceived = false;
                                        InboundChatInvite.GetInstance().PopupRecords(eventInvite, SFDCCallType.InboundChat);
                                    }
                                    else
                                    {
                                        IsConsultReceived = true;
                                        InboundChatInvite.GetInstance().PopupRecords(eventInvite, SFDCCallType.ConsultChatReceived);
                                    }
                                }
                                else if (eventInvite.Interaction.InteractionType == "Consult")
                                {
                                    IsConsultReceived = true;
                                    InboundChatInvite.GetInstance().PopupRecords(eventInvite, SFDCCallType.ConsultChatReceived);
                                }
                                break;

                            case EventSessionInfo.MessageId:
                                sessionInfo = (EventSessionInfo)events;
                                try
                                {
                                    if (LastSessionId != eventInvite.Interaction.InteractionId)
                                    {
                                        LastSessionId = eventInvite.Interaction.InteractionId;

                                        if (!ChatIXNCollection.ContainsKey(eventInvite.Interaction.InteractionId))
                                        {
                                            ChatIXNCollection.Add(eventInvite.Interaction.InteractionId, eventInvite);
                                        }

                                        if (!CallDurationData.ContainsKey(eventInvite.Interaction.InteractionId))
                                        {
                                            CallDurationData.Add(eventInvite.Interaction.InteractionId, System.DateTime.Now);
                                        }
                                        ChatSessionConnected.GetInstance().PopupRecords(eventInvite, SFDCCallType.InboundChat);
                                    }
                                    if (sessionInfo.SessionStatus == Genesyslab.Platform.WebMedia.Protocols.BasicChat.SessionStatus.Over)
                                    {
                                        if (CallDurationData.ContainsKey(sessionInfo.ChatTranscript.SessionId))
                                        {
                                            TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[sessionInfo.ChatTranscript.SessionId]);
                                            callDuration = ts.Hours + "Hr " + ts.Minutes + " mins " + ts.Seconds + "secs";
                                            if (!FinishedCallDuration.ContainsKey(sessionInfo.ChatTranscript.SessionId))
                                            {
                                                FinishedCallDuration.Add(sessionInfo.ChatTranscript.SessionId, callDuration);
                                            }
                                            else
                                            {
                                                FinishedCallDuration[sessionInfo.ChatTranscript.SessionId] = callDuration;
                                            }
                                        }
                                        if (IsConsultReceived)
                                            ChatSessionEnded.GetInstance().PopupRecords(eventInvite, SFDCCallType.ConsultChatReceived, callDuration, GetChatInteractionContent(sessionInfo.ChatTranscript.SessionId).Replace('&', ' '));
                                        else
                                            ChatSessionEnded.GetInstance().PopupRecords(eventInvite, SFDCCallType.InboundChat, callDuration, GetChatInteractionContent(sessionInfo.ChatTranscript.SessionId).Replace('&', ' '));
                                    }
                                }
                                catch (Exception generalException)
                                {
                                    this.logger.Error(generalException.ToString());
                                }
                                break;

                            default:
                                logger.Info("Unhandled Event " + events.Name);
                                break;
                        }
                    }
                }
                else
                {
                    logger.Error("ReceiveChatEvents : SFDC Popup disabled since the popup Objects not configured....");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReceiveChatEvents : Error occured while receiving Chat events " + generalException.ToString());
            }
        }

        #endregion RecieveChatEvents

        #region GetChatInteractionContent

        /// <summary>
        /// Gets Chat interaction Content
        /// </summary>
        /// <param name="ixnId"></param>
        /// <returns></returns>
        private string GetChatInteractionContent(string ixnId)
        {
            try
            {
                this.logger.Info("GetChatInteractionContent : Retrieving Chat Content for the IXN Id : " + ixnId);

                if (Settings.AgentDetails.UCSServer != null)
                {
                    RequestGetInteractionContent ucsRequest = RequestGetInteractionContent.Create();
                    ucsRequest.InteractionId = ixnId;
                    IMessage ucsResponse = Settings.AgentDetails.UCSServer.Request(ucsRequest);
                    if (ucsResponse is Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventError)
                    {
                        this.logger.Error("GetChatInteractionContent : Event Error Returend while requesting Chat Interaction Data with Interaction Id : " + ixnId + "\n Error Message :" + ucsResponse.ToString());
                    }
                    else
                    {
                        EventGetInteractionContent chatContent = ucsResponse as EventGetInteractionContent;
                        if (chatContent != null)
                        {
                            if (!String.IsNullOrEmpty(chatContent.InteractionContent.StructuredText))
                                return GetFormattedChatData(chatContent.InteractionContent.StructuredText, ixnId);
                        }
                    }
                }
                else
                    this.logger.Error("GetChatInteractionContent : Could not retrieve chat content because UCS Protocol is null.");
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetChatInteractionContent : Error occurred at :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetChatInteractionContent

        #region GetFormattedChatData

        /// <summary>
        /// Gets Formatted Chat data
        /// </summary>
        /// <param name="xmldata"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private string GetFormattedChatData(string xmldata, string sessionId)
        {
            try
            {
                logger.Info("GetFormattedChatData : Formatting the Chat Content : " + xmldata);
                xml.LoadXml(xmldata);
                RootNode = xml.GetElementsByTagName("chatTranscript");
                newParty = xml.GetElementsByTagName("newParty");
                IDictionary<string, string> userInfo = new Dictionary<string, string>();
                string ChatMessage = string.Empty;

                if (RootNode != null)
                {
                    DateTime chatStartTime = Convert.ToDateTime(RootNode[0].Attributes["startAt"].Value);
                    XmlNodeList nodes = RootNode[0].ChildNodes;
                    foreach (XmlNode node in nodes)
                    {
                        if (node.Name == "newParty")
                        {
                            foreach (XmlNode party in node.ChildNodes)
                            {
                                if (party.Name == "userInfo")
                                {
                                    if (party.Attributes["userType"].Value == "CLIENT")
                                    {
                                        userInfo.Add(node.Attributes["userId"].Value, "CLIENT_" + party.Attributes["userNick"].Value);
                                        ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : New Party " + userInfo[node.Attributes["userId"].Value] + " has joined the session.\n";
                                    }
                                    else if (party.Attributes["userType"].Value == "AGENT")
                                    {
                                        userInfo.Add(node.Attributes["userId"].Value, "AGENT_" + party.Attributes["userNick"].Value);
                                        ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : New Party " + userInfo[node.Attributes["userId"].Value] + "  has joined the session.\n";
                                    }
                                }
                            }
                        }
                        else if (node.Name == "message")
                        {
                            ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] :" + userInfo[node.Attributes["userId"].Value] + "  : " + node.FirstChild.InnerText + "\n";
                        }
                        else if (node.Name == "partyLeft")
                        {
                            ChatMessage += "[" + chatStartTime.AddSeconds(int.Parse(node.Attributes["timeShift"].Value)).ToString("hh:mm:ss tt") + "] : Party " + userInfo[node.Attributes["userId"].Value] + "   has left the session. " + node.FirstChild.InnerText + "\n";
                        }
                    }
                }
                else
                {
                    this.logger.Error("GetFormattedChatData : ChatTranscript is null.");
                }

                return ChatMessage;
            }
            catch (Exception generalException)
            {
                this.logger.Error("GetFormattedChatData : Error occurred :" + generalException.ToString());
            }
            return string.Empty;
        }

        #endregion GetFormattedChatData

        #region UpdateDispositionCode

        /// <summary>
        /// Gets Updated Disposition Code Change
        /// </summary>
        /// <param name="ixnId"></param>
        /// <param name="key"></param>
        /// <param name="code"></param>
        public void UpdateDispositionCodeChange(string ixnId, string key, string code)
        {
            try
            {
                logger.Info("Updating activity log");
                if (ChatIXNCollection.ContainsKey(ixnId))
                {
                    if (key.Contains("."))
                    {
                        key = key.Substring(key.LastIndexOf("."));
                    }
                    EventInvite updateData = ChatIXNCollection[ixnId];
                    if (updateData != null)
                    {
                        if (updateData.Interaction.InteractionUserData.ContainsKey(key))
                        {
                            updateData.Interaction.InteractionUserData[key] = code;
                        }
                        else
                        {
                            updateData.Interaction.InteractionUserData.Add(key, code);
                        }
                        if (CallDurationData.ContainsKey(updateData.Interaction.InteractionId))
                        {
                            TimeSpan ts = System.DateTime.Now.Subtract(CallDurationData[updateData.Interaction.InteractionId]);
                            callDuration = ts.Hours + "Hr " + ts.Minutes + " mins " + ts.Seconds + "secs";
                        }
                        InboundChatInvite.GetInstance().UpdateRecords(eventInvite, SFDCCallType.InboundChat, callDuration, GetChatInteractionContent(updateData.Interaction.InteractionId).Replace('&', ' '));
                    }
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("UpdateDispositionCodeChange : Error at updating disposition code Changed : " + generalException.ToString());
            }
        }

        #endregion UpdateDispositionCode
    }
}