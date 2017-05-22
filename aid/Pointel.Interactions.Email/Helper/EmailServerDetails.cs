#region Header

/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 15-May-2015
* Author     : Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

#endregion Header


internal delegate void NotifyFromAddress();

namespace Pointel.Interactions.Email.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Controls;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Configuration.Protocols.Types;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Email.DataContext;

    /// <summary>
    /// Class EmailServerDetails.
    /// </summary>
    class EmailServerDetails
    {
        #region Events

        public static event NotifyFromAddress OnFromAddressChanged;

        #endregion Events

        #region Methods

        public static int EmailMaxAttachmentSize()
        {
            //Get Attachment Max Size
            int EmailAttachmentMaxSize = 0;
            var sizeString = (ConfigContainer.Instance().AllKeys.Contains("email.max-attachments-size") ? (string)ConfigContainer.Instance().GetValue("email.max-attachments-size") : string.Empty);
            var amSize = 0;
            if (int.TryParse(sizeString, out amSize))
            {
                EmailAttachmentMaxSize = amSize;
            }
            return EmailAttachmentMaxSize;
        }

        /// <summary>
        /// Gets from address.
        /// </summary>
        /// <returns>List<System.String></returns>
        public static void GetFromAddress()
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            try
            {
                // Get Email Server Application Name
                string emailServerAddress = (ConfigContainer.Instance().AllKeys.Contains("email.from-address") ? (string)ConfigContainer.Instance().GetValue("email.from-address") : string.Empty);
                if (string.IsNullOrEmpty(emailServerAddress) || emailServerAddress.ToLower().Trim().Equals("$emailserver$"))
                    getEmailIDsFromConnectionTab(EmailDataContext.GetInstance().ApplicationName);
                else
                    getEmailIDsFromBussinessAttribute(ConfigContainer.Instance().GetAsString("email.from-address"));

                logger.Info(emailServerAddress);

            }
            catch (Exception ex)
            {
                logger.Error("Error Occurred while reading : GetFromAddress() " + ex.ToString());
            }
        }

        private static void DataCallBack(IAsyncResult iasyncResult)
        {
            FromAddresses comboBoxItem;
            if (iasyncResult != null && iasyncResult.AsyncState == null)
            {
                var icollection = EmailDataContext.GetInstance().ConfigurationServer.EndRetrieveMultipleObjects<CfgEnumeratorValue>(iasyncResult);
                foreach (CfgEnumeratorValue enumerator in icollection)
                {
                    if (enumerator.State == CfgObjectState.CFGEnabled)
                    {
                        comboBoxItem = new FromAddresses();
                        comboBoxItem.Content = enumerator.DisplayName;
                        comboBoxItem.Tag = enumerator.Name;
                        comboBoxItem.ToolTip = enumerator.Description;
                        if (enumerator.IsDefault == CfgFlag.CFGTrue)
                            comboBoxItem.IsSelected = true;
                        else
                            comboBoxItem.IsSelected = false;
                        EmailDataContext.GetInstance().MailBox.Add(comboBoxItem);
                    }
                }
                EmailDataContext.GetInstance().isFromAddressPopulated = true;
                if (OnFromAddressChanged != null)
                    OnFromAddressChanged.Invoke();
            }
        }

        /// <summary>
        /// Gets the email ids from bussiness attribute.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        private static void getEmailIDsFromBussinessAttribute(string applicationName)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            EmailDataContext.GetInstance().MailBox = new List<FromAddresses>();
            try
            {
                CfgEnumeratorQuery businessAttributeQuery = new CfgEnumeratorQuery();
                businessAttributeQuery.Name = applicationName;
                businessAttributeQuery.TenantDbid = ConfigContainer.Instance().TenantDbId;

                var businessAttribute = EmailDataContext.GetInstance().ConfigurationServer.RetrieveObject<CfgEnumerator>(businessAttributeQuery);
                if (businessAttribute != null)
                {
                    CfgEnumeratorValueQuery attributeValuesQuery = new CfgEnumeratorValueQuery();
                    attributeValuesQuery.EnumeratorDbid = businessAttribute.DBID;
                    EmailDataContext.GetInstance().ConfigurationServer.BeginRetrieveMultipleObjects(attributeValuesQuery, DataCallBack, null);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while reading : getEmailServerFromAIDConnectionTab() " + applicationName + "=" + generalException.ToString());
            }
        }

        /// <summary>
        /// Gets the email server from aid connection tab.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>System.String.</returns>
        private static void getEmailIDsFromConnectionTab(string applicationName)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            EmailDataContext.GetInstance().MailBox = new List<FromAddresses>();
            FromAddresses comboBoxItem;
            try
            {
                CfgApplication application = ReadApplicationLevelServerDetails(applicationName);
                if (application != null && application.AppServers != null && application.AppServers.Count != 0)
                {
                    //Get Interaction Server Name from AID connections Tab.
                    foreach (CfgConnInfo appDetails in application.AppServers)
                    {
                        if (appDetails.AppServer.Type == CfgAppType.CFGInteractionServer)
                        {
                            CfgApplication ixnApp = ReadApplicationLevelServerDetails(appDetails.AppServer.Name.ToString());
                            if (ixnApp != null && ixnApp.AppServers != null && ixnApp.AppServers.Count != 0)
                            {
                                //Get Email Server Name from Interaction connections Tab.
                                foreach (CfgConnInfo ixnappDetail in ixnApp.AppServers)
                                {
                                    if (ixnappDetail.AppServer.Type == CfgAppType.CFGEmailServer)
                                    {
                                        //Read application keys and values
                                        CfgApplication applicationObject = ReadApplicationLevelServerDetails(ixnappDetail.AppServer.Name.ToString().Trim());
                                        if (applicationObject != null && applicationObject.Options.Keys.Count > 0)
                                        {
                                            // Get sections from Email Server
                                            foreach (string section in applicationObject.Options.AllKeys)
                                            {
                                                if (section.Contains("pop-client"))
                                                {
                                                    comboBoxItem = new FromAddresses();
                                                    KeyValueCollection kvpPopClient = (KeyValueCollection)applicationObject.Options[section];
                                                    if (kvpPopClient != null && kvpPopClient.Keys.Count > 0 && kvpPopClient.AllKeys.Contains("address") &&
                                                        !string.IsNullOrEmpty(kvpPopClient["address"].ToString()))
                                                    {
                                                        comboBoxItem.Tag = kvpPopClient["address"].ToString();
                                                        comboBoxItem.Content = kvpPopClient["address"].ToString();
                                                        comboBoxItem.ToolTip = kvpPopClient["address"].ToString();
                                                        comboBoxItem.IsSelected = false;
                                                        var getCount = EmailDataContext.GetInstance().MailBox.Where(x => x.Tag.ToString() == kvpPopClient["address"].ToString());
                                                        if (getCount.Count() <= 0)
                                                            EmailDataContext.GetInstance().MailBox.Add(comboBoxItem);
                                                    }
                                                }
                                            }
                                        }
                                        EmailDataContext.GetInstance().isFromAddressPopulated = true;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while reading : getEmailServerFromAIDConnectionTab() " + applicationName + "=" + generalException.ToString());
            }
        }

        /// <summary>
        /// Reads the application level server details.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>CfgApplication.</returns>
        private static CfgApplication ReadApplicationLevelServerDetails(string applicationName)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            CfgApplication application = null;
            try
            {
                application = new CfgApplication(EmailDataContext.GetInstance().ConfigurationServer);
                CfgApplicationQuery queryApp = new CfgApplicationQuery();
                queryApp.Name = applicationName;
                application = EmailDataContext.GetInstance().ConfigurationServer.RetrieveObject<CfgApplication>(queryApp);
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while reading : ReadApplicationLevelServerDetails() " + applicationName + "=" + generalException.ToString());
            }
            return application;
        }

        #endregion Methods
    }
}