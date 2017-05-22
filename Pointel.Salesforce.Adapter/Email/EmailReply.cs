using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    public class EmailReply
    {
        private static EmailReply _replyObject = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SearchHandler _searchHandler = null;
        private SFDCData _sfdcPopupData = null;

        private EmailReply()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailReply GetInstance()
        {
            if (_replyObject == null)
            {
                _replyObject = new EmailReply();
            }
            return _replyObject;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on EmailReply Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this._sfdcPopupData = this._emailEvents.GetInboundUpdateData(emailData, callType, "reply");
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
                this._logger.Error("PopupRecords : Error Occurred while collecting Log data : " + exception.ToString());
            }
        }
    }
}