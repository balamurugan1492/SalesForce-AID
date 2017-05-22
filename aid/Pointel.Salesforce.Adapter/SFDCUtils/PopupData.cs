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

using System.Text;

namespace Pointel.Salesforce.Adapter.SFDCUtils
{
    internal class PopupData
    {
        public string InteractionId;
        public string PopupUrl;
        public string ObjectName;

        #region ToString Method

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                stringBuilder.Append("\nInteractionId : " + InteractionId);
                stringBuilder.Append("\nObjectName : " + ObjectName);
                stringBuilder.Append("\nPopupUrl : " + PopupUrl);
                return stringBuilder.ToString();
            }
            catch
            {
            }
            return "";
        }

        #endregion ToString Method
    }
}