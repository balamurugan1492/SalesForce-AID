using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{
    internal class ChatSessionEnded
    {
        #region Fields

        private static ChatSessionEnded _inviteObject = null;
        private ChatManager _chatEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcUpdateData = null;
        private SearchHandler _searchHandler = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor of the Class ChatSessionEnded
        /// </summary>
        private ChatSessionEnded()
        {
            this._logger = Log.GenInstance();
            this._chatEvents = ChatManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets Instance of the class ChatSessionEnded
        /// </summary>
        /// <returns></returns>
        public static ChatSessionEnded GetInstance()
        {
            if (_inviteObject == null)
            {
                _inviteObject = new ChatSessionEnded();
            }
            return _inviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC on Chat Connected
        /// </summary>
        /// <param name="eventInvite"></param>
        /// <param name="callType"></param>
        /// <param name="callDuration"></param>
        /// <param name="chatContent"></param>
        public void PopupRecords(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                    _sfdcUpdateData = this._chatEvents.GetInboundUpdateData(chatData, "released");
                else if (callType == SFDCCallType.ConsultChatReceived)
                    _sfdcUpdateData = this._chatEvents.GetConsultUpdateData(chatData, "released");

                if (_sfdcUpdateData != null)
                {
                    _sfdcUpdateData.InteractionId = chatData.InteractionId;
                    this._searchHandler.ProcessUpdateData(chatData.InteractionId, _sfdcUpdateData);
                }
                else
                    this._logger.Info("PopupRecords : Search data is empty");
            }
            catch (Exception generalException)
            {
                this._logger.Error("PopupRecords : Error Occurred While collecting log data : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}