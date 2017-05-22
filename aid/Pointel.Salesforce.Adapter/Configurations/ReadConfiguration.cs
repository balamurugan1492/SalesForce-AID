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

using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Salesforce.Adapter.LogMessage;
using System;
using System.Collections.Generic;

namespace Pointel.Salesforce.Adapter.Configurations
{
    /// <summary>
    /// Comment: Reads the SFDC Configuration using iWS Hierarchy
    /// Last Modified: 25-08-2015
    /// Created by: Pointel Inc
    /// </summary>
    internal class ReadConfiguration
    {
        #region Fields Declarations

        private Log logger = null;
        private static ReadConfiguration currentObject = null;

        #endregion Fields Declarations

        #region Constructor

        private ReadConfiguration()
        {
            this.logger = Log.GenInstance();
        }

        #endregion Constructor

        #region GetInstance

        public static ReadConfiguration GetInstance()
        {
            if (currentObject == null)
            {
                currentObject = new ReadConfiguration();
            }
            return currentObject;
        }

        #endregion GetInstance

        #region ReadGeneralConfigurations

        public KeyValueCollection ReadGeneralConfigurations(CfgApplication myApplication, List<CfgAgentGroup> agentGroups, CfgPerson personObject)
        {
            try
            {
                logger.Info("ReadGeneralConfigurations : Reading salesforce-integration Configuration");
                KeyValueCollection SFDCConfig = null;

                #region Reading General Configurations

                try
                {
                    if (myApplication != null)
                    {
                        //Reading Agent Level Configurations
                        if (myApplication.Options.ContainsKey("salesforce-integration"))
                        {
                            SFDCConfig = myApplication.Options.GetAsKeyValueCollection("salesforce-integration");
                            this.logger.Info("ReadGeneralConfigurations : " + (SFDCConfig != null ? "Read Configuration at Application Level" : " No configuration found at Application Level"));
                        }
                        else
                        {
                            this.logger.Info("ReadGeneralConfigurations : SFDC General Configuration is not found at Application Level.");
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadGeneralConfigurations : Error occurred while reading configuration at application level :" + generalException.ToString());
                }

                //Reading AgentGroupLevel Configurartions

                try
                {
                    if (agentGroups != null)
                    {
                        foreach (CfgAgentGroup agentgroup in agentGroups)
                        {
                            if (agentgroup.GroupInfo.UserProperties.ContainsKey("salesforce-integration"))
                            {
                                KeyValueCollection agentGroupConfig = agentgroup.GroupInfo.UserProperties.GetAsKeyValueCollection("salesforce-integration");
                                if (agentGroupConfig != null)
                                {
                                    if (SFDCConfig != null)
                                    {
                                        foreach (string key in agentGroupConfig.AllKeys)
                                        {
                                            if (SFDCConfig.ContainsKey(key))
                                            {
                                                SFDCConfig.Set(key, agentGroupConfig.GetAsString(key));
                                            }
                                            else
                                            {
                                                SFDCConfig.Add(key, agentGroupConfig.GetAsString(key));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SFDCConfig = agentGroupConfig;
                                    }
                                    logger.Info("ReadGeneralConfigurations : SFDC configuration taken from Agent Group : " + agentgroup.GroupInfo.Name);
                                    break;
                                }
                            }
                            else
                            {
                                logger.Info("ReadGeneralConfigurations : No SFDC configuration found at Agent Group : " + agentgroup.GroupInfo.Name);
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadGeneralConfigurations : Error occurred while reading configuration at AgentGroup level :" + generalException.ToString());
                }

                //Reading Agent Level Configurations

                try
                {
                    if (personObject.UserProperties.ContainsKey("salesforce-integration"))
                    {
                        KeyValueCollection agentConfig = personObject.UserProperties.GetAsKeyValueCollection("salesforce-integration");
                        if (agentConfig != null)
                        {
                            if (SFDCConfig != null)
                            {
                                foreach (string key in agentConfig.AllKeys)
                                {
                                    if (SFDCConfig.ContainsKey(key))
                                    {
                                        SFDCConfig.Set(key, agentConfig.GetAsString(key));
                                    }
                                    else
                                    {
                                        SFDCConfig.Add(key, agentConfig.GetAsString(key));
                                    }
                                }
                            }
                            else
                            {
                                SFDCConfig = agentConfig;
                            }
                        }
                    }
                    else
                    {
                        logger.Info("ReadGeneralConfigurations :  No SFDC configuration found at Agent Level");
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadGeneralConfigurations : Error occurred while reading configuration at Agent level :" + generalException.ToString());
                }

                #endregion Reading General Configurations

                if (SFDCConfig != null)
                    logger.Info("ReadGeneralConfigurations : General salesforce-integration Configuration Read : " + SFDCConfig.ToString());

                return SFDCConfig;
            }
            catch (Exception generalException)
            {
                logger.Error("ReadGeneralConfigurations : Error Occurred while reading SFDC General Configuration : " + generalException.ToString());
            }
            return null;
        }

        #endregion ReadGeneralConfigurations

        #region ReadObjectBasedConfigurations

        public KeyValueCollection ReadSFDCObjectConfig(CfgApplication myApplication, List<CfgAgentGroup> agentGroups, CfgPerson personObject, string sectionName)
        {
            try
            {
                logger.Info("ReadSFDCObjectConfig : Reading SFDC Objects Configuration");
                logger.Info("ReadSFDCObjectConfig : Object Name : " + sectionName);
                KeyValueCollection SFDCConfig = null;

                #region Reading General Configurations

                try
                {
                    if (myApplication != null)
                    {
                        //Reading Agent Level Configurations
                        if (myApplication.UserProperties.ContainsKey(sectionName))
                        {
                            SFDCConfig = myApplication.UserProperties.GetAsKeyValueCollection(sectionName);
                            this.logger.Info("ReadSFDCObjectConfig : " + (SFDCConfig != null ? "Read " + sectionName + " Configuration at Application Level" : " No configuration found at Application Level"));
                        }
                        else
                        {
                            this.logger.Info("ReadSFDCObjectConfig : " + sectionName + " Configuration is not found at Application Level.");
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadSFDCObjectConfig : Error occurred while reading configuration at application level :" + generalException.ToString());
                }

                //Reading AgentGroupLevel Configurartions

                try
                {
                    if (agentGroups != null)
                    {
                        foreach (CfgAgentGroup agentgroup in agentGroups)
                        {
                            if (agentgroup.GroupInfo.UserProperties.ContainsKey(sectionName))
                            {
                                KeyValueCollection agentGroupConfig = agentgroup.GroupInfo.UserProperties.GetAsKeyValueCollection(sectionName);
                                if (agentGroupConfig != null)
                                {
                                    if (SFDCConfig != null)
                                    {
                                        foreach (string key in agentGroupConfig.AllKeys)
                                        {
                                            if (SFDCConfig.ContainsKey(key))
                                            {
                                                SFDCConfig.Set(key, agentGroupConfig.GetAsString(key));
                                            }
                                            else
                                            {
                                                SFDCConfig.Add(key, agentGroupConfig.GetAsString(key));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SFDCConfig = agentGroupConfig;
                                    }
                                    logger.Info("ReadSFDCObjectConfig : " + sectionName + " configuration taken from Agent Group : " + agentgroup.GroupInfo.Name);
                                    break;
                                }
                            }
                            else
                            {
                                logger.Info("ReadSFDCObjectConfig : " + sectionName + " configuration is not found at Agent Group : " + agentgroup.GroupInfo.Name);
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadSFDCObjectConfig : Error occurred while reading configuration at AgentGroup level :" + generalException.ToString());
                }

                //Reading Agent Level Configurations

                try
                {
                    if (personObject.UserProperties.ContainsKey(sectionName))
                    {
                        KeyValueCollection agentConfig = personObject.UserProperties.GetAsKeyValueCollection(sectionName);
                        if (agentConfig != null)
                        {
                            if (SFDCConfig != null)
                            {
                                foreach (string key in agentConfig.AllKeys)
                                {
                                    if (SFDCConfig.ContainsKey(key))
                                    {
                                        SFDCConfig.Set(key, agentConfig.GetAsString(key));
                                    }
                                    else
                                    {
                                        SFDCConfig.Add(key, agentConfig.GetAsString(key));
                                    }
                                }
                            }
                            else
                            {
                                SFDCConfig = agentConfig;
                            }
                        }
                    }
                    else
                    {
                        logger.Info("ReadSFDCObjectConfig : " + sectionName + " configuration is not found at Agent Level");
                    }
                }
                catch (Exception generalException)
                {
                    this.logger.Error("ReadSFDCObjectConfig : Error occurred while reading configuration at Agent level :" + generalException.ToString());
                }

                #endregion Reading General Configurations

                if (SFDCConfig != null)
                    logger.Info("ReadSFDCObjectConfig : " + sectionName + " Configuration Data : " + SFDCConfig.ToString());

                return SFDCConfig;
            }
            catch (Exception generalException)
            {
                logger.Error("ReadSFDCObjectConfig : Error Occurred while reading SFDC Configuration : " + generalException.ToString());
            }
            return null;
        }

        #endregion ReadObjectBasedConfigurations

        #region ReadBusinessAttributeConfigurations

        public KeyValueCollection ReadBusinessAttribuiteConfig(IConfService configService, int tenantDBID, string attributeName, string attributeValueName)
        {
            try
            {
                logger.Info("ReadBusinessAttribuiteConfig : Reading Business Attribute Configurations.............");
                logger.Info("ReadBusinessAttribuiteConfig : Business Attribute Name : " + attributeName);
                logger.Info("ReadBusinessAttribuiteConfig : Business Attribute Value Name : " + attributeValueName);
                logger.Info("ReadBusinessAttribuiteConfig : Tenant DBID : " + tenantDBID.ToString());

                if (configService != null)
                {
                    CfgEnumeratorQuery enumeratorQuery = new CfgEnumeratorQuery();
                    enumeratorQuery.TenantDbid = tenantDBID;
                    enumeratorQuery.Name = attributeName;

                    CfgEnumerator _cfgEnumerator = configService.RetrieveObject<CfgEnumerator>(enumeratorQuery);

                    if (_cfgEnumerator != null)
                    {
                        CfgEnumeratorValueQuery enumeratorValueQuery = new CfgEnumeratorValueQuery();
                        enumeratorValueQuery.EnumeratorDbid = _cfgEnumerator.DBID;
                        enumeratorValueQuery.Name = attributeValueName;
                        CfgEnumeratorValue _cfgEnumeratorValue = configService.RetrieveObject<CfgEnumeratorValue>(enumeratorValueQuery);
                        if (_cfgEnumeratorValue != null)
                        {
                            logger.Info("ReadBusinessAttribuiteConfig : Configuration Data : " + Convert.ToString(_cfgEnumeratorValue.UserProperties));
                            return _cfgEnumeratorValue.UserProperties;
                        }
                        else
                        {
                            logger.Info("ReadBusinessAttribuiteConfig : Reading Business Attribute Value " + attributeValueName + " Failed ");
                        }
                    }
                    else
                    {
                        logger.Info("ReadBusinessAttribuiteConfig : Reading Business Attribute " + attributeName + " Failed ");
                    }
                }
                else
                {
                    logger.Info("ReadBusinessAttribuiteConfig : Can not read Business Attribute " + attributeName + ", because ConfService Object is null ");
                }
            }
            catch (Exception generalException)
            {
                this.logger.Error("ReadBusinessAttribuiteConfig : Error occurred while reading Business Attribute Configuration :" + generalException.ToString());
            }
            return null;
        }

        #endregion ReadBusinessAttributeConfigurations
    }
}