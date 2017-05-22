#region System Namespace
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Drawing;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using System.Data.OracleClient;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.ConnectionManager;
using Pointel.Statistics.Core.StatisticsProvider;
using Pointel.Statistics.Core.Provider;
using Pointel.Statistics.Core.StatisticsRequest;
using Pointel.Statistics.Core.Utility;
using Pointel.Logger.Core;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
using Genesyslab.Platform.Commons.Protocols;
#endregion


namespace Pointel.Statistics.Core.Subscriber
{
    internal class DBSubscriber : IObserver<IStatisticsCollection>
    {
        #region Field Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        private IDisposable _cancellation;
        StatisticsSetting statSettings = new StatisticsSetting();
        StatVariables statVariables = new StatVariables();
        //StatisticMetric statMetric;
        DispatcherTimer DBTimer = new DispatcherTimer();
        IStatisticsCollection tempStatCollection;
        StatisticsBase statBase = new StatisticsBase();

        #region SQL Server
        SqlConnection sqlConnection = new SqlConnection();
        //SqlCommand sqlCommand;
        //SqlDataAdapter sqlAdapter;
        #endregion

        #region SQLite
        SQLiteConnection sqliteCon = new SQLiteConnection();
        //SQLiteDataAdapter sqliteDA = null;
        //SQLiteCommand sqliteCmd = null;
        #endregion

        #region ORACLE
        OracleConnection oracleConn = new OracleConnection();
        // OracleCommand oracleCmd = null;
        // OracleDataAdapter oracleDA = null;
        #endregion

        #endregion

        public virtual void Subscribe(StatisticsDataProvider provider)
        {
            _cancellation = provider.Subscribe(this);
            DBTimer.Interval = new TimeSpan(0, 0, 1);
            DBTimer.Tick += new EventHandler(DBTimer_Tick);
        }

        void DBTimer_Tick(object sender, EventArgs e)
        {
            OnNext(tempStatCollection);
        }

        /// <summary>
        /// Unsubscribes this instance.
        /// </summary>
        public virtual void Unsubscribe()
        {
            _cancellation.Dispose();
        }

