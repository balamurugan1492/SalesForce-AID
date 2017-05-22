#region System Namespaces

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Linq;
using System.Runtime.InteropServices;

#endregion System Namespaces

#region Log4Net Namespace



#endregion Log4Net Namespace

#region Genesys Namespaces

using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;

#endregion Genesys Namespaces

#region Pointel Namespaces

using Pointel.Integration.Core.iSubjects;
using Pointel.Integration.Core.Providers;
using Pointel.Integration.Core.Util;

#endregion Pointel Namespaces

namespace Pointel.Integration.Core.Observers
{
    /// <summary>
    /// This class used to Subscribe the CRM DB Integration
    /// </summary>
    internal class CrmDbSubscriber : IObserver<iCallData>
    {
        #region Field Declaration

        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private Settings setting = Settings.GetInstance();
        private string result = string.Empty;
        private string query = string.Empty;

        #endregion Field Declaration

        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>

        #region Subscribe

        public virtual void Subscribe(CallDataProviders provider)
        {
            try
            {
                cancellation = provider.Subscribe(this);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Subscribe CRM DB Integration " + generalException.ToString());
            }
        }

        #endregion Subscribe

        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>

        #region Unsubscribe

        public virtual void Unsubscribe()
        {
            //try
            //{
            //    cancellation.Dispose();
            //}
            //catch (Exception generalException)
            //{
            //    logger.Error("Error occurred while UnSubscribe CRM DB Integration " + generalException.ToString());
            //}
        }

        #endregion Unsubscribe

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>

        #region OnCompleted

        public void OnCompleted()
        {
            try
            {
                Unsubscribe();
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while On Completed " + generalException.ToString());
            }
        }

        #endregion OnCompleted

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>

        #region OnError

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        #endregion OnError

        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>

        #region OnNext

