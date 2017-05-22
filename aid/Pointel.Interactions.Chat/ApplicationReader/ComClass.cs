using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Interactions.Chat.Settings;
using Pointel.Configuration.Manager;
using System.Linq;

namespace Pointel.Interactions.Chat.ApplicationReader
{
    public class ComClass
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private List<string> loadCaseDataKey = new List<string>();
        private List<string> loadCaseDataFilterKey = new List<string>();
        private List<string> loadCaseDataSortingKey = new List<string>();
        private Dictionary<string, string> dispositionCodes = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> dispositionSubCodes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> loadAvailablePushURL = new Dictionary<string, string>();
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        #endregion

        #region ReadCaseData Push URL and Disposition
        /// <summary>
        /// Reads the case data push URL disposition.
        /// </summary>
        public void ReadCaseDataPushUrlDisposition()
        {
            try
            {
                loadCaseDataKey.Clear();
                loadCaseDataFilterKey.Clear();
                loadCaseDataSortingKey.Clear();
                loadAvailablePushURL.Clear();
                dispositionCodes.Clear();
                dispositionSubCodes.Clear();
                if (ConfigContainer.Instance().AllKeys.Contains("ChatAttachDataKey") && ConfigContainer.Instance().GetValue("ChatAttachDataKey") != null)
                    loadCaseDataKey = ConfigContainer.Instance().GetValue("ChatAttachDataKey");
                if (ConfigContainer.Instance().AllKeys.Contains("ChatAttachDataFilterKey") && ConfigContainer.Instance().GetValue("ChatAttachDataFilterKey") != null)
                    loadCaseDataFilterKey = ConfigContainer.Instance().GetValue("ChatAttachDataFilterKey");
                if (ConfigContainer.Instance().AllKeys.Contains("ChatAttachDataSortKey") && ConfigContainer.Instance().GetValue("ChatAttachDataSortKey") != null)
                    loadCaseDataSortingKey = ConfigContainer.Instance().GetValue("ChatAttachDataSortKey");
                if (ConfigContainer.Instance().AllKeys.Contains("ChatPushUrl") && ConfigContainer.Instance().GetValue("ChatPushUrl") != null)
                    loadAvailablePushURL = ConfigContainer.Instance().GetValue("ChatPushUrl");
                if (ConfigContainer.Instance().AllKeys.Contains("chat.disposition.codes"))
                    dispositionCodes = ConfigContainer.Instance().GetValue("chat.disposition.codes");
                if (ConfigContainer.Instance().AllKeys.Contains("interaction.enable.multi-dispositioncode") && (((string)ConfigContainer.Instance().GetValue("interaction.enable.multi-dispositioncode")).ToLower().Equals("true")))
                    if (ConfigContainer.Instance().AllKeys.Contains("chat.subdisposition.codes"))
                        dispositionSubCodes = ConfigContainer.Instance().GetValue("chat.subdisposition.codes");
            }
            catch (Exception ex)
            {
                logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }
        #endregion

        #region Get all KVP's based on the hierarchical level
        /// <summary>
        /// Gets all values.
        /// </summary>
        public void GetAllValues()
        {
            try
            {
                logger.Debug("Get all KVP's based on the hierarchical level");

                //System Values
                if (ConfigContainer.Instance().AllKeys.Contains("image-path"))
                    _chatDataContext.Imagepath = (string)ConfigContainer.Instance().GetValue("image-path");

                //agent.ixn.desktop values
                if (ConfigContainer.Instance().AllKeys.Contains("chat.welcome-message"))
                    _chatDataContext.ChatWelcomeMessage = (string)ConfigContainer.Instance().GetValue("chat.welcome-message");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.farewell-message"))
                    _chatDataContext.ChatFareWellMessage = (string)ConfigContainer.Instance().GetValue("chat.farewell-message");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.typing-timeout"))
                    _chatDataContext.ChatTypingTimout = (string)ConfigContainer.Instance().GetValue("chat.typing-timeout");

                //if (ConfigContainer.Instance().AllKeys.Contains("chat.toast-information-key"))

                if (ConfigContainer.Instance().AllKeys.Contains("chat.time-stamp-format"))
                    _chatDataContext.ChatTimeStampFormat = (string)ConfigContainer.Instance().GetValue("chat.time-stamp-format");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.system.text-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.system.text-color")))
                        _chatDataContext.ChatSystemTextColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.system.text-color"));
                    else
                        _chatDataContext.ChatSystemTextColor = Color.FromName("Black");
                else
                    _chatDataContext.ChatSystemTextColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.pending-response-to-customer"))
                    _chatDataContext.ChatPendingResponseToCustomer = (string)ConfigContainer.Instance().GetValue("chat.pending-response-to-customer");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.text-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color")))
                        _chatDataContext.OtherAgentTextColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.other-agent.text-color"));
                    else
                        _chatDataContext.OtherAgentTextColor = Color.FromName("Black");
                else
                    _chatDataContext.OtherAgentTextColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.other-agent.prompt-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color")))
                        _chatDataContext.OtherAgentPromptColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.other-agent.prompt-color"));
                    else
                        _chatDataContext.OtherAgentPromptColor = Color.FromName("Black");
                else
                    _chatDataContext.OtherAgentPromptColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.client.text-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.client.text-color")))
                        _chatDataContext.ClientTextColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.client.text-color"));
                    else
                        _chatDataContext.ClientTextColor = Color.FromName("Black");
                else
                    _chatDataContext.ClientTextColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.client.prompt-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color")))
                        _chatDataContext.ClientPromptColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.client.prompt-color"));
                    else
                        _chatDataContext.ClientPromptColor = Color.FromName("Black");
                else
                    _chatDataContext.ClientPromptColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.text-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.agent.text-color")))
                        _chatDataContext.AgentTextColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.agent.text-color"));
                    else
                        _chatDataContext.AgentTextColor = Color.FromName("Black");
                else
                    _chatDataContext.AgentTextColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.agent.prompt-color"))
                    if (!string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color")))
                        _chatDataContext.AgentPromptColor = Color.FromName((string)ConfigContainer.Instance().GetValue("chat.agent.prompt-color"));
                    else
                        _chatDataContext.AgentPromptColor = Color.FromName("Black");
                else
                    _chatDataContext.AgentPromptColor = Color.FromName("Black");

                if (ConfigContainer.Instance().AllKeys.Contains("chat.auto-answer.timer"))
                    _chatDataContext.AutoAnswerDelay = int.Parse((string)ConfigContainer.Instance().GetValue("chat.auto-answer.timer")) * 1000;

                if (ConfigContainer.Instance().AllKeys.Contains("voice.dial-pad.number-digit"))
                    _chatDataContext.DialpadDigits = int.Parse((string)ConfigContainer.Instance().GetValue("voice.dial-pad.number-digit"));

                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name"))
                    _chatDataContext.DisPositionKeyName = (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name");

                if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name"))
                    _chatDataContext.DispositionCollectionKeyName = (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name");

                //enable.disable.channels values
                if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.auto-answer.reject"))
                    if ((((string)ConfigContainer.Instance().GetValue("chat.enable.auto-answer.reject")).ToLower().Equals("true")))
                        _chatDataContext.IsEnableAutoAnswerReject = true;
                    else
                        _chatDataContext.IsEnableAutoAnswerReject = false;
                if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.add-case-data"))
                    if ((((string)ConfigContainer.Instance().GetValue("chat.enable.add-case-data")).ToLower().Equals("true")))
                        _chatDataContext.IsChatEnabledAddCaseData = Visibility.Visible;
                    else
                        _chatDataContext.IsChatEnabledAddCaseData = Visibility.Hidden;
                _chatDataContext.OwnerIDorPersonDBID = ConfigContainer.Instance().PersonDbId;

                if (ConfigContainer.Instance().AllKeys.Contains("email.validate-expression"))
                    _chatDataContext.EmailValidateExpression = ConfigContainer.Instance().GetAsString("email.validate-expression");

                ReadCaseDataPushUrlDisposition();

                if (loadCaseDataKey.Count > 0 && loadCaseDataKey != null)
                {
                    _chatDataContext.LoadCaseDataKeys = loadCaseDataKey.ToList();
                }
                if (loadCaseDataFilterKey.Count > 0 && loadCaseDataFilterKey != null)
                {
                    _chatDataContext.LoadCaseDataFilterKeys = loadCaseDataFilterKey.ToList();
                }
                if (loadCaseDataSortingKey.Count > 0 && loadCaseDataSortingKey != null)
                {
                    _chatDataContext.LoadCaseDataSortKeys = loadCaseDataSortingKey.ToList();
                }
                if (loadAvailablePushURL.Count > 0 && loadAvailablePushURL != null)
                {
                    _chatDataContext.LoadAvailablePushURL.Clear();
                    foreach (var item in loadAvailablePushURL)
                        _chatDataContext.LoadAvailablePushURL.Add(item.Key, item.Value);
                }
                if (dispositionCodes.Count > 0 && dispositionCodes != null)
                {
                    _chatDataContext.LoadDispositionCodes.Clear();
                    foreach (var items in dispositionCodes)
                        _chatDataContext.LoadDispositionCodes.Add(items.Key, items.Value);

                }
                if (dispositionSubCodes.Count > 0 && dispositionSubCodes != null)
                {
                    _chatDataContext.LoadSubDispositionCodes.Clear();
                    _chatDataContext.LoadSubDispositionCodes = dispositionSubCodes;
                }
            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred while reading GetAllValues() =  " + commonException.ToString());
            }
            finally
            {
                loadCaseDataKey.Clear();
                loadCaseDataKey = null;
                loadCaseDataFilterKey.Clear();
                loadCaseDataFilterKey = null;
                loadCaseDataSortingKey.Clear();
                loadCaseDataSortingKey = null;
                dispositionCodes.Clear();
                dispositionCodes = null;
                dispositionSubCodes.Clear();
                dispositionSubCodes = null;
                loadAvailablePushURL.Clear();
                loadAvailablePushURL = null;
            }
        }
        #endregion
    }
}
