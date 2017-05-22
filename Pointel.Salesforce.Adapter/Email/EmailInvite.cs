namespace Pointel.Salesforce.Adapter.Email
{
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using System;

    public class EmailInvite
    {
        private static EmailInvite _InviteObject = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailInvite()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailInvite GetInstance()
        {
            if (_InviteObject == null)
            {
                _InviteObject = new EmailInvite();
            }
            return _InviteObject;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to popup or update SFDC on EmailInvite Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundPopupData(emailData, callType, "invite");
                    if (this._sfdcPopupData != null)
                    {
                        this._sfdcPopupData.InteractionId = emailData.InteractionId;
                        this._searchHandler.ProcessSearchData(emailData.InteractionId, this._sfdcPopupData, callType);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailInvite: Error Occurred : " + exception.ToString());
            }
        }

        public void UpdateRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on AttachedDataChanged Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                    this._sfdcPopupData = this._emailEvents.GetInboundUpdateData(emailData, callType, "datachanged");
                else if (callType == SFDCCallType.OutboundEmailSuccess)
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailUpdateData(emailData, callType, "datachanged");

                if (this._sfdcPopupData != null)
                {
                    this._sfdcPopupData.InteractionId = emailData.InteractionId;
                    this._searchHandler.ProcessUpdateData(emailData.InteractionId, this._sfdcPopupData);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailInvite: Error Occurred in Updating sfdc data : " + exception.ToString());
            }
        }
    }
}