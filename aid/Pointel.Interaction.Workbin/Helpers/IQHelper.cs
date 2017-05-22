using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Configuration.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class IQHelper : IDisposable
    {
        private Pointel.Logger.Core.ILog _logger = null;

        public IQHelper()
        {
            _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        }


        public List<IQMenuItem> LoadIQ()
        {
            List<IQMenuItem> lstIQ = new List<IQMenuItem>();
            bool isSelected = false;
            try
            {
                //Getting filter names
                string[] filters = ConfigContainer.Instance().GetAsString("interaction-management.filters").Split(',');

                foreach (var filter in filters)
                {
                    // Read the filter section and add it in ConfigContainer(CME)
                    if (!ConfigContainer.Instance().AllKeys.Contains(filter))
                        ConfigContainer.Instance().ReadSection(filter);

                    // Add the filter if it has display name, proper condition and category
                    if (ConfigContainer.Instance().AllKeys.Contains(filter) && ConfigContainer.Instance().GetValue(filter) != null)
                    {
                        var readSection = (KeyValueCollection)ConfigContainer.Instance().GetValue(filter);

                        // Check category is empty or not
                        if (readSection.AllKeys.Contains("category") && !string.IsNullOrEmpty(readSection.GetAsString("category")))
                        {
                            bool isAddCategoryinList = true;
                            // Check Category is already exist or not
                            var categoryMenu = lstIQ.Where(x => x.DisplayName == readSection["category"].ToString()
                                && x.IQTYPE == Pointel.Interaction.Workbin.Helpers.IQTYPES.Category).FirstOrDefault();

                            if (categoryMenu == null)
                            {
                                //Add new Category menu item
                                categoryMenu = new IQMenuItem()
                                {
                                    DisplayName = readSection["category"].ToString(),
                                    IQTYPE = Pointel.Interaction.Workbin.Helpers.IQTYPES.Category,
                                    ImagePath = "/Agent.Interaction.Desktop;component/Images/DefaultFolder.png"
                                };
                            }
                            else
                                isAddCategoryinList = false;


                            // Check display name is empty or not
                            if (readSection.AllKeys.Contains("display-name") && readSection["display-name"] != null && !string.IsNullOrEmpty(readSection["display-name"].ToString()))
                            {
                                // Check condition is empty or not
                                if (readSection.AllKeys.Contains("condition") && readSection["condition"] != null && !string.IsNullOrEmpty(readSection["condition"].ToString()))
                                {
                                    // Check queues is empty or not
                                    if (readSection.AllKeys.Contains("queues") && readSection["queues"] != null && !string.IsNullOrEmpty(readSection["queues"].ToString()))
                                    {
                                        bool isAddConditioninList = true;
                                        // Check condition menu is already exist or not
                                        IQMenuItem conditionMenu = null;
                                        if (lstIQ.Count > 0 && !isAddCategoryinList)
                                        {

                                            conditionMenu = categoryMenu.Items.Where(x => x.DisplayName == readSection["display-name"].ToString()
                                                                                           && x.IQTYPE == Pointel.Interaction.Workbin.Helpers.IQTYPES.Condition
                                                                                           && x.ImagePath == "/Agent.Interaction.Desktop;component/Images/DefaultFolder.png"
                                                                                           && x.Category == readSection["category"].ToString())
                                                                                           .FirstOrDefault();
                                        }
                                        if (conditionMenu == null)
                                        {
                                            //Add new condition menu item
                                            conditionMenu = new IQMenuItem()
                                            {
                                                DisplayName = readSection["display-name"].ToString(),
                                                IQTYPE = Pointel.Interaction.Workbin.Helpers.IQTYPES.Condition,
                                                ImagePath = "/Agent.Interaction.Desktop;component/Images/DefaultFolder.png",
                                                Category = readSection["category"].ToString()
                                            };
                                        }
                                        else
                                            isAddConditioninList = false;

                                        string[] queues = readSection["queues"].ToString().Split(',');
                                        foreach (var queue in queues)
                                        {
                                            if (!string.IsNullOrEmpty(queue.Trim()) && IsValidQueue(queue))
                                            {
                                                if (lstIQ.Count > 0)
                                                {
                                                    // Check the condition is already exist or not
                                                    bool IsIQConditionAlreadyExist = lstIQ.Where(
                                                           x => x.Items.Where(y => y.Items.Where(z => z.Condition == readSection.GetAsString("condition") + " AND Queue='" + queue + "'"
                                                           ).ToList().Count > 0).ToList().Count > 0).ToList().Count > 0;


                                                    if (IsIQConditionAlreadyExist)
                                                        continue;
                                                }

                                                //Add new queue menu item
                                                IQMenuItem queueMenu = new IQMenuItem()
                                                {
                                                    DisplayName = queue,
                                                    IQTYPE = Pointel.Interaction.Workbin.Helpers.IQTYPES.Queue,
                                                    ImagePath = "/Agent.Interaction.Desktop;component/Images/Queue.png",
                                                    Condition = readSection["condition"].ToString() + " AND Queue='" + queue + "'"
                                                };

                                                // Add Displayed Coloumns to queue if exist
                                                if (readSection.AllKeys.Contains("displayed-columns") && readSection["displayed-columns"] != null && !string.IsNullOrEmpty(readSection["displayed-columns"].ToString()))
                                                    queueMenu.DisplayedColoumns = readSection["displayed-columns"].ToString();

                                                // Add Bussiness Attribute name for filtering case data to queue if exist
                                                if (readSection.AllKeys.Contains("case-data.business-attribute") && !string.IsNullOrEmpty(readSection["case-data.business-attribute"].ToString()))
                                                { 
                                                    queueMenu.CaseDataBussinessAttribute = readSection["case-data.business-attribute"].ToString();
                                                    queueMenu.CaseDataToFilter=new List<string>();
                                                    ConfigManager objConfigManager = new ConfigManager();
                                                    if(ConfigContainer.Instance().AllKeys.Contains("interaction.casedata-object-name"))
                                                    {
                                                        CfgEnumeratorValue objBusinessAttribute = objConfigManager.GetBusinessAttribute(
                                                            ConfigContainer.Instance().GetAsString("interaction.casedata-object-name"), queueMenu.CaseDataBussinessAttribute);
                                                        if (objBusinessAttribute != null && objBusinessAttribute.UserProperties.ContainsKey("case-data-filter"))
                                                        {
                                                            foreach (string value in objBusinessAttribute.UserProperties.GetAsKeyValueCollection("case-data-filter").AllValues)
                                                                queueMenu.CaseDataToFilter.Add(value);
                                                        }
                                                    }                                              
                                                }
                                                else // Suppose the case data not configured need to read from parent business attribute key.
                                                {

                                                }

                                                // Add Quick Search Attributes to queue if exist
                                                if (readSection.AllKeys.Contains("quick-search-attributes") && readSection["quick-search-attributes"] != null && !string.IsNullOrEmpty(readSection["quick-search-attributes"].ToString()))
                                                    queueMenu.QuickSearchAttributes = readSection["quick-search-attributes"].ToString();

                                                if(!isSelected)                                               
                                                    queueMenu.IsSelected = true;
                                                  
                                                conditionMenu.Items.Add(queueMenu);

                                            }
                                        }
                                        if (isAddConditioninList && conditionMenu.Items != null && conditionMenu.Items.Count > 0)
                                        {
                                            if (!isSelected)
                                            {
                                                categoryMenu.IsExpanded = true;
                                                conditionMenu.IsExpanded = true;
                                                isSelected = true;
                                            }
                                            categoryMenu.Items.Add(conditionMenu);
                                        }
                                            
                                    }
                                    else
                                    {
                                        if (lstIQ.Count > 0)
                                        {
                                            // Check the condition is already exist or not
                                            bool IsIQItemAlreadyExist = lstIQ.Where(
                                               x => x.Items.Where(
                                                   y => y.Condition == readSection.GetAsString("condition")
                                                   ).ToList().Count > 0).ToList().Count > 0;


                                            if (IsIQItemAlreadyExist)
                                                goto NextFilter;

                                        }

                                        //Add new condition menu item
                                        IQMenuItem conditionMenu = new IQMenuItem()
                                        {
                                            DisplayName = readSection["display-name"].ToString(),
                                            IQTYPE = Pointel.Interaction.Workbin.Helpers.IQTYPES.Condition,
                                            Category = readSection["category"].ToString()
                                        };

                                        conditionMenu.ImagePath = "/Agent.Interaction.Desktop;component/Images/DefaultFile.png";
                                        conditionMenu.Condition = readSection["condition"].ToString();

                                        // Add Displayed Coloumns to condition if exist
                                        if (readSection.AllKeys.Contains("displayed-columns") && readSection["displayed-columns"] != null && !string.IsNullOrEmpty(readSection["displayed-columns"].ToString()))
                                            conditionMenu.DisplayedColoumns = readSection["displayed-columns"].ToString();

                                        // Add Bussiness Attribute name for filtering case data to condition if exist
                                        if (readSection.AllKeys.Contains("case-data.business-attribute") && readSection["case-data.business-attribute"] != null && !string.IsNullOrEmpty(readSection["case-data.business-attribute"].ToString()))
                                            conditionMenu.CaseDataBussinessAttribute = readSection["case-data.business-attribute"].ToString();

                                        // Add Quick Search Attributes to condition if exist
                                        if (readSection.AllKeys.Contains("quick-search-attributes") && readSection["quick-search-attributes"] != null && !string.IsNullOrEmpty(readSection["quick-search-attributes"].ToString()))
                                            conditionMenu.QuickSearchAttributes = readSection["quick-search-attributes"].ToString();
                                        
                                        if (!isSelected)     
                                        {
                                            conditionMenu.IsSelected = true;                                        
                                            categoryMenu.IsExpanded = true;
                                            isSelected = true;
                                        }

                                        categoryMenu.Items.Add(conditionMenu);
                                    }
                                    if (isAddCategoryinList && categoryMenu.Items != null && categoryMenu.Items.Count > 0)
                                        lstIQ.Add(categoryMenu);
                                }
                                else
                                    goto NextFilter; // No Condition
                            }
                            else
                                goto NextFilter; // No Display Name                                 
                        }
                        else
                            goto NextFilter;  // No Category
                    }
                NextFilter:
                    continue;
                }

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as :" + generalException.ToString());
            }
            return lstIQ;
        }

        private bool IsValidQueue(string queueName)
        {
            try
            {
                CfgScriptQuery qScripts = new CfgScriptQuery();
                qScripts.ScriptType = Genesyslab.Platform.Configuration.Protocols.Types.CfgScriptType.CFGInteractionQueue;
                qScripts.Name = queueName;
                qScripts.TenantDbid = ConfigContainer.Instance().TenantDbId;
                CfgScript interactionqueue = ((Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService)ConfigContainer.Instance().ConfServiceObject).RetrieveObject<CfgScript>(qScripts);
                if (interactionqueue != null)
                    return true;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred as :" + generalException.ToString());
            }
            return false;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }

}
