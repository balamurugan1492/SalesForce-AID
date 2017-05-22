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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    /// <summary>
    /// Comment:  Holds the  Acticity Log Data for User Level Activity .
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class UserActivityData : IUserActivity
    {
        #region Fields Declaration

        public string ObjectName { get; set; }
        public string RecordID { get; set; }
        public bool CreateActvityLog { get; set; }
        public bool UpdateActivityLog { get; set; }
        public string InteractionId { get; set; }

        public List<XmlElement> ActivityLogData
        {
            get;
            set;
        }

        public List<XmlElement> UpdateActivityLogData
        {
            get;
            set;
        }

        #endregion Fields Declaration

        #region ToString Method

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
            return txt.ToString(); ;
        }

        #endregion ToString Method
    }
}