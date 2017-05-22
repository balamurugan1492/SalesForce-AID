/*
* =====================================
* Pointel.Interaction.Workbin.ApplicationDetails.ApplicationDetails
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* ====================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Pointel.Interaction.Workbin.Utility;
using Pointel.Configuration.Manager;
using System;

namespace Pointel.Interaction.Workbin.ApplicationDetails
{
    /// <summary>
    /// Perform the configuration Operation.
    /// </summary>
    public class ApplicationDetails
    {
        /// <summary>
        /// The Will Retrieve the Pperson Id of which group the login agent is superadmin.
        /// If the User is Agent Means it will not Performed.
        /// </summary>
        public static List<string> GetMyTeamAgentId()
        {
            List<string> empId = new List<string>();
            CfgAgentGroupQuery agentGroupQuery = new CfgAgentGroupQuery();
            agentGroupQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;
            ICollection<CfgAgentGroup> agentGroups = ConfigContainer.Instance().ConfServiceObject.RetrieveMultipleObjects<CfgAgentGroup>(agentGroupQuery);
            if (agentGroups != null && agentGroups.Count > 0)
            {
                //this is added by sakthikumar to determine the age is admin or not.
                WorkbinUtility.Instance().AgentGroup = agentGroups.Where(x => x != null && x.GroupInfo != null && x.GroupInfo.Managers != null && x.GroupInfo.Managers.Where(y => y.EmployeeID.Equals(ConfigContainer.Instance().AgentId)).ToList().Count > 0).ToList();
                WorkbinUtility.Instance().IsSuperadmin = WorkbinUtility.Instance().AgentGroup.Count > 0;
                foreach (CfgAgentGroup agentgroup in WorkbinUtility.Instance().AgentGroup)
                {
                    foreach (CfgPerson person in agentgroup.Agents)
                    {
                        empId.Add(person.EmployeeID);
                    }
                }
            }
            return empId;
        }

        /// <summary>
        /// The method will used to Read the Workbin name of the Agent and Team Workbin
        /// </summary>
        public static void LoadWorkbin()
        {
             Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
            try
            {
                //Added by sakthi to load workbins.
                List<string> interactionWorkbin = WorkbinConverter.GetWorkbinList();
                WorkbinUtility.Instance().TeamWorkbinList.Clear();
                WorkbinUtility.Instance().PersonalWorkbinList.Clear();
                if (interactionWorkbin != null && interactionWorkbin.Count > 0)
                {
                    string[] listofKeys = ConfigContainer.Instance().AllKeys.Where(x => x.ToLower().StartsWith("workbin.")
                        && x.ToCharArray().Where(y => y.Equals('.')).ToList().Count == 2 && !x.Contains("..")).ToArray<string>();

                    foreach (string keys in listofKeys)
                    {
                        string workbinName = ConfigContainer.Instance().GetValue(keys);
                        if (interactionWorkbin.Where(x => x.Equals(workbinName)).ToList().Count > 0 && !WorkbinUtility.Instance().PersonalWorkbinList.ContainsKey(workbinName))
                        {
                            WorkbinUtility.Instance().PersonalWorkbinList.Add(workbinName, "From,To,Subject,Received");
                            if (ConfigContainer.Instance().AllKeys.Contains(keys + ".displayed-columns") && !string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue(keys + ".displayed-columns"))))
                            {
                                WorkbinUtility.Instance().PersonalWorkbinList[workbinName] = (string)ConfigContainer.Instance().GetValue(keys + ".displayed-columns");
                            }
                            //workbin.email.in-progress
                            else if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress")
                                && string.Compare((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress"), workbinName) == 0)
                            {
                                WorkbinUtility.Instance().PersonalWorkbinList[workbinName] = "From,Subject,Received";
                            }
                            else if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft")
                                && string.Compare((string)ConfigContainer.Instance().GetValue("workbin.email.draft"), workbinName) == 0)
                            {
                                WorkbinUtility.Instance().PersonalWorkbinList[workbinName] = "To,Subject,Submitted";
                            }

                        }

                    }

                    string key = "workbin.teamworkbin";
                    if (ConfigContainer.Instance().AllKeys.Contains(key) && !string.IsNullOrEmpty(ConfigContainer.Instance().GetValue(key)))
                    {
                        foreach (string workbinName in ConfigContainer.Instance().GetValue(key).Split(','))
                        {
                            if (interactionWorkbin.Where(x => x.Equals(workbinName)).ToList().Count > 0)
                                WorkbinUtility.Instance().TeamWorkbinList.Add(workbinName, "From,To,Subject,Received");
                            if (ConfigContainer.Instance().AllKeys.Contains(key + ".displayed-columns") && !string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue(key + ".displayed-columns"))))
                            {
                                WorkbinUtility.Instance().PersonalWorkbinList[workbinName] = (string)ConfigContainer.Instance().GetValue(key + ".displayed-columns");
                            }
                            //workbin.email.in-progress
                            else if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress")
                                && string.Compare((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress"), workbinName) == 0)
                            {
                                WorkbinUtility.Instance().TeamWorkbinList[workbinName] = "From,Subject,Received";
                            }
                            else if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.draft")
                                && string.Compare((string)ConfigContainer.Instance().GetValue("workbin.email.draft"), workbinName) == 0)
                            {
                                WorkbinUtility.Instance().TeamWorkbinList[workbinName] = "To,Subject,Submitted";
                            }
                        }
                    }
                    else if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress"))
                    {
                        WorkbinUtility.Instance().TeamWorkbinList.Add((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress"), "From,Subject,Received");
                        if (ConfigContainer.Instance().AllKeys.Contains("workbin.email.in-progress.displayed-columns") && !string.IsNullOrEmpty(((string)ConfigContainer.Instance().GetValue("workbin.email.in-progress.displayed-columns"))))
                        {
                            WorkbinUtility.Instance().PersonalWorkbinList[(string)ConfigContainer.Instance().GetValue("workbin.email.in-progress")] = (string)ConfigContainer.Instance().GetValue("workbin.email.in-progress.displayed-columns");
                        }
                    }
                    WorkbinUtility.Instance().IsAgentLoginIXN = true;
                }
                else
                {
                    string[] listofKeys = ConfigContainer.Instance().AllKeys.Where(x => x.ToLower().StartsWith("workbin.")
                        && x.ToCharArray().Where(y => y.Equals('.')).ToList().Count == 2 && !x.Contains("..")).ToArray<string>();
                    WorkbinUtility.Instance().Workbins = new Dictionary<string, string>();
                    foreach (string keys in listofKeys)
                    {
                        string workbinName = ConfigContainer.Instance().GetValue(keys);
                        WorkbinUtility.Instance().Workbins.Add(keys, workbinName);
                    }
                    WorkbinUtility.Instance().IsAgentLoginIXN = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }
    }
}