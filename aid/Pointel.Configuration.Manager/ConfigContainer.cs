#region Header

/*
* =====================================
* Pointel.Configuration.Manager.Core
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

#endregion Header

namespace Pointel.Configuration.Manager
{
    using System;
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Collections;

    using log4net;

    using Pointel.Configuration.Manager.Core;

    /// <summary>
    /// Configuration container class holds the configuration server data
    /// </summary>
    public class ConfigContainer
    {
        #region Fields

        internal Dictionary<string, dynamic> _cmeValues = new Dictionary<string, dynamic>();

        private static ConfigContainer _instance = null;

        private List<string> _allKeys = new List<string>();
        private int _applicationDbId;
        private ConfService _confService;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
             "AID");
        private int _personDbId;
        private int _tenantDbId;
        private string _userName = string.Empty;

        #endregion Fields

        #region Properties

        public string AgentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets all keys.
        /// </summary>
        /// <value>
        /// All keys.
        /// </value>
        public List<string> AllKeys
        {
            get
            {
                return new List<string>(CMEValues.Keys);
            }
            //get
            //{
            //    return _allKeys;
            //}
            //set
            //{
            //    if (_allKeys != value)
            //    {
            //        _allKeys = value;
            //    }
            //}
        }

        /// <summary>
        /// Gets or sets the application database unique identifier.
        /// </summary>
        /// <value>
        /// The application database unique identifier.
        /// </value>
        public int ApplicationDbId
        {
            get
            {
                return _applicationDbId;
            }
            set
            {
                if (_applicationDbId != value)
                {
                    _applicationDbId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the configuration service object.
        /// </summary>
        /// <value>
        /// The configuration service object.
        /// </value>
        public ConfService ConfServiceObject
        {
            get
            {
                return _confService;
            }
            set
            {
                if (_confService != value)
                {
                    _confService = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the person database unique identifier.
        /// </summary>
        /// <value>
        /// The person database unique identifier.
        /// </value>
        public int PersonDbId
        {
            get
            {
                return _personDbId;
            }
            set
            {
                if (_personDbId != value)
                {
                    _personDbId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tenant database unique identifier.
        /// </summary>
        /// <value>
        /// The tenant database unique identifier.
        /// </value>
        public int TenantDbId
        {
            get
            {
                return _tenantDbId;
            }
            set
            {
                if (_tenantDbId != value)
                {
                    _tenantDbId = value;
                }
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                }
            }
        }

        internal Dictionary<string, dynamic> CMEValues
        {
            get
            {
                return _cmeValues;
            }
            set
            {
                if (_cmeValues != value)
                {
                    _cmeValues = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns></returns>
        public static ConfigContainer Instance()
        {
            if (_instance == null)
            {
                _instance = new ConfigContainer();
                return _instance;
            }
            return _instance;
        }

        /// <summary>
        /// Gets as boolean.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public bool GetAsBoolean(string keyName, bool defaultValue = false)
        {
            if (CMEValues.ContainsKey(keyName) && CMEValues[keyName].GetType() == (typeof(Pointel.Configuration.Manager.Helpers.ConfigValue)))
            {
                bool temp;
                if (!bool.TryParse(((Pointel.Configuration.Manager.Helpers.ConfigValue)CMEValues[keyName]).Value, out temp))
                    goto End;
                return temp;
            }
        End:
            return defaultValue;
        }

        /// <summary>
        /// Gets as integer.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public int GetAsInteger(string keyName)
        {
            if (!CMEValues.ContainsKey(keyName))
                throw new Exception("No key found");

            if (CMEValues[keyName].GetType() == (typeof(Pointel.Configuration.Manager.Helpers.ConfigValue)))
                return Convert.ToInt32(((Pointel.Configuration.Manager.Helpers.ConfigValue)CMEValues[keyName]).Value);
            else
                return CMEValues[keyName];
        }

        /// <summary>
        /// Gets as string.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public string GetAsString(string keyName)
        {
            if (!CMEValues.ContainsKey(keyName))
                return "No key found";

            if (CMEValues[keyName].GetType() == (typeof(Pointel.Configuration.Manager.Helpers.ConfigValue)))
                return ((Pointel.Configuration.Manager.Helpers.ConfigValue)CMEValues[keyName]).Value;
            else
                return CMEValues[keyName];
        }

        /// <summary>
        /// Gets the value of the given key name.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public dynamic GetValue(string keyName)
        {
            if (!CMEValues.ContainsKey(keyName))
                return "No key found";

            if (CMEValues[keyName].GetType() == (typeof(Pointel.Configuration.Manager.Helpers.ConfigValue)))
                return ((Pointel.Configuration.Manager.Helpers.ConfigValue)CMEValues[keyName]).Value;
            else
                return CMEValues[keyName];
        }

        public void NullValue(string keyName)
        {
            //if (this.AllKeys.Contains(keyName))
            //    this.AllKeys.Remove(keyName);
            if (this.CMEValues.ContainsKey(keyName))
                this.CMEValues.Remove(keyName);
        }

        public void ReadSection(string sectionName, bool readOnlyApplication = true)
        {
            ConfigurationHandler objConfigurationHandler = new ConfigurationHandler();
            KeyValueCollection objSection = objConfigurationHandler.GetSection(sectionName, readOnlyApplication);
            if (objSection != null)
            {
                //ConfigContainer.Instance().AllKeys.Add(sectionName);
                ConfigContainer.Instance().CMEValues.Add(sectionName, objSection);
            }
        }

        #endregion Methods

        #region Other

        //public void ReadApplication(string applicationName, string userName)
        //{
        //    try
        //    {
        //        Dictionary<string, object> dicCMEObjects = new Dictionary<string, object>();
        //        ReadApplicationObject readValues = new ReadApplicationObject();
        //        //readValues.ReadApplication(applicationName);
        //        readValues.ReadAgentGroup(userName);
        //        dicCMEObjects = readValues.ReadPerson(userName);
        //        if (UserConnectionHandler.GetInstance().MessageToClient.ContainsKey(userName) && UserConnectionHandler.GetInstance().MessageToClient[userName] != null)
        //        {
        //            UserConnectionHandler.GetInstance().MessageToClient[userName].NotifyCMEObjects(dicCMEObjects);
        //        }
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while reading the application : " +
        //            ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
        //    }
        //}

        #endregion Other
    }
}