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

using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    /// <summary>
    /// Comment: Holds SFDC Object's Data to Popup, Create,update Activity and New Record
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class SFDCData
    {
        #region Fields

        public string InteractionId;
        public string CommonSearchData;
        public string CommonPopupObjects;
        public string CommonSearchFormats;
        public string CommonSearchCondition;
        public LeadData LeadData;
        public ContactData ContactData;
        public AccountData AccountData;
        public CaseData CaseData;
        public OpportunityData OpportunityData;
        public IUserActivity UserActivityData;
        public CustomObjectData[] CustomObjectData;

        public List<XmlElement> ProfileActivityLogData
        {
            get;
            set;
        }

        public List<XmlElement> ProfileUpdateActivityLogData
        {
            get;
            set;
        }

        public List<XmlElement> ProfileActivityLogAppendData
        {
            get;
            set;
        }

        #endregion Fields

        #region ToString Method

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                stringBuilder.Append("InteractionId : " + InteractionId);
                stringBuilder.Append("\nCommonSearchData : " + CommonSearchData);
                stringBuilder.Append("\nCommonPopupObjects : " + CommonPopupObjects);
                stringBuilder.Append("\nCommonSearchFormats : " + CommonSearchFormats);
                stringBuilder.Append("\nLeadData : " + ((LeadData != null) ? LeadData.ToString() : "null"));
                stringBuilder.Append("\nContactData : " + ((ContactData != null) ? ContactData.ToString() : "null"));
                stringBuilder.Append("\nAccountData : " + ((AccountData != null) ? AccountData.ToString() : "null"));
                stringBuilder.Append("\nCaseData : " + ((CaseData != null) ? CaseData.ToString() : "null"));
                stringBuilder.Append("\nOpportunityData : " + ((OpportunityData != null) ? OpportunityData.ToString() : "null"));
                stringBuilder.Append("\nUserActivityData : " + ((UserActivityData != null) ? UserActivityData.ToString() : "null"));
                if (this.CustomObjectData != null)
                {
                    string cstdata = string.Empty;
                    foreach (CustomObjectData cst in this.CustomObjectData)
                    {
                        cstdata += "\n" + cst.ToString();
                    }
                    stringBuilder.Append("\nCustomObjectData : " + cstdata);
                }
                else
                {
                    stringBuilder.Append("\nCustomObjectData : " + "null");
                }
            }
            catch
            {
                return stringBuilder.ToString();
            }
            return stringBuilder.ToString();
        }

        #endregion ToString Method
    }
}