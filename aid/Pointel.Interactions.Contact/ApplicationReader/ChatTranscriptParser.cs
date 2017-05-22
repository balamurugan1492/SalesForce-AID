using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Settings;

namespace Pointel.Interactions.Contact.ApplicationReader
{
    internal class ChatTranscriptParser
    {
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private string userNick = string.Empty;
        private int timeShift = 0;
        private string userId = string.Empty;
        private string message = string.Empty;
        private Hashtable agentInfo = new Hashtable();
        private DateTime _sessionStartedTime = new DateTime();
        private ContactDataContext emailUtility = ContactDataContext.GetInstance();
        Run commonRun;
        Paragraph typeNotifierParagraph = new Paragraph();
        private Dictionary<string, KeyValuePair<string, string>> chatParties = new Dictionary<string, KeyValuePair<string, string>>();

        /// <summary>
        /// Gets the chat session.
        /// </summary>
        /// <param name="document">The document.</param>
        public void GetChatSession(XmlDocument chatTranscript, FlowDocument rtbDocument)
        {
            string userType = string.Empty;
            try
            {
                if (chatTranscript != null)
                {
                    _sessionStartedTime = DateTime.ParseExact(chatTranscript.DocumentElement.GetAttribute("startAt").ToString(), "yyyy-MM-ddTHH:mm:ssZ", null).ToUniversalTime();
                }
                if (rtbDocument != null && rtbDocument.Blocks != null)
                    rtbDocument.Blocks.Clear();
                IEnumerable<XElement> childList = from element in XElement.Parse(chatTranscript.OuterXml).Elements() select element;
                foreach (XElement child in childList)
                {
                    switch (child.Name.ToString())
                    {
                        case "message":
                            if (child.HasAttributes)
                            {
                                userId = (child.Attribute("userId").Value == null ? string.Empty : child.Attribute("userId").Value);
                                timeShift = child.Attribute("timeShift").Value == null ? 0 : Convert.ToInt32(child.Attribute("timeShift").Value);
                                if (child.HasElements)
                                {
                                    var msg = child.Element("msgText");
                                    if (msg != null)
                                        message = msg.Value;
                                    else
                                        message = string.Empty;
                                    if (chatParties.ContainsKey(userId))
                                    {
                                        var data = chatParties[userId];
                                        userNick = data.Key;
                                        userType = data.Value;
                                    }
                                    chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift) + " " + userNick + ": ", child.Name.ToString(), message, "", rtbDocument);
                                }
                            }
                            break;
                        case "notice":
                            if (child.HasAttributes)
                            {
                                userId = (child.Attribute("userId").Value == null ? string.Empty : child.Attribute("userId").Value);
                                timeShift = (child.Attribute("timeShift").Value == null ? 0 : Convert.ToInt32(child.Attribute("timeShift").Value));
                                message = child.ToString();
                                if (child.HasElements)
                                {
                                    if (chatParties.ContainsKey(userId))
                                    {
                                        var data = chatParties[userId];
                                        userNick = data.Key;
                                        userType = data.Value;
                                    }
                                    var notice = child.Element("noticeText");
                                    if (notice != null)
                                    {
                                        message = notice.Value;
                                        chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift) + " " + userNick + ": ", child.Name.ToString(), " Pushed the URL ", message, rtbDocument);
                                    }
                                    var msg = child.Element("msgText");
                                    if (msg != null)
                                    {
                                        message = msg.Value;
                                        chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift) + " " + userNick + ": ", child.Name.ToString(), message, "", rtbDocument);
                                    }
                                }
                            }
                            break;
                        case "newParty":
                            if (child.HasAttributes)
                            {
                                userId = (child.Attribute("userId").Value == null ? string.Empty : child.Attribute("userId").Value);
                                if (child.HasElements)
                                {
                                    var user = child.Element("userInfo");
                                    if (user != null)
                                    {
                                        timeShift = (user.Attribute("timeZoneOffset").Value == null ? 0 : Convert.ToInt32(user.Attribute("timeZoneOffset").Value));
                                        userNick = (user.Attribute("userNick").Value == null ? string.Empty : user.Attribute("userNick").Value);
                                        userType = (user.Attribute("userType").Value == null ? string.Empty : user.Attribute("userType").Value);
                                        string personId = (user.Attribute("personId").Value == null ? string.Empty : user.Attribute("personId").Value);
                                        if (!chatParties.ContainsKey(userId))
                                            chatParties.Add(userId, new KeyValuePair<string, string>(userNick, userType));
                                        chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift), child.Name.ToString(), " New party '" + userNick + "' has joined the session", string.Empty, rtbDocument);
                                    }
                                }
                            }
                            break;
                        case "partyLeft":
                            if (child.HasAttributes)
                            {
                                timeShift = (child.Attribute("timeShift").Value == null ? 0 : Convert.ToInt32(child.Attribute("timeShift").Value));
                                userId = (child.Attribute("userId").Value == null ? string.Empty : child.Attribute("userId").Value);
                                if (chatParties.ContainsKey(userId))
                                {
                                    var data = chatParties[userId];
                                    userNick = data.Key;
                                    userType = data.Value;
                                }
                                chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift), child.Name.ToString(), " Party '" + userNick + "' has left the session", string.Empty, rtbDocument);
                            }
                            break;
                    }
                }

                #region Old Code
                //XmlElement xmlElement = chatTranscript.DocumentElement;
                //if (xmlElement.Attributes != null)
                //{
                //    _sessionStartedTime = DateTime.ParseExact(xmlElement.GetAttribute("startAt").ToString(), "yyyy-MM-ddTHH:mm:ssZ", null).ToUniversalTime();
                //}
                //XmlNodeList lstNodes = chatTranscript.DocumentElement.ChildNodes;
                //foreach (XmlNode childNode in lstNodes)
                //{
                //    switch (childNode.Name.ToString())
                //    {
                //        case "message":
                //            XmlNodeList messageList = xmlElement.GetElementsByTagName("message");
                //            if (messageList != null)
                //            {
                //                //for (int i = 0; i < messageList.Count; i++)
                //                //{
                //                XmlElement element = (XmlElement)messageList.Item(0);
                //                userId = element.GetAttribute("userId");
                //                timeShift = Convert.ToInt32(element.GetAttribute("timeShift"));
                //                message = element.InnerText.ToString();
                //                if (chatParties.ContainsKey(userId))
                //                {
                //                    var data = chatParties[userId];
                //                    userNick = data.Key;
                //                    userType = data.Value;
                //                }
                //                chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift) + " " + userNick + ": ", childNode.Name, message, "");
                //                //}
                //            }
                //            break;
                //        case "notice":
                //            XmlNodeList noticeList = xmlElement.GetElementsByTagName("notice");
                //            if (noticeList != null)
                //            {
                //                //for (int i = 0; i < noticeList.Count; i++)
                //                //{
                //                XmlElement element = (XmlElement)noticeList.Item(0);
                //                userId = element.GetAttribute("userId");
                //                timeShift = Convert.ToInt32(element.GetAttribute("timeShift"));
                //                message = element.InnerText.ToString();
                //                if (chatParties.ContainsKey(userId))
                //                {
                //                    var data = chatParties[userId];
                //                    userNick = data.Key;
                //                    userType = data.Value;
                //                }
                //                chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift) + " " + userNick + ": ", childNode.Name, " Pushed the URL ", message);
                //                //}
                //            }
                //            break;
                //        case "newParty":
                //            XmlNodeList newParty = xmlElement.GetElementsByTagName("newParty");
                //            if (newParty != null)
                //            {
                //                for (int i = 0; i < newParty.Count; i++)
                //                {
                //                    XmlElement element = (XmlElement)newParty.Item(i);
                //                    userId = element.GetAttribute("userId");
                //                    XmlNodeList userInfo = element.GetElementsByTagName("userInfo");
                //                    if (userInfo != null)
                //                    {
                //                        //for (int k = 0; k < userInfo.Count; k++)
                //                        //{
                //                        XmlElement user = (XmlElement)userInfo.Item(0);
                //                        userNick = user.GetAttribute("userNick");
                //                        userType = user.GetAttribute("userType");
                //                        string personId = user.GetAttribute("personId");
                //                        if (userType.ToLower() == "agent" && personId != ContactDataContext.GetInstance().AgentID)
                //                            userType = "OtherAgent";
                //                        timeShift = Convert.ToInt32(user.GetAttribute("timeZoneOffset"));
                //                        if (!chatParties.ContainsKey(userId))
                //                            chatParties.Add(userId, new KeyValuePair<string, string>(userNick, userType));
                //                        //}
                //                        chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift), childNode.Name, " New party '" + userNick + "' has joined the session", string.Empty);
                //                    }
                //                }
                //            }
                //            break;
                //        case "partyLeft":
                //            XmlNodeList partyLeft = xmlElement.GetElementsByTagName("partyLeft");
                //            if (partyLeft != null)
                //            {
                //                //for (int i = 0; i < partyLeft.Count; i++)
                //                //{
                //                XmlElement element = (XmlElement)partyLeft.Item(0);
                //                timeShift = Convert.ToInt32(element.GetAttribute("timeShift"));
                //                userId = element.GetAttribute("userId");
                //                if (chatParties.ContainsKey(userId))
                //                {
                //                    var data = chatParties[userId];
                //                    userNick = data.Key;
                //                    userType = data.Value;
                //                }
                //                chatConversationBinding(userType, getCurrentTimewithSpecFormat(timeShift), childNode.Name, " Party '" + userNick + "' has left the session", string.Empty);
                //                //}
                //            }
                //            break;
                //        default:
                //            break;
                //    }
                //}
                #endregion
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred GetChatSession()" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the current timewith spec format.
        /// </summary>
        /// <param name="timeShift">The time shift.</param>
        /// <returns></returns>
        private string getCurrentTimewithSpecFormat(int timeShift)
        {
            try
            {
                string format = ConfigContainer.Instance().AllKeys.Contains("chat.time-stamp-format") ? (string)ConfigContainer.Instance().GetValue("chat.time-stamp-format") : string.Empty;
                //ContactDataContext.GetInstance().ChatTimeStampFormat;
                string result = string.Empty;
                DateTime time = _sessionStartedTime.AddSeconds(timeShift);
                switch (format)
                {
                    case "hh:mm":
                        result = "[" + time.ToString("hh:mm") + "]";
                        break;
                    case "hh:mm tt":
                        result = "[" + time.ToString("hh:mm tt") + "]";
                        break;
                    case "hh:mm:ss":
                        result = "[" + time.ToString("hh:mm:ss") + "]";
                        break;
                    case "hh:mm:ss tt":
                        result = "[" + time.ToString("hh:mm:ss tt") + "]";
                        break;
                    default:
                        result = "[" + time.ToString("hh:mm:ss tt") + "]";
                        break;
                }
                return result;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while getCurrentTimewithSpecFormat() : " + generalException.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Chats the conversation binding.
        /// </summary>
        /// <param name="chatUserType">Type of the chat user.</param>
        /// <param name="userNameWithTimeShift">The user name with time shift.</param>
        /// <param name="infoType">Type of the information.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="noticeText">The notice text.</param>
        private void chatConversationBinding(string chatUserType, string userNameWithTimeShift, string infoType, string messageText, string noticeText, FlowDocument rtbDocument)
        {
            Paragraph normalMessageParagraph = new Paragraph();
            Paragraph urlMessageParagraph = new Paragraph();
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    switch (infoType)
                    {
                        case "message":
                            switch (chatUserType.ToLower())
                            {
                                case "agent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }

                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    //if (ContactDataContext.GetInstance().AgentTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().AgentTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().AgentTextColor.A, ContactDataContext.GetInstance().AgentTextColor.R, ContactDataContext.GetInstance().AgentTextColor.G, ContactDataContext.GetInstance().AgentTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    break;
                                case "client":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(
                                                ((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"))).Name.ToString()));

                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    //if (ContactDataContext.GetInstance().ClientPromptColor.ToString().Contains("#"))
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientPromptColor.Name.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientPromptColor.A, ContactDataContext.GetInstance().ClientPromptColor.R, ContactDataContext.GetInstance().ClientPromptColor.G, ContactDataContext.GetInstance().ClientPromptColor.B));
                                    //}
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    //if (ContactDataContext.GetInstance().ClientTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientTextColor.A, ContactDataContext.GetInstance().ClientTextColor.R, ContactDataContext.GetInstance().ClientTextColor.G, ContactDataContext.GetInstance().ClientTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    break;
                                case "otheragent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    //if (ContactDataContext.GetInstance().OtherAgentPromptColor.ToString().Contains("#"))
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentPromptColor.Name.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentPromptColor.A, ContactDataContext.GetInstance().OtherAgentPromptColor.R, ContactDataContext.GetInstance().OtherAgentPromptColor.G, ContactDataContext.GetInstance().OtherAgentPromptColor.B));
                                    //}
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }

                                    //if (ContactDataContext.GetInstance().OtherAgentTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentTextColor.A, ContactDataContext.GetInstance().OtherAgentTextColor.R, ContactDataContext.GetInstance().OtherAgentTextColor.G, ContactDataContext.GetInstance().OtherAgentTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            rtbDocument.Blocks.Add(normalMessageParagraph);

                            break;
                        case "newParty":
                            switch (chatUserType.ToLower())
                            {
                                case "agent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }

                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }

                                    break;
                                case "client":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(
                                                ((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"))).Name.ToString()));

                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    //if (ContactDataContext.GetInstance().ClientPromptColor.ToString().Contains("#"))
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientPromptColor.Name.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientPromptColor.A, ContactDataContext.GetInstance().ClientPromptColor.R, ContactDataContext.GetInstance().ClientPromptColor.G, ContactDataContext.GetInstance().ClientPromptColor.B));
                                    //}
                                    //if (ContactDataContext.GetInstance().ClientTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientTextColor.A, ContactDataContext.GetInstance().ClientTextColor.R, ContactDataContext.GetInstance().ClientTextColor.G, ContactDataContext.GetInstance().ClientTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    break;
                                case "otheragent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    //if (ContactDataContext.GetInstance().OtherAgentPromptColor.ToString().Contains("#"))
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentPromptColor.Name.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentPromptColor.A, ContactDataContext.GetInstance().OtherAgentPromptColor.R, ContactDataContext.GetInstance().OtherAgentPromptColor.G, ContactDataContext.GetInstance().OtherAgentPromptColor.B));
                                    //}
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    //if (ContactDataContext.GetInstance().OtherAgentTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentTextColor.A, ContactDataContext.GetInstance().OtherAgentTextColor.R, ContactDataContext.GetInstance().OtherAgentTextColor.G, ContactDataContext.GetInstance().OtherAgentTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            rtbDocument.Blocks.Add(normalMessageParagraph);

                            break;
                        case "notice":
                            switch (chatUserType.ToLower())
                            {
                                case "agent":
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")));
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")).Contains("#"))
                                            {
                                                normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                                normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }

                                        DetectURLs(normalMessageParagraph);
                                        rtbDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")));
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }

                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")).Contains("#"))
                                            {
                                                normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                                normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        temprun = new Run(noticeText);
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        rtbDocument.Blocks.Add(urlMessageParagraph);

                                    }
                                    break;
                                case "client":
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {
                                        typeNotifierParagraph.Inlines.Clear();
                                        typeNotifierParagraph.FontStyle = FontStyles.Italic;
                                        typeNotifierParagraph.Inlines.Add(new Run(userNameWithTimeShift));
                                        typeNotifierParagraph.Foreground = Brushes.LightGray;
                                        rtbDocument.Blocks.Add(typeNotifierParagraph);

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(
                                                    ((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"))).Name.ToString()));

                                            }
                                            else
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")));
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        //if (ContactDataContext.GetInstance().ClientPromptColor.ToString().Contains("#"))
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientPromptColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientPromptColor.A, ContactDataContext.GetInstance().ClientPromptColor.R, ContactDataContext.GetInstance().ClientPromptColor.G, ContactDataContext.GetInstance().ClientPromptColor.B));
                                        //}
                                        //if (ContactDataContext.GetInstance().ClientTextColor.ToString().Contains("#"))
                                        //{
                                        //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientTextColor.Name.ToString()));
                                        //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        //    normalMessageParagraph.Inlines.Add(messageText);
                                        //}
                                        //else
                                        //{
                                        //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientTextColor.A, ContactDataContext.GetInstance().ClientTextColor.R, ContactDataContext.GetInstance().ClientTextColor.G, ContactDataContext.GetInstance().ClientTextColor.B));
                                        //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        //    normalMessageParagraph.Inlines.Add(messageText);
                                        //}
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.client.text-color")).Contains("#"))
                                            {
                                                normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color"))).Name.ToString()));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color")));
                                                normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }
                                        DetectURLs(normalMessageParagraph);
                                        rtbDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(
                                                    ((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"))).Name.ToString()));

                                            }
                                            else
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")));
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        //if (ContactDataContext.GetInstance().ClientPromptColor.ToString().Contains("#"))
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientPromptColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientPromptColor.A, ContactDataContext.GetInstance().ClientPromptColor.R, ContactDataContext.GetInstance().ClientPromptColor.G, ContactDataContext.GetInstance().ClientPromptColor.B));
                                        //}
                                        //if (ContactDataContext.GetInstance().ClientTextColor.ToString().Contains("#"))
                                        //{
                                        //    urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().ClientTextColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().ClientTextColor.A, ContactDataContext.GetInstance().ClientTextColor.R, ContactDataContext.GetInstance().ClientTextColor.G, ContactDataContext.GetInstance().ClientTextColor.B));
                                        //}
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.client.text-color")).Contains("#"))
                                            {
                                                urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color")));
                                                urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        temprun = new Run(noticeText);
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        rtbDocument.Blocks.Add(urlMessageParagraph);
                                    }
                                    break;
                                case "otheragent":
                                    if (noticeText == string.Empty && messageText == string.Empty)
                                    {

                                    }
                                    if (noticeText == string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }

                                        //if (ContactDataContext.GetInstance().OtherAgentPromptColor.ToString().Contains("#"))
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentPromptColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentPromptColor.A, ContactDataContext.GetInstance().OtherAgentPromptColor.R, ContactDataContext.GetInstance().OtherAgentPromptColor.G, ContactDataContext.GetInstance().OtherAgentPromptColor.B));
                                        //}
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")).Contains("#"))
                                            {
                                                normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"))).Name.ToString()));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")));
                                                normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                                normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                                normalMessageParagraph.Inlines.Add(messageText);
                                            }
                                        //if (ContactDataContext.GetInstance().OtherAgentTextColor.ToString().Contains("#"))
                                        //{
                                        //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentTextColor.Name.ToString()));
                                        //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        //    normalMessageParagraph.Inlines.Add(messageText);
                                        //}
                                        //else
                                        //{
                                        //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentTextColor.A, ContactDataContext.GetInstance().OtherAgentTextColor.R, ContactDataContext.GetInstance().OtherAgentTextColor.G, ContactDataContext.GetInstance().OtherAgentTextColor.B));
                                        //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        //    normalMessageParagraph.Inlines.Add(messageText);
                                        //}
                                        DetectURLs(normalMessageParagraph);
                                        rtbDocument.Blocks.Add(normalMessageParagraph);

                                    }
                                    else if (noticeText != string.Empty && messageText != string.Empty)
                                    {
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")).Contains("#"))
                                            {
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                                commonRun = new Run(userNameWithTimeShift);
                                                commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        //if (ContactDataContext.GetInstance().OtherAgentPromptColor.ToString().Contains("#"))
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentPromptColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    commonRun = new Run(userNameWithTimeShift);
                                        //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentPromptColor.A, ContactDataContext.GetInstance().OtherAgentPromptColor.R, ContactDataContext.GetInstance().OtherAgentPromptColor.G, ContactDataContext.GetInstance().OtherAgentPromptColor.B));
                                        //}
                                        if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                                            if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")).Contains("#"))
                                            {
                                                urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"))).Name.ToString()));
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")));
                                                urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                        //if (ContactDataContext.GetInstance().OtherAgentTextColor.ToString().Contains("#"))
                                        //{
                                        //    urlMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentTextColor.Name.ToString()));
                                        //}
                                        //else
                                        //{
                                        //    urlMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentTextColor.A, ContactDataContext.GetInstance().OtherAgentTextColor.R, ContactDataContext.GetInstance().OtherAgentTextColor.G, ContactDataContext.GetInstance().OtherAgentTextColor.B));
                                        //}
                                        urlMessageParagraph.Inlines.Add(new Bold(commonRun));
                                        urlMessageParagraph.Inlines.Add(messageText);
                                        Run temprun = null;
                                        temprun = new Run(noticeText);
                                        Hyperlink hl = new Hyperlink(temprun);
                                        hl.Foreground = Brushes.Blue;
                                        hl.NavigateUri = new Uri(noticeText);
                                        urlMessageParagraph.Inlines.Add(hl);
                                        rtbDocument.Blocks.Add(urlMessageParagraph);
                                    }
                                    break;
                            }
                            break;
                        case "partyLeft":
                            switch (chatUserType.ToLower())
                            {
                                case "agent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    break;
                                case "client":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(
                                                ((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"))).Name.ToString()));

                                        }
                                        else
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")));
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.client.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.client.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    break;
                                case "otheragent":
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")).Contains("#"))
                                        {
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"))).Name.ToString()));
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")));
                                            commonRun = new Run(userNameWithTimeShift);
                                            commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                        }
                                    //if (ContactDataContext.GetInstance().OtherAgentPromptColor.ToString().Contains("#"))
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentPromptColor.Name.ToString()));
                                    //}
                                    //else
                                    //{
                                    //    commonRun = new Run(userNameWithTimeShift);
                                    //    commonRun.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentPromptColor.A, ContactDataContext.GetInstance().OtherAgentPromptColor.R, ContactDataContext.GetInstance().OtherAgentPromptColor.G, ContactDataContext.GetInstance().OtherAgentPromptColor.B));
                                    //}
                                    if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                                        if (((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")).Contains("#"))
                                        {
                                            normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"))).Name.ToString()));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                        else
                                        {
                                            System.Drawing.Color color = System.Drawing.Color.FromName(((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")));
                                            normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
                                            normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                            normalMessageParagraph.Inlines.Add(messageText);
                                        }
                                    //if (ContactDataContext.GetInstance().OtherAgentTextColor.ToString().Contains("#"))
                                    //{
                                    //    normalMessageParagraph.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(ContactDataContext.GetInstance().OtherAgentTextColor.Name.ToString()));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    //else
                                    //{
                                    //    normalMessageParagraph.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ContactDataContext.GetInstance().OtherAgentTextColor.A, ContactDataContext.GetInstance().OtherAgentTextColor.R, ContactDataContext.GetInstance().OtherAgentTextColor.G, ContactDataContext.GetInstance().OtherAgentTextColor.B));
                                    //    normalMessageParagraph.Inlines.Add(new Bold(commonRun));
                                    //    normalMessageParagraph.Inlines.Add(messageText);
                                    //}
                                    break;
                            }
                            DetectURLs(normalMessageParagraph);
                            rtbDocument.Blocks.Add(normalMessageParagraph);
                            break;
                    }
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while do chatConversationBinding()" + generalException.ToString());
                }
            }));
        }

        public static bool IsHyperlink(string word)
        {
            try
            {
                Regex urlRegex = new Regex(@"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$");
                if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
                {
                    Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
                    if (UrlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                    else if (urlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                // logger.Error("Error occurred while IsHyperlink() :" + generalException.ToString());
            }
            return false;
        }

        public static void DetectURLs(Paragraph par)
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                try
                {
                    string paragraphText = new TextRange(par.ContentStart, par.ContentEnd).Text; 
                    paragraphText = paragraphText.Replace("\n", string.Empty);
                    foreach (string word in paragraphText.Split(' ').ToList())
                    {
                        if (IsHyperlink(word))
                        {
                            Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                            if (!uri.IsAbsoluteUri)
                            {
                                uri = new Uri(@"http://" + word, UriKind.Absolute);
                            }

                            if (uri != null)
                            {
                                TextPointer position = par.ContentStart;
                                while (position != null)
                                {
                                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                                    {
                                        string textRun = position.GetTextInRun(LogicalDirection.Forward);
                                        int indexInRun = textRun.IndexOf(word);
                                        if (indexInRun >= 0)
                                        {
                                            TextPointer start = position.GetPositionAtOffset(indexInRun);
                                            TextPointer end = start.GetPositionAtOffset(word.Length);
                                            var link = new Hyperlink(start, end);
                                            link.NavigateUri = uri;
                                            link.RequestNavigate += Hyperlink_Click;
                                            link.Foreground = Brushes.Blue;
                                            link.Style = null;
                                        }
                                    }
                                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                                }
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    //  logger.Error("Error occurred in DetectURLs() " + generalException.Message);
                }
            }));

        }

        public static void Hyperlink_Click(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
                e.Handled = true;
            }
            catch (Exception generalExprtion)
            {
                // logger.Error("Error occurred while Hyperlink_Click() : " + generalExprtion.ToString());
            }
        }
    }
}