        public void OnNext(iCallData value)
        {
            string result = string.Empty;
            try
            {
                if (value != null && value.EventMessage!=null)
                {
                    switch (value.EventMessage.Id)
                    {                         
                        case EventRinging.MessageId:
                            EventRinging NewCustomerDataRinging = (EventRinging)value.EventMessage;
                            logger.Trace(NewCustomerDataRinging.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(NewCustomerDataRinging.Name))
                                {
                                    SaveDataDB(NewCustomerDataRinging.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                                //default Ringing
                                else if (value.CrmDbData.CallDataEventDBType == null && value.CrmDbData.CallDataEventDBType.Length == 0)
                                {
                                    SaveDataDB(NewCustomerDataRinging.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventReleased.MessageId:
                            EventReleased eventReleased = (EventReleased)value.EventMessage;
                            logger.Trace(eventReleased.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventReleased.Name))
                                {
                                    SaveDataDB(eventReleased.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventEstablished.MessageId:
                            EventEstablished eventEstablished = (EventEstablished)value.EventMessage;
                            logger.Trace(eventEstablished.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventEstablished.Name))
                                {
                                    SaveDataDB(eventEstablished.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventHeld.MessageId:
                            EventHeld eventHeld = (EventHeld)value.EventMessage;
                            logger.Trace(eventHeld.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventHeld.Name))
                                {
                                    SaveDataDB(eventHeld.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventPartyChanged.MessageId:
                            EventPartyChanged eventPartyChanged = (EventPartyChanged)value.EventMessage;
                            logger.Trace(eventPartyChanged.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventPartyChanged.Name))
                                {
                                    SaveDataDB(eventPartyChanged.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventAttachedDataChanged.MessageId:
                            EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)value.EventMessage;
                            logger.Trace(eventAttachedDataChanged.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventAttachedDataChanged.Name))
                                {
                                    SaveDataDB(eventAttachedDataChanged.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventDialing.MessageId:
                            EventDialing eventDialing = (EventDialing)value.EventMessage;
                            logger.Trace(eventDialing.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventDialing.Name))
                                {
                                    SaveDataDB(eventDialing.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventRetrieved.MessageId:
                            EventRetrieved eventRetrieved = (EventRetrieved)value.EventMessage;
                            logger.Trace(eventRetrieved.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventRetrieved.Name))
                                {
                                    SaveDataDB(eventRetrieved.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventAbandoned.MessageId:
                            EventAbandoned eventAbandoned = (EventAbandoned)value.EventMessage;
                            logger.Trace(eventAbandoned.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventAbandoned.Name))
                                {
                                    SaveDataDB(eventAbandoned.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventPartyAdded.MessageId:
                            EventAbandoned eventPartyAdded = (EventAbandoned)value.EventMessage;
                            logger.Trace(eventPartyAdded.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventPartyAdded.Name))
                                {
                                    SaveDataDB(eventPartyAdded.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;

                        case EventPartyDeleted.MessageId:
                            EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)value.EventMessage;
                            logger.Trace(eventPartyDeleted.ToString());
                            if (setting.EnableCrmDbCommunication)
                            {
                                if (value.CrmDbData.CallDataEventDBType.Contains(eventPartyDeleted.Name))
                                {
                                    SaveDataDB(eventPartyDeleted.UserData, value, parameter: value.CrmDbData.ParameterName);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while writing call data to a file " + generalException.ToString());
            }
        }

        #endregion OnNext

        /// <summary>
        /// Saves the data database.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="value">The value.</param>
        /// <summary>
        /// Saves the data database.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="value">The value.</param>
        /// <param name="parameter">The parameter.</param>

        #region SaveDataDB

        private void SaveDataDB(KeyValueCollection userData, iCallData value, [Optional] Dictionary<string, string> parameter)
        {
            try
            {
                if (value.CrmDbData.CrmDbFormat.Equals("SqlLite"))
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> keys in parameter)
                        {
                            if (userData != null)
                            {
                                if (userData.ContainsKey(keys.Value))
                                {
                                    result += keys.Key + "=" + Convert.ToString(userData[keys.Value]) + value.CrmDbData.Delimiter;
                                }
                            }
                        }
                        AttributeFilter(value, userData);
                        string sqlStatement = "INSERT INTO CustomerDetails (UserData,AgentId) VALUES(@UserData,@AgentId)";
                        string dbConnectionString = @"Data Source=" + value.CrmDbData.DirectoryPath.ToString();
                        logger.Info("Data Source : " + dbConnectionString);
                        SQLiteConnection sqliteCon = new SQLiteConnection(dbConnectionString);
                        sqliteCon.Open();
                        SQLiteCommand command = new SQLiteCommand(sqlStatement, sqliteCon);
                        command.Parameters.Add("@UserData", DbType.String).Value = result;
                        command.Parameters.Add("@AgentId", DbType.String).Value = Settings.AgentId;
                        command.ExecuteNonQuery();
                        sqliteCon.Close();
                    }
                    catch (Exception exception)
                    {
                        logger.Error("Error occurred in" + exception.ToString());
                    }
                }
                else if (value.CrmDbData.CrmDbFormat.Equals("SqlServer"))
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> keys in parameter)
                        {
                            if (userData != null)
                            {
                                if (userData.ContainsKey(keys.Value))
                                {
                                    result += keys.Key + "=" + Convert.ToString(userData[keys.Value]) + value.CrmDbData.Delimiter;
                                }
                            }
                        }
                        AttributeFilter(value, userData);
                        // define INSERT query with parameters
                        query = "INSERT INTO CustomerDetails (UserData,AgentId) VALUES (@UserData,@AgentId) ";
                        // create connection and command
                        using (SqlCommand cmd = new SqlCommand(query, Settings.GetInstance().cn))
                        {
                            // define parameters and their values
                            cmd.Parameters.Add("@UserData", SqlDbType.VarChar, 300).Value = result;
                            cmd.Parameters.Add("@AgentId", SqlDbType.VarChar, 100).Value = Settings.AgentId;
                            // open connection, execute INSERT, close connection
                            //cn.Open();
                            cmd.ExecuteNonQuery();
                            Settings.GetInstance().cn.Close();
                        }
                    }
                    catch (Exception exception)
                    {
                        logger.Error("Error occurred in" + exception.ToString());
                    }
                }
                else if (value.CrmDbData.CrmDbFormat.Equals("Oracle"))
                {
                    foreach (KeyValuePair<string, string> keys in parameter)
                    {
                        if (userData != null)
                        {
                            if (userData.ContainsKey(keys.Value))
                            {
                                result += keys.Key + "=" + Convert.ToString(userData[keys.Value]) + value.CrmDbData.Delimiter;
                            }
                        }
                    }
                    AttributeFilter(value, userData);
                    // define INSERT query with parameters
                    string queryString = "INSERT INTO customerdetails  values ('" + Settings.AgentId + "','" + result + "')";
                    OracleCommand command = new OracleCommand(queryString);
                    command.Connection = Settings.GetInstance().connection;
                    try
                    {
                        //connection.Open();
                        command.ExecuteNonQuery();
                        Settings.GetInstance().connection.Close();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error occurred while writing call data to a file " + ex.ToString());
                    }
                }
                result = string.Empty;
            }
            catch (Exception exception)
            {
                logger.Error("SendTextData:" + exception.ToString());
            }
        }

        #endregion SaveDataDB

        /// <summary>
        /// Attributes the filter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>

        #region AttributeFilter

        private string AttributeFilter(iCallData value, KeyValueCollection userData)
        {
            //foreach (string key in value.CrmDbData.AttributeFilter)
            //{
            foreach (KeyValuePair<string, string> keys in value.CrmDbData.ParameterValue)
            {
                //if (key != "")
                //    if (userData.AllKeys.Contains(key))
                //    {
                //        result += key + "=" + Convert.ToString(userData[key]) + value.CrmDbData.Delimiter;
                //    }
                //    else
                {
                    switch (value.EventMessage.Id)
                    {
                        case EventRinging.MessageId:
                            EventRinging NewCustomerDataRinging = (EventRinging)value.EventMessage;

                            if (NewCustomerDataRinging.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(NewCustomerDataRinging[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(NewCustomerDataRinging["AgentID"]);
                            break;

                        case EventReleased.MessageId:
                            EventReleased eventReleased = (EventReleased)value.EventMessage;
                            if (eventReleased.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventReleased[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventReleased["AgentID"]);
                            break;

                        case EventEstablished.MessageId:
                            EventEstablished eventEstablished = (EventEstablished)value.EventMessage;
                            if (eventEstablished.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventEstablished[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }

                            Settings.AgentId = Convert.ToString(eventEstablished["AgentID"]);
                            break;

                        case EventAttachedDataChanged.MessageId:
                            EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)value.EventMessage;
                            if (eventAttachedDataChanged.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventAttachedDataChanged[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventAttachedDataChanged["AgentID"]);
                            break;

                        case EventHeld.MessageId:
                            EventHeld eventHeld = (EventHeld)value.EventMessage;
                            if (eventHeld.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventHeld[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventHeld["AgentID"]);
                            break;

                        case EventPartyChanged.MessageId:
                            EventPartyChanged eventPartyChanged = (EventPartyChanged)value.EventMessage;
                            if (eventPartyChanged.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventPartyChanged[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventPartyChanged["AgentID"]);
                            break;

                        case EventDialing.MessageId:
                            EventDialing eventDialing = (EventDialing)value.EventMessage;
                            if (eventDialing.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventDialing[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventDialing["AgentID"]);
                            break;

                        case EventRetrieved.MessageId:
                            EventRetrieved eventRetrieved = (EventRetrieved)value.EventMessage;
                            if (eventRetrieved.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventRetrieved[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventRetrieved["AgentID"]);
                            break;

                        case EventAbandoned.MessageId:
                            EventAbandoned eventAbandoned = (EventAbandoned)value.EventMessage;
                            if (eventAbandoned.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventAbandoned[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventAbandoned["AgentID"]);
                            break;

                        case EventPartyAdded.MessageId:
                            EventAbandoned eventPartyAdded = (EventAbandoned)value.EventMessage;
                            if (eventPartyAdded.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventPartyAdded[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventPartyAdded["AgentID"]);
                            break;

                        case EventPartyDeleted.MessageId:
                            EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)value.EventMessage;
                            if (eventPartyDeleted.Contains(keys.Key))
                            {
                                result += keys.Value + "=" + Convert.ToString(eventPartyDeleted[keys.Key]) + value.CrmDbData.Delimiter;
                            }
                            else
                            {
                                result += keys.Value + "=" + "''" + value.CrmDbData.Delimiter;
                            }
                            Settings.AgentId = Convert.ToString(eventPartyDeleted["AgentID"]);
                            break;
                    }
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        #endregion AttributeFilter
    }
}