using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Email
{
    internal class EmailCreate
    {
        private static EmailCreate _emailCreate = null;
        private EmailManager _emailEvents = null;
        private Log _logger = null;
        private SFDCData _sfdcPopupData = null;
        private SearchHandler searchHandler = null;

        private EmailCreate()
        {
            this._logger = Log.GenInstance();
            this._emailEvents = EmailManager.GetInstance();
            this.searchHandler = SearchHandler.GetInstance();
        }

        public static EmailCreate GetInstance()
        {
            if (_emailCreate == null)
            {
                _emailCreate = new EmailCreate();
            }
            return _emailCreate;
        }

        public void PopupRecords(IXNCustomData emailData, SFDCCallType callType)
        {
            try
            {
                this._logger.Info("Trying to Popup SFDC on EmailCreate Event, Email Type: " + callType.ToString());
                if (callType == SFDCCallType.OutboundEmailSuccess)
                {
                    this._sfdcPopupData = this._emailEvents.GetOutboundEmailPopupData(emailData, callType, "create");
                    if (this._sfdcPopupData != null)
                    {
                        this._sfdcPopupData.InteractionId = emailData.InteractionId;
                        if (emailData.IXN_Attributes.ParentId != null && Settings.InboundEmailSearchResult.ContainsKey(emailData.IXN_Attributes.ParentId))
                        {
                            this.searchHandler.ProcessSFDCResponseForCommonSearch(Settings.InboundEmailSearchResult[emailData.IXN_Attributes.ParentId], this._sfdcPopupData, emailData.InteractionId, callType);
                        }
                        else
                        {
                            this.searchHandler.ProcessSearchData(emailData.InteractionId, this._sfdcPopupData, callType);
                        }
                    }
                    else
                        this._logger.Info("Search data is empty");
                }
            }
            catch (Exception exception)
            {
                this._logger.Error("EmailCreate: Error Occurred : " + exception.ToString());
            }
        }
    }
}