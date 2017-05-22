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

namespace Pointel.Salesforce.Adapter.Configurations
{
    using System;
    using System.Text;

    internal class ChatOptions : CommonOptions
    {
        #region Properties

        public string ConMultiMatchRecordAction
        {
            get;
            set;
        }

        public string ConNoMatchRecordAction
        {
            get;
            set;
        }

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

        public string ConsultPopupEvent
        {
            get;
            set;
        }

        public string ConsultSearchAttributeKeys
        {
            get;
            set;
        }

        /// <summary>
        /// Consult Chat Configurations
        /// </summary>
        public string ConsultSearchUserDataKeys
        {
            get;
            set;
        }

        public string ConsultUpdateEvent
        {
            get;
            set;
        }

        public string InbMultiMatchRecordAction
        {
            get;
            set;
        }

        public string InbNoMatchRecordAction
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