using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{
    internal class ChatSessionConnected
    {/// <summary>
        /// Comment: Provides Inbound Chat SFDC Popup Last Modified: 25-08-2015 Created by: Pointel
        /// Inc </summary>

        #region Fields

        private static ChatSessionConnected _inviteObject = null;
        private ChatManager _chatEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcPopupData = null;
        private SearchHandler _searchHandler = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor of the Class
        /// </summary>
        private ChatSessionConnected()
        {
            this._logger = Log.GenInstance();
            this._chatEvents = ChatManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// Gets the Instance of the class
        /// </summary>
        /// <returns></returns>
        public static ChatSessionConnected GetInstance()
        {
            if (_inviteObject == null)
            {
                _inviteObject = new ChatSessionConnected();
            }
            return _inviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        /// <summary>
        /// Popup SFDC on EventInvite Event
        /// </summary>
        /// <param name="ixnData"></param>
        /// <param name="callType"></param>
        public void PopupRecords(IXNCustomData ixnData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("PopupRecords: ChatSessionConnected Event ");
                if (callType == SFDCCallType.InboundChat)
                    _sfdcPopupData = this._chatEvents.GetInboundPopupData(ixnData, callType, "established");
                else if (callType == SFDCCallType.ConsultChatReceived)
                    _sfdcPopupData = this._chatEvents.GetInboundPopupData(ixnData, callType, "established");

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = ixnData.InteractionId;
                    this._searchHandler.ProcessSearchData(ixnData.InteractionId, _sfdcPopupData, callType);
                }
                else
                    this._logger.Info("PopupRecords : Search data is empty");
            }
            catch (Exception generalException)
            {
                this._logger.Error("PopupRecords : Error Occurred at ChatConnected : " + generalException.ToString());
            }
        }

        #endregion PopupRecords
    }
}