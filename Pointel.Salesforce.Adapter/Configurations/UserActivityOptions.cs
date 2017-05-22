/*
  Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

namespace Pointel.Salesforce.Adapter.Configurations
{
    internal class UserActivityOptions
    {
        public bool ConsultCanCreateLog
        {
            get;
            set;
        }

        public bool ConsultCanUpdateLog
        {
            get;
            set;
        }

        public string ConsultLogEvent
        {
            get;
            set;
        }

        public string ConsultPopupEvent
        {
            get;
            set;
        }

        public string ConsultUpdateEvent
        {
            get;
            set;
        }

        public bool InboundCanCreateLog
        {
            get;
            set;
        }

        public bool InboundCanUpdateLog
        {
            get;
            set;
        }

        public string InboundPopupEvent
        {
            get;
            set;
        }

        public string InboundUpdateEvent
        {
            get;
            set;
        }

        public string ObjectName
        {
            get;
            set;
        }

        public bool OutboundCanCreateLog
        {
            get;
            set;
        }

        public bool OutboundCanUpdateLog
        {
            get;
            set;
        }

        public bool OutboundFailureCanCreateLog
        {
            get;
            set;
        }

        public string OutboundFailurePopupEvent
        {
            get;
            set;
        }

        public string OutboundPopupEvent
        {
            get;
            set;
        }

        public string OutboundUpdateEvent
        {
            get;
            set;
        }
    }
}