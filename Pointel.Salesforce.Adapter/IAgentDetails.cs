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

using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter
{
    /// <summary>
    /// Comment: Enables the third party application to supply Media Information to SFDCAdapter
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    public interface IAgentDetails
    {
        /// <summary>
        /// Property used to get ThisDN of an agent
        /// </summary>
        string ThisDN { get; }

        /// <summary>
        /// Property used to get CfgAgent Object
        /// </summary>
        CfgPerson Person { get; }

        /// <summary>
        /// Property used to get List of agentGroups of an agent
        /// </summary>
        List<CfgAgentGroup> AgentGroups { get; }

        /// <summary>
        /// Property used to get and application level configurations
        /// </summary>
        CfgApplication MyApplication { get; }

        /// <summary>
        /// Property to get agent is on call or not
        /// </summary>
        int IsAgentOnCall { get; }

        CurrentAgentStatus AgentStatus { get; }
    }
}