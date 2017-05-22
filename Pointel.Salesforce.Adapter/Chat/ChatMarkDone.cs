using Pointel.Salesforce.Adapter.LogMessage;
using Pointel.Salesforce.Adapter.SFDCUtils;
using Pointel.Salesforce.Adapter.Utility;
using System;

namespace Pointel.Salesforce.Adapter.Chat
{
    internal class ChatMarkDone
    {
        private static ChatMarkDone MarkdoneObject = null;
        private SearchHandler _searchHandler = null;
        private ChatManager chatManager = null;
        private Log logger = null;
        private SFDCData sfdcPopupData = null;

        private ChatMarkDone()
        {
            this.logger = Log.GenInstance();
            this.chatManager = ChatManager.GetInstance();
            this._searchHandler = SearchHandler.GetInstance();
        }

        public static ChatMarkDone GetInstance()
        {
            if (MarkdoneObject == null)
            {
                MarkdoneObject = new ChatMarkDone();
            }
            return MarkdoneObject;
        }

        public void UpdateRecords(IXNCustomData chatData, SFDCCallType callType)
        {
            try
            {
                if (callType == SFDCCallType.InboundEmail)
                {
                    this.logger.Info("UpdateRecords : Update SFDC Logs for Email Markdone ");
                    this.sfdcPopupData = this.chatManager.GetInboundUpdateData(chatData, "markdone");
                    if (this.sfdcPopupData != null)
                    {
                        this.sfdcPopupData.InteractionId = chatData.InteractionId;
                        this._searchHandler.ProcessUpdateData(chatData.InteractionId, this.sfdcPopupData);
                    }
                    else
                    {
                        this.logger.Info("UpdateRecords : Search data is empty");
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger.Error("UpdateRecords : Error Occurred while collecting Log data : " + exception.ToString());
            }
        }
    }
}