﻿#region Header

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

    internal class CommonOptions
    {
        #region Fields

        private int maxRecordsOpen;
        private string searchPageMode;

        #endregion Fields

        #region Properties

        public bool CanCreateLogForNewRecordCreate
        {
            get;
            set;
        }

        public bool CanUpdateRecordData
        {
            get;
            set;
        }

        public string CustomObjectURL
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

        public string InboundSearchAttributeKeys
        {
            get;
            set;
        }

        public string InboundSearchUserDataKeys
        {
            get;
            set;
        }

        public string InboundUpdateEvent
        {
            get;
            set;
        }

        public int MaxNosRecordOpen
        {
            get
            {
                if (maxRecordsOpen > 50)
                {
                    return 50;
                }
                else
                    return maxRecordsOpen;
            }
            set
            {
                maxRecordsOpen = value;
            }
        }

        public string NewrecordFieldIds
        {
            get;
            set;
        }

        public string ObjectName
        {
            get;
            set;
        }

        public string PhoneNumberSearchFormat
        {
            get;
            set;
        }

        public string SearchCondition
        {
            get;
            set;
        }

        public string SearchFields
        {
            get;
            set;
        }

        public string SearchPageMode
        {
            get
            {
                return searchPageMode;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (value.ToLower() != "all")
                    {
                        string[] temp = value.Split(',');
                        searchPageMode = "&";
                        foreach (string key in temp)
                        {
                            if (key.ToLower() == "contact")
                            {
                                searchPageMode += "sen=003&";
                            }
                            else if (key.ToLower() == "account")
                            {
                                searchPageMode += "sen=001&";
                            }
                            else if (key.ToLower() == "lead")
                            {
                                searchPageMode += "sen=00Q&";
                            }
                            else if (key.ToLower() == "case")
                            {
                                searchPageMode += "sen=500&";
                            }
                            else if (key.ToLower() == "opportunity")
                            {
                                searchPageMode += "sen=006&";
                            }
                            else
                            {
                                searchPageMode += "sen=" + key + "&";
                            }
                        }
                    }
                    else
                        searchPageMode = "&";
                }
            }
        }

        public string SearchPriority
        {
            get;
            set;
        }

        #endregion Properties
    }
}