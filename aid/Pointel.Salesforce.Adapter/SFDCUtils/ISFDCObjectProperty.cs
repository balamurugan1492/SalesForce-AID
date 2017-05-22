#region Header

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

#endregion Header

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Comment: Provides Common properties to Popup and create activity for SFDC Objects
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    public interface ISFDCObjectProperty
    {
        #region Properties

        List<XmlElement> ActivityLogData
        {
            get;
            set;
        }

        string ActivityRecordID
        {
            get;
            set;
        }

        bool CanCreateMultiMatchActivityLog
        {
            get;
            set;
        }

        bool CanCreateNoRecordActivityLog
        {
            get;
            set;
        }

        bool CanCreateProfileActivityforConNoRecord
        {
            get;
            set;
        }

        bool CanCreateProfileActivityforInbNoRecord
        {
            get;
            set;
        }

        bool CanCreateProfileActivityforOutNoRecord
        {
            get;
            set;
        }

        bool CanPopupMultiMatchActivityLog
        {
            get;
            set;
        }

        bool CanPopupNoRecordActivityLog
        {
            get;
            set;
        }

        string ClickToDialRecordId
        {
            get;
            set;
        }

        bool CreateActvityLog
        {
            get;
            set;
        }

        bool CreateLogForNewRecord
        {
            get;
            set;
        }

        XmlElement[] CreateRecordFieldData
        {
            get;
            set;
        }

        int MaxRecordOpenCount
        {
            get;
            set;
        }

        string MultipleMatchRecord
        {
            get;
            set;
        }

        string NewRecordFieldIds
        {
            get;
            set;
        }

        string NoRecordFound
        {
            get;
            set;
        }

        string ObjectName
        {
            get;
            set;
        }

        bool OutboundDialedFromSFDC
        {
            get;
            set;
        }

        string PhoneNumberSearchFormat
        {
            get;
            set;
        }

        string RecordID
        {
            get;
            set;
        }

        string SearchCondition
        {
            get;
            set;
        }

        string SearchData
        {
            get;
            set;
        }

        string SearchFields
        {
            get;
            set;
        }

        string SearchpageMode
        {
            get;
            set;
        }

        bool UpdateActivityLog
        {
            get;
            set;
        }

        List<XmlElement> UpdateActivityLogData
        {
            get;
            set;
        }

        bool UpdateRecordFields
        {
            get;
            set;
        }

        XmlElement[] UpdateRecordFieldsData
        {
            get;
            set;
        }

        #endregion Properties
    }
}