using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{
    internal class ChatRejected
    {
        #region Fields

        private static ChatRejected _thisObject = null;
        private ChatManager _chatEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcUpdateData = null;
        private SearchHandler _searchHandler = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="ChatRejected"/> class from being created.
        /// </summary>
        private ChatRejected()
        {
            this._logger = Log.GenInstance();
            this._chatEvents = ChatManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static ChatRejected GetInstance()
        {
            if (_thisObject == null)
            {
                _thisObject = new ChatRejected();
            }
            return _thisObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Updates the records.
        /// </summary>
        /// <param name="chatData">The chat data.</param>
        /// <param name="callType">Type of the call.</param>
        public void UpdateRecords(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                    _sfdcUpdateData = this._chatEvents.GetInboundUpdateData(chatData, "rejected");
                else if (callType == SFDCCallType.ConsultChatReceived)
                    _sfdcUpdateData = this._chatEvents.GetConsultUpdateData(chatData, "rejected");

                if (_sfdcUpdateData != null)
                {
                    _sfdcUpdateData.InteractionId = chatData.InteractionId;
                    this._searchHandler.ProcessUpdateData(chatData.InteractionId, _sfdcUpdateData);
                }
                else
                    this._logger.Info("UpdateRecords: Search data is empty");
            }
            catch (Exception generalException)
            {
                this._logger.Error("UpdateRecords : Error Occurred While collecting log data : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}