        /// <summary>
        /// Called when [completed].
        /// </summary>
        public void OnCompleted()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="error">The error.</param>
        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [next].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnNext(IStatisticsCollection value)
        {
            List<string> tempThresholdValues;
            List<Color> tempThresholdColors;
            DataTable dataTable = null;
            try
            {
                foreach (IDBValules dbStat in value.DBValues)
                {
                    DBTimer.Stop();

                    tempStatCollection = value;

                    string connectionString = string.Empty;

                    if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLServer.ToString(), true) == 0)
                    {
                        #region SQL Server Connection

                        if (!StatisticsSetting.GetInstance().isDBConnectionOpened)
                        {
                            logger.Info("DBSubscriber : OnNext :  Establishing connection with the Database");

                            DBConnection();
                        }
                        if (StatisticsSetting.GetInstance().sqlConnection.State == ConnectionState.Open)
                        {
                            logger.Info("DBSubscriber : OnNext :  Connection to the Database established");

                            dataTable = new DataTable();
                            StatisticsSetting.GetInstance().sqlCommand = new SqlCommand(dbStat.Query, StatisticsSetting.GetInstance().sqlConnection);
                            StatisticsSetting.GetInstance().sqlAdapter = new SqlDataAdapter(StatisticsSetting.GetInstance().sqlCommand);
                            StatisticsSetting.GetInstance().sqlAdapter.Fill(dataTable);
                            StatisticsSetting.GetInstance().sqlCommand.Notification = null;

                            //SqlDependency sqlDependency = new SqlDependency(StatisticsSetting.GetInstance().sqlCommand);
                            //sqlDependency.OnChange += new OnChangeEventHandler(sqlDependency_OnChange);
                        }
                        else
                        {
                            logger.Error("DBSubscriber : OnNext :  Connection to the Database lost, Trying to Re-Establish the connection with the Database");

                            DBConnection();
                        }

                        #endregion
                    }
                    else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLite.ToString(), true) == 0)
                    {
                        #region SQLite Connection

                        if (!StatisticsSetting.GetInstance().isDBConnectionOpened)
                        {
                            logger.Info("DBSubscriber : OnNext :  Establishing connection with the Database");

                            DBConnection();
                        }
                        if (StatisticsSetting.GetInstance().sqliteCon.State == ConnectionState.Open)
                        {
                            logger.Info("DBSubscriber : OnNext :  Connection to the Database established");

                            dataTable = new DataTable();
                            StatisticsSetting.GetInstance().sqliteCmd = StatisticsSetting.GetInstance().sqliteCon.CreateCommand();
                            StatisticsSetting.GetInstance().sqliteCmd.CommandText = dbStat.Query;
                            StatisticsSetting.GetInstance().sqliteDA = new SQLiteDataAdapter(StatisticsSetting.GetInstance().sqliteCmd);
                            StatisticsSetting.GetInstance().sqliteDA.Fill(dataTable);
                        }
                        else
                        {
                            logger.Error("DBSubscriber : OnNext :  Connection to the Database lost, Trying to Re-Establish the connection with the Database");

                            DBConnection();
                        }

                        #endregion
                    }
                    else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.ORACLE.ToString(), true) == 0)
                    {
                        #region ORACLE Connection

                        if (!StatisticsSetting.GetInstance().isDBConnectionOpened)
                        {
                            logger.Info("DBSubscriber : OnNext :  Establishing connection with the Database");

                            DBConnection();
                        }
                        if (StatisticsSetting.GetInstance().oracleConn.State == ConnectionState.Open)
                        {
                            logger.Info("DBSubscriber : OnNext :  Connection to the Database established");

                            dataTable = new DataTable();
                            StatisticsSetting.GetInstance().oracleCmd = StatisticsSetting.GetInstance().oracleConn.CreateCommand();
                            StatisticsSetting.GetInstance().oracleCmd.CommandText = dbStat.Query;
                            StatisticsSetting.GetInstance().oracleDA = new OracleDataAdapter(StatisticsSetting.GetInstance().oracleCmd);
                            StatisticsSetting.GetInstance().oracleDA.Fill(dataTable);
                        }
                        else
                        {
                            logger.Error("DBSubscriber : OnNext :  Connection to the Database lost, Trying to Re-Establish the connection with the Database");

                            DBConnection();
                        }

                        #endregion
                    }


                    if (dataTable.Rows.Count != 0)
                    {
                        tempThresholdValues = new List<string>();
                        tempThresholdValues.Add(dbStat.Threshold1);
                        tempThresholdValues.Add(dbStat.Threshold2);

                        tempThresholdColors = new List<Color>();
                        tempThresholdColors.Add(dbStat.Color1);
                        tempThresholdColors.Add(dbStat.Color2);
                        tempThresholdColors.Add(dbStat.Color3);

                        if (!StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(dbStat.ReferenceID.ToString()))
                            StatisticsSetting.GetInstance().ThresholdValues.Add(dbStat.ReferenceID.ToString(), tempThresholdValues);

                        if (!StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(dbStat.ReferenceID.ToString()))
                            StatisticsSetting.GetInstance().ThresholdColors.Add(dbStat.ReferenceID.ToString(), tempThresholdColors);

                        if (!StatisticsSetting.GetInstance().DictAllStats.ContainsKey(dbStat.ReferenceID.ToString()))
                            StatisticsSetting.GetInstance().DictAllStats.Add(dbStat.ReferenceID.ToString(), dbStat.TempStat);

                        if (!StatisticsSetting.GetInstance().DictDBStatValuesHolder.ContainsKey(dbStat.ReferenceID.ToString()))
                            StatisticsSetting.GetInstance().DictDBStatValuesHolder.Add(dbStat.ReferenceID.ToString(), dataTable.Rows[0][0].ToString());
                        else
                            StatisticsSetting.GetInstance().DictDBStatValuesHolder[dbStat.ReferenceID.ToString()] = dataTable.Rows[0][0].ToString();

                    }

                    statBase.NotifyDBStatistics(dbStat.ReferenceID.ToString());
                }

                DBTimer.Start();
            }
            catch (SQLiteException sqliteException)
            {
                logger.Error("DBSubscriber : OnNext : " + (sqliteException.InnerException == null ? sqliteException.Message : sqliteException.InnerException.Message));
                statBase.DisplayDBError(sqliteException.InnerException == null ? sqliteException.Message : sqliteException.InnerException.Message);
                DBConnection();
            }
            catch (SqlException sqlException)
            {
                logger.Error("DBSubscriber : OnNext : " + (sqlException.InnerException == null ? sqlException.Message : sqlException.InnerException.Message));
                statBase.DisplayDBError(sqlException.InnerException == null ? sqlException.Message : sqlException.InnerException.Message);
                DBConnection();
            }
            catch (OracleException oracleException)
            {
                logger.Error("DBSubscriber : OnNext : " + (oracleException.InnerException == null ? oracleException.Message : oracleException.InnerException.Message));
                statBase.DisplayDBError(oracleException.InnerException == null ? oracleException.Message : oracleException.InnerException.Message);
                DBConnection();
            }
            catch (Exception GeneralException)
            {
                logger.Error("DBSubscriber : OnNext : " + (GeneralException.InnerException == null ? GeneralException.Message : GeneralException.InnerException.Message));
            }
            finally
            {
                //sqlConnection.Close();     
            }
        }

        void sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            throw new NotImplementedException();
        }


        #region DBConnection

        /// <summary>
        /// Databases the connection.
        /// </summary>
        public void DBConnection()
        {
            try
            {
                if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLServer.ToString(), true) == 0)
                {
                    logger.Info("DBSubscriber : DBConnection :  Establishing connection with the Database");

                    #region SQL Connection

                    StatisticsSetting.GetInstance().sqlConnection.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource + ";Initial Catalog=" + StatisticsSetting.GetInstance().DBName + ";Persist Security Info=True;User ID=" + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword;
                    StatisticsSetting.GetInstance().sqlConnection.Open();

                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;

                    #endregion
                }
                else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.SQLite.ToString(), true) == 0)
                {
                    logger.Info("DBSubscriber : DBConnection :  Establishing connection with the Database");

                    #region SQLite Connection

                    StatisticsSetting.GetInstance().sqliteCon.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource;
                    StatisticsSetting.GetInstance().sqliteCon.Open();

                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;

                    #endregion
                }
                else if (string.Compare(StatisticsSetting.GetInstance().DBType, StatisticsEnum.DBType.ORACLE.ToString(), true) == 0)
                {
                    logger.Info("DBSubscriber : DBConnection :  Establishing connection with the Database");

                    #region ORACLE Connection

                    if (StatisticsSetting.GetInstance().DBSID == "" || StatisticsSetting.GetInstance().DBSID == null)
                    {
                        StatisticsSetting.GetInstance().DBDataSource = "(Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + StatisticsSetting.GetInstance().DBHost + ")(PORT=" + StatisticsSetting.GetInstance().DBPort + ")))(CONNECT_DATA=(SERVICE_NAME=" + StatisticsSetting.GetInstance().DBSName + ")));User Id = " + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword + ";";
                    }
                    else
                    {
                        StatisticsSetting.GetInstance().DBDataSource = "(Description=(Address_list=(Address=(Protocol=TCP)(HOST=" + StatisticsSetting.GetInstance().DBHost + ")(PORT=" + StatisticsSetting.GetInstance().DBPort + ")))(CONNECT_DATA=(SID=" + StatisticsSetting.GetInstance().DBSID + ")));User Id = " + StatisticsSetting.GetInstance().DBUserName + ";Password=" + StatisticsSetting.GetInstance().DBPassword + ";";
                    }

                    StatisticsSetting.GetInstance().oracleConn.ConnectionString = "Data Source=" + StatisticsSetting.GetInstance().DBDataSource;
                    StatisticsSetting.GetInstance().oracleConn.Open();

                    StatisticsSetting.GetInstance().isDBConnectionOpened = true;

                    #endregion
                }
            }
            catch (SQLiteException sqliteException)
            {
                logger.Error("DBSubscriber : DBConnection : " + (sqliteException.InnerException == null ? sqliteException.Message : sqliteException.InnerException.Message));
                statBase.DisplayDBError(sqliteException.InnerException == null ? sqliteException.Message : sqliteException.InnerException.Message);
                DBTimer.Start();
            }
            catch (SqlException sqlException)
            {
                logger.Error("DBSubscriber : DBConnection : " + (sqlException.InnerException == null ? sqlException.Message : sqlException.InnerException.Message));
                statBase.DisplayDBError(sqlException.InnerException == null ? sqlException.Message : sqlException.InnerException.Message);
                DBTimer.Start();
            }
            catch (OracleException oracleException)
            {
                logger.Error("DBSubscriber : DBConnection : " + (oracleException.InnerException == null ? oracleException.Message : oracleException.InnerException.Message));
                statBase.DisplayDBError(oracleException.InnerException == null ? oracleException.Message : oracleException.InnerException.Message);
                DBTimer.Start();
            }

        }

        #endregion
    }
}
