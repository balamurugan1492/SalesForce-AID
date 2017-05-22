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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Comment: Holds the Search, Activity Log , New SFDC Record Data for CustomObject
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class CustomObjectData : ICustomObject
    {
        #region Properties

        public List<XmlElement> ActivityLogData
        {
            get;
            set;
        }

        public string ActivityRecordID
        {
            get;
            set;
        }

        public bool CanCreateMultiMatchActivityLog
        {
            get;
            set;
        }

        public bool CanCreateNoRecordActivityLog
        {
            get;
            set;
        }

        public bool CanCreateProfileActivityforConNoRecord
        {
            get;
            set;
        }

        public bool CanCreateProfileActivityforInbNoRecord
        {
            get;
            set;
        }

        public bool CanCreateProfileActivityforOutNoRecord
        {
            get;
            set;
        }

        public bool CanPopupMultiMatchActivityLog
        {
            get;
            set;
        }

        public bool CanPopupNoRecordActivityLog
        {
            get;
            set;
        }

        public string ClickToDialRecordId
        {
            get;
            set;
        }

        public bool CreateActvityLog
        {
            get;
            set;
        }

        public bool CreateLogForNewRecord
        {
            get;
            set;
        }

        public XmlElement[] CreateRecordFieldData
        {
            get;
            set;
        }

        public string CustomObjectURL
        {
            get;
            set;
        }

        public int MaxRecordOpenCount
        {
            get;
            set;
        }

        public string MultipleMatchRecord
        {
            get;
            set;
        }

        public string NewRecordFieldIds
        {
            get;
            set;
        }

        public string NoRecordFound
        {
            get;
            set;
        }

        public string ObjectName
        {
            get;
            set;
        }

        public bool OutboundDialedFromSFDC
        {
            get;
            set;
        }

        public string PhoneNumberSearchFormat
        {
            get;
            set;
        }

        public string RecordID
        {
            get;
            set;
        }

        public string SearchCondition
        {
            get;
            set;
        }

        public string SearchData
        {
            get;
            set;
        }

        public string SearchFields
        {
            get;
            set;
        }

        public string SearchpageMode
        {
            get;
            set;
        }

        public bool UpdateActivityLog
        {
            get;
            set;
        }

        public List<XmlElement> UpdateActivityLogData
        {
            get;
            set;
        }

        public bool UpdateRecordFields
        {
            get;
            set;
        }

        public XmlElement[] UpdateRecordFieldsData
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();
            try
            {
                foreach (System.ComponentModel.PropertyDescriptor descriptor in System.ComponentModel.TypeDescriptor.GetProperties(this))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(this);
                    txt.Append(String.Format("{0} = {1}\n", name, value));
                }
            }
            catch (Exception)
            {
                return "";
            }
            return txt.ToString();
        }

        #endregion Methods
    }
}