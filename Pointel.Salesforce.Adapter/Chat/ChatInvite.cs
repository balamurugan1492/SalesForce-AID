using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{/// <summary>
    /// Comment: Provides Inbound Voice Call SFDC Popup Last Modified: 25-08-2015 Created by: Pointel
    /// Inc </summary>
    public class ChatInvite
    {
        #region Fields

        private static ChatInvite _inviteObject = null;
        private ChatManager _chatEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcPopupData = null;
        private SearchHandler _searchHandler = null;

        #endregion Fields

        #region Constructor

        private ChatInvite()
        {
            this._logger = Log.GenInstance();
            this._chatEvents = ChatManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static ChatInvite GetInstance()
        {
            if (_inviteObject == null)
            {
                _inviteObject = new ChatInvite();
            }
            return _inviteObject;
        }

        #endregion GetInstance

        #region PopupRecords

        public void PopupRecords(IXNCustomData ixnData, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat)
                    _sfdcPopupData = this._chatEvents.GetInboundPopupData(ixnData, callType, "invite");
                else if (callType == SFDCCallType.ConsultChatReceived)
                    _sfdcPopupData = this._chatEvents.GetConsultReceivedPopupData(ixnData, callType, "invite");

                if (_sfdcPopupData != null)
                {
                    _sfdcPopupData.InteractionId = ixnData.InteractionId;
                    this._searchHandler.ProcessSearchData(ixnData.InteractionId, _sfdcPopupData, callType);
                }
                else
                    this._logger.Info("ChatInvite.PopupRecords : Search data is empty");
            }
            catch (Exception generalException)
            {
                _logger.Error("ChatInvite.PopupRecords : Error Occurred while collecting Log data : " + generalException.ToString());
            }
        }

        #endregion PopupRecords

        #region UpdateRecords

        public void UpdateRecords(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundChat || callType == SFDCCallType.ConsultChatReceived)
                {
                    _sfdcPopupData = this._chatEvents.GetInboundUpdateData(chatData, "datachanged");
                    if (_sfdcPopupData != null)
                    {
                        _sfdcPopupData.InteractionId = chatData.InteractionId;
                        this._searchHandler.ProcessUpdateData(chatData.InteractionId, _sfdcPopupData);
                    }
                    else
                        this._logger.Info("ChatInvite.UpdateRecords : Search data is empty");
                }
            }
            catch (Exception generalException)
            {
                this._logger.Error("ChatInvite.UpdateRecords : Error Occurred in Updating sfdc data : " + generalException.ToString());
            }
        }

        #endregion UpdateRecords
    }
}