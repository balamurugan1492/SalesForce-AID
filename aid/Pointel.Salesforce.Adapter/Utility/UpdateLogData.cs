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

using Genesyslab.Platform.Commons.Collections;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Utility
{
    internal class UpdateLogData
    {
        public string InteractionId = string.Empty;
        public string ContactRecordId = string.Empty;
        public string ContactActivityId = string.Empty;
        public string CaseRecordId = string.Empty;
        public string CaseActivityId = string.Empty;
        public string AccountRecordId = string.Empty;
        public string AccountActivityId = string.Empty;
        public string LeadRecordId = string.Empty;
        public string LeadActivityId = string.Empty;
        public string OpportunityRecordId = string.Empty;
        public string OpportunityActivityId = string.Empty;
        public string UserActivityId = string.Empty;
        public string ProfileActivityId = string.Empty;
        public Dictionary<string, KeyValueCollection> CustomObject = new Dictionary<string, KeyValueCollection>();
    }
}