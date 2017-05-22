namespace Pointel.Integration.Core.Agent_Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Pointel.Common.DataBase;
    using Pointel.Configuration.Manager;
    using Pointel.Integration.Core.Util;

    class AgentActivities
    {
        #region Fields
        static string connectionString = string.Empty;
        Database db;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        public string loginDBID = string.Empty;
        public string notreadyDBID = string.Empty;
        Settings settings = Settings.GetInstance();
        #endregion Fields

        #region Methods

        public AgentActivities()
        {
            connectionString = "Data Source=" + GetValue("DataSource") + ";Initial Catalog=" + GetValue("Catalog") + ";User Id=" +
                                     GetValue("UserID") + ";Password=" + GetValue("Password");
        }


        private string GetValue(string KeyName)
        {
            var value = string.Empty;
            value = ConfigContainer.Instance().AllKeys.Contains(KeyName) ? ConfigContainer.Instance().GetValue(KeyName) : string.Empty;
            return value;
        }

        /// <summary>
        /// Inserts the agent login activity.
        /// </summary>
        public void InsertAgentLoginActivity()
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("tblAgentLogin")
                    && !string.IsNullOrEmpty(ConfigContainer.Instance().GetValue("tblAgentLogin")) && !string.IsNullOrEmpty(settings.AgentLoginID))
                {
                    logger.Info("InsertAgentLoginActivity : Entry");
                    string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    string query = "Insert into " + ConfigContainer.Instance().GetValue("tblAgentLogin") +
                        "(Date,AgentLoginID,TimeStamp) values('" + DateTime.Now.Date.ToString() + "','" + settings.AgentLoginID + "','" + time + "')";
                    db = new Database();
                    db.Provider = "System.Data.SqlClient";
                    db.ConnectionString = connectionString;
                    db.CreateConnection(true);
                    logger.Info("InsertAgentLoginActivity: Connection Created");
                    db.ExecuteNonQuery(query);
                    logger.Info("InsertAgentLoginActivity: Query Executed : " + query);
                    db.CloseConnection();
                    logger.Info("InsertAgentLoginActivity: Connection Closed");
                    GetLoginDBID(ConfigContainer.Instance().GetValue("tblAgentLogin"), DateTime.Now.Date.ToString(), settings.AgentLoginID, time);
                    logger.Info("InsertAgentLoginActivity : Exit");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Exception During Inserting AgentLoginActivity: " + ex.Message);
            }
        }

        #region GetLoginDBID
        /// <summary>
        /// Gets the login DBID (An Identity Key).
        /// </summary>
        /// <param name="tblname">The tblname.</param>
        /// <param name="date">The date.</param>
        /// <param name="AgentID">The agent ID.</param>
        /// <param name="time">The time.</param>
        private void GetLoginDBID(string tblname, string date, string AgentID, string time)
        {
            try
            {
                logger.Info("GetLoginDBID : Entry");
                string query = "Select DBID from " + tblname + " where Date='" + date + "' and AgentLoginID='" + AgentID + "' and TimeStamp='" + time + "'";
                db = new Database();
                db.Provider = "System.Data.SqlClient";
                db.ConnectionString = connectionString;
                db.CreateConnection(true);
                logger.Info("GetLoginDBID: Connection Created");
                loginDBID = db.ExecuteScalar(2, query).ToString();
                logger.Info("GetLoginDBID: Query Executed : " + query);
                db.CloseConnection();
                logger.Info("GetLoginDBID: Connection Closed");
                logger.Info("GetLoginDBID : Exit");
            }
            catch (Exception ex)
            {
                loginDBID = string.Empty;
                logger.Error("Excpetion During GettingLoingDBID : " + ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Inserts the agent logout activity in the Database.
        /// </summary>
        public void InsertAgentLogoutActivity()
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("tblAgentLogout")
                    && !string.IsNullOrEmpty(ConfigContainer.Instance().GetValue("tblAgentLogout")) && !string.IsNullOrEmpty(loginDBID))
                {
                    logger.Info("InsertAgentLogoutActivity : Entry");
                    string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    string query = "Insert into " + ConfigContainer.Instance().GetValue("tblAgentLogout") +
                        "(Date,LoginDBID,TimeStamp) values('" + DateTime.Now.Date.ToString() + "'," + loginDBID + ",'" + time + "')";
                    db = new Database();
                    db.Provider = "System.Data.SqlClient";
                    db.ConnectionString = connectionString;
                    db.CreateConnection(true);
                    logger.Info("InsertAgentLogoutActivity: Connection Created");
                    db.ExecuteNonQuery(query);
                    logger.Info("InsertAgentLogoutActivity: Query Executed : " + query);
                    db.CloseConnection();
                    logger.Info("InsertAgentLogoutActivity: Connection Closed");
                    loginDBID = string.Empty;
                    notreadyDBID = string.Empty;
                    logger.Info("InsertAgentLogoutActivity : Exit");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception Occured during InsertAgentLogoutActivity: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserts the agent not ready activity in the Database.
        /// </summary>
        public void InsertAgentNotReadyActivity(string reason)
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("tblAgentNotReady")
                    && !string.IsNullOrEmpty(ConfigContainer.Instance().GetValue("tblAgentNotReady")) && !string.IsNullOrEmpty(loginDBID))
                {
                    string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    string query = "Insert into " + ConfigContainer.Instance().GetValue("tblAgentNotReady") +
                                    "(Date,LoginDBID,TimeStamp,Reason) values('" + DateTime.Now.Date.ToString() +
                                     "'," + loginDBID + ",'" + time + "','" + reason + "')";
                    db = new Database();
                    db.Provider = "System.Data.SqlClient";
                    db.ConnectionString = connectionString;
                    db.CreateConnection(true);
                    logger.Info("InsertAgentNotReadyActivity: Connection Created");
                    db.ExecuteNonQuery(query);
                    logger.Info("InsertAgentNotReadyActivity: Query Executed : " + query);
                    db.CloseConnection();
                    logger.Info("InsertAgentNotReadyActivity: Connection Closed");
                    GetNotReadyDBID(ConfigContainer.Instance().GetValue("tblAgentNotReady"), DateTime.Now.Date.ToString(), time);
                    logger.Info("InsertAgentNotReadyActivity : Exit");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception Occured during InsertAgentNotReadyActivity : " + ex.Message);

            }
        }

        /// <summary>
        /// Gets the not ready DBID.
        /// </summary>
        /// <param name="tablename">The tablename.</param>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        private void GetNotReadyDBID(string tablename, string date, string time)
        {
            try
            {
                if (!string.IsNullOrEmpty(loginDBID))
                {
                    string query = "Select DBID from " + tablename + " where Date='" + date + "' and LoginDBID=" + loginDBID + " and TimeStamp='" + time + "'";
                    db = new Database();
                    db.Provider = "System.Data.SqlClient";
                    db.ConnectionString = connectionString;
                    db.CreateConnection(true);
                    logger.Info("GetNotReadyDBID: Connection Created");
                    notreadyDBID = db.ExecuteScalar(2, query).ToString();
                    logger.Info("GetNotReadyDBID: Query Executed : " + query);
                    db.CloseConnection();
                    logger.Info("GetNotReadyDBID: Connection Closed");
                    logger.Info("GetNotReadyDBID : Exit");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception Occured during GetNotReadyDBID : " + ex.Message);
            }
        }
        /// <summary>
        /// Updates the agent ready activity in the Database.
        /// </summary>
        public void UpdateAgentReadyActivity()
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("tblAgentNotReady")
                   && !string.IsNullOrEmpty(ConfigContainer.Instance().GetValue("tblAgentNotReady")) && !string.IsNullOrEmpty(notreadyDBID))
                {
                    string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    string query = "Update " + ConfigContainer.Instance().GetValue("tblAgentNotReady") + " set EndTime='" + time + "' where DBID=" + notreadyDBID;
                    db = new Database();
                    db.Provider = "System.Data.SqlClient";
                    db.ConnectionString = connectionString;
                    db.CreateConnection(true);
                    logger.Info("UpdateAgentReadyActivity: Connection Created");
                    db.ExecuteNonQuery(query);
                    logger.Info("UpdateAgentReadyActivity: Query Executed : " + query);
                    db.CloseConnection();
                    logger.Info("UpdateAgentReadyActivity: Connection Closed");
                    logger.Info("UpdateAgentReadyActivity : Exit");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception Occured during UpdateAgentReadyActivity : " + ex.Message);
            }
        }


        #endregion Methods
    }
}