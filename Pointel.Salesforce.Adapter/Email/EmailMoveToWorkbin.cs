using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    public class EmailMoveToWorkbin
    {
        private static EmailMoveToWorkbin _MoveObject = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailMoveToWorkbin()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailMoveToWorkbin GetInstance()
        {
            if (_MoveObject == null)
            {
                _MoveObject = new EmailMoveToWorkbin();
            }
            return _MoveObject;
        }

        public void UpdateRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on EmailMoveToWorkbin Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                    this._sfdcPopupData = this._emailEvents.GetInboundUpdateData(emailData, callType, "movetoworkbin");
                else if (callType == SFDCCallType.OutboundEmailSuccess)
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailUpdateData(emailData, callType, "movetoworkbin");

                if (this._sfdcPopupData != null)
                {
                    this._sfdcPopupData.InteractionId = emailData.InteractionId;
                    this._searchHandler.ProcessUpdateData(emailData.InteractionId, this._sfdcPopupData);
                }
                else
                    this._logger.Info("UpdateRecords ; Search data is empty");
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailMoveToWorkbin: Error Occurred while collecting data : " + exception.ToString());
            }
        }
    }
}