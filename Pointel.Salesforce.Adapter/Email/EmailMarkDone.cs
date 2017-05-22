namespace Pointel.Salesforce.Adapter.Email
{
    using Pointel.Salesforce.Adapter.LogMessage;
    using Pointel.Salesforce.Adapter.SFDCUtils;
    using Pointel.Salesforce.Adapter.Utility;
    using System;

    public class EmailMarkDone
    {
        private static EmailMarkDone MarkdoneObject = null;
        private SearchHandler _searchHandler = null;
        private EmailManager emailEvents = null;
        private Log _logger = null;
        private SFDCData sfdcPopupData = null;

        private EmailMarkDone()
        {
            this._logger = Log.GenInstance();
            this.emailEvents = EmailManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static EmailMarkDone GetInstance()
        {
            if (MarkdoneObject == null)
            {
                MarkdoneObject = new EmailMarkDone();
            }
            return MarkdoneObject;
        }

        public void UpdateRecords(IXNCustomData EmailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to update SFDC on Markdone Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.InboundEmail)
                {
                    this.sfdcPopupData = this.emailEvents.GetInboundUpdateData(EmailData, callType, "markdone");
                    if (this.sfdcPopupData != null)
                    {
                        this.sfdcPopupData.InteractionId = EmailData.InteractionId;
                        this._searchHandler.ProcessUpdateData(EmailData.InteractionId, this.sfdcPopupData);
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailMarkDone: Error Occurred while collecting Log data : " + exception.ToString());
            }
        }
    }
}