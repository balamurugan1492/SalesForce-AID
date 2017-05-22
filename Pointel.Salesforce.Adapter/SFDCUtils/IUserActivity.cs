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
using System.Linq;
using System.Text;
using System.Xml;

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    /// <summary>
    /// Comment: Provides necessary properties to SFDC User Level Activity
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
   public interface IUserActivity
    {
        string ObjectName
        {
            get;
            set;
        }       
        string RecordID
        {
            get;
            set;
        }  
        bool CreateActvityLog
        {
            get;
            set;
        }
        List<XmlElement> ActivityLogData
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
    }
}
