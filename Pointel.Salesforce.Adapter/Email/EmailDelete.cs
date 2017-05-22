using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    internal class EmailDelete
    {
        private static EmailDelete _EmailRejected = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailDelete()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailDelete GetInstance()
        {
            if (_EmailRejected == null)
            {
                _EmailRejected = new EmailDelete();
            }
            return _EmailRejected;
        }

        public void UpdateRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to popup or update SFDC on EmailDelete Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.OutboundEmailSuccess)
                {
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailUpdateData(emailData, callType, "delete");
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
                this._logger.Error("EmailDelete: Error Occurred : " + exception.ToString());
            }
        }
    }
}