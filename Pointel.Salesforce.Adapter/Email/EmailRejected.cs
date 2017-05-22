using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    internal class EmailRejected
    {
        private static EmailRejected _EmailRejected = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailRejected()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailRejected GetInstance()
        {
            if (_EmailRejected == null)
            {
                _EmailRejected = new EmailRejected();
            }
            return _EmailRejected;
        }

        public void UpdateRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on EmailRejected Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundUpdateData(emailData, callType, "rejected");
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
                this._logger.Error("EmailRejected: Error Occurred while collecting Log data : " + exception.ToString());
            }
        }
    }
}