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
using System.Text;

namespace Pointel.Salesforce.Adapter.Configurations
{
    /// <summary>
    /// Comment: Holds the properties of SFDC Object's Voice Configuration Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>

    internal class VoiceOptions : CommonOptions
    {
        /// <summary>
        /// Outbound Call Configurations
        /// </summary>

        public bool CanCreateProfileActivityforConNoRecord { get; set; }

        public bool CanCreateProfileActivityforInbNoRecord { get; set; }

        public bool CanCreateProfileActivityforOutNoRecord { get; set; }

        public bool ConCanCreateMultimatchActivity
        {
            get;
            set;
        }

        public bool ConCanCreateNorecordActivity
        {
            get;
            set;
        }

        public bool ConCanPopupMultimatchActivity { get; set; }

        public bool ConCanPopupNorecordActivity { get; set; }

        public string ConMultiMatchRecordAction { get; set; }

        public string ConNoMatchRecordAction { get; set; }

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

        public string ConsultDialPlanPrefix
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

        public string ConsultSearchAttributeKeys
        {
            get;
            set;
        }

        /// <summary>
        /// Consult Call Configurations
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

        public bool InbCanCreateMultimatchActivity
        {
            get;
            set;
        }

        public bool InbCanCreateNorecordActivity
        {
            get;
            set;
        }

        public bool InbCanPopupMultimatchActivity { get; set; }

        public bool InbCanPopupNorecordActivity { get; set; }

        public string InbMultiMatchRecordAction { get; set; }

        public string InbNoMatchRecordAction { get; set; }

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

        public string OutboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string OutboundSearchUserDataKeys
        {
            get;
            set;
        }

        public string OutboundUpdateEvent
        {
            get;
            set;
        }

        public bool OutCanCreateMultimatchActivity
        {
            get;
            set;
        }

        public bool OutCanCreateNorecordActivity
        {
            get;
            set;
        }

        public bool OutCanPopupMultimatchActivity { get; set; }
        public bool OutCanPopupNorecordActivity { get; set; }
        public string OutMultiMatchRecordAction { get; set; }
        public string OutNoMatchRecordAction { get; set; }

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
            return txt.ToString();
        }

        #endregion ToString Method
    }
}