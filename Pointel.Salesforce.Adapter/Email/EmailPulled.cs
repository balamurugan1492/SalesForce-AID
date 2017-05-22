using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    public class EmailPulled
    {
        private static EmailPulled _pulledObject = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailPulled()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailPulled GetInstance()
        {
            if (_pulledObject == null)
            {
                _pulledObject = new EmailPulled();
            }
            return _pulledObject;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on EmailPulled Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmailPulled)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundPopupData(emailData, callType, "pulled");
                }
                else if (callType == SFDCCallType.OutboundEmailPulled)
                {
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailPopupData(emailData, callType, "pulled");
                }

                if (this._sfdcPopupData != null)
                {
                    this._sfdcPopupData.InteractionId = emailData.InteractionId;
                    this._searchHandler.ProcessSearchData(emailData.InteractionId, this._sfdcPopupData, callType);
                }
                else
                    this._logger.Info("Search data is empty");
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailPulled: Error Occurred while collecting Log data : " + exception.ToString());
            }
        }
    }
}