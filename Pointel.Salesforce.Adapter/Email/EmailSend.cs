using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    internal class EmailSend
    {
        private static EmailSend _outboundEmailSend = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcPopupData = null;
        private SearchHandler searchHandler = null;

        private EmailSend()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this.searchHandler = SearchHandler.GetInstance();
        }

        public static EmailSend GetInstance()
        {
            if (_outboundEmailSend == null)
            {
                _outboundEmailSend = new EmailSend();
            }
            return _outboundEmailSend;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to popup SFDC on EmailSend Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.OutboundEmailSuccess)
                {
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailPopupData(emailData, callType, "send");
                    if (this._sfdcPopupData != null)
                    {
                        this._sfdcPopupData.InteractionId = emailData.InteractionId;
                        this.searchHandler.ProcessSearchData(emailData.InteractionId, this._sfdcPopupData, callType);
                    }
                    else
                        this._logger.Info("Search data is empty");

                    this._logger.Info("Trying to update SFDC on EmailSend Event, Email Type: " + callType.ToString());
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailUpdateData(emailData, callType, "send");
                    if (this._sfdcPopupData != null)
                    {
                        this._sfdcPopupData.InteractionId = emailData.InteractionId;

                        if (Settings.SFDCOptions.CanAddEmailAttachmentsInLog)
                            this.searchHandler.ProcessUpdateData(emailData.InteractionId, this._sfdcPopupData, true);
                        else
                            this.searchHandler.ProcessUpdateData(emailData.InteractionId, this._sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailSend: Error Occurred, Exception : " + exception.ToString());
            }
        }
    }
}