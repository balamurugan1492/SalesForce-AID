namespace Pointel.Salesforce.Adapter.Email
{
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using System;

    public class EmailConnected
    {
        private static EmailConnected _emailConnected = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailConnected()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailConnected GetInstance()
        {
            if (_emailConnected == null)
            {
                _emailConnected = new EmailConnected();
            }
            return _emailConnected;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to Popup SFDC on EmailConnected Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundPopupData(emailData, callType, "established");
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
                this._logger.Error("EmailConnected : Error Occurred while collecting Log data : " + exception.ToString());
            }
        }

        public void UpdateRecords(IXNCustomData emailData, SFDCCallType callType, string callDuration, string chatContent)
        {
            try
            {
                this._logger.Info("Trying to Update SFDC Logs on EmailConnected event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundUpdateData(emailData, callType, "datachanged");
                    if (this._sfdcPopupData != null)
                    {
                        this._sfdcPopupData.InteractionId = emailData.InteractionId;
                        this._searchHandler.ProcessUpdateData(emailData.InteractionId, this._sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailConnected : Error Occurred in Updating sfdc data : " + exception.ToString());
            }
        }
    }
}