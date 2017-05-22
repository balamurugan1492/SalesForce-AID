#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Pointel Namespace
using Pointel.Logger.Core;
using Pointel.Statistics.Core.Utility;
using Pointel.Statistics.Core.ConnectionManager;
#endregion

#region Genesys Namespace
using Genesyslab.Platform.Reporting.Protocols;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Routing.Protocols.CustomServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
#endregion

namespace Pointel.Statistics.Core.ConnectionManager
{
    /// <summary>
    /// This class contains to connection establishment, handling server events and subscribe etc.
    /// </summary>
    internal class ReportingServer
    {
        #region Field Declarations
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsSetting Settings = StatisticsSetting.GetInstance();
        public static List<IStatTicker> ToClient = new List<IStatTicker>();       
        #endregion

        #region Subscriber

        /// <summary>
        /// Subscribes the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void Subscribe(IStatTicker listener)
        {
            ToClient.Add(listener);
        }

        #endregion

        //#region Create StatserverConnection

        ///// <summary>
        ///// Creates the connection.
        ///// </summary>
        ///// <param name="host">The host.</param>
        ///// <param name="port">The port.</param>
        ///// <param name="backupHost">The backup host.</param>
        ///// <param name="backupPort">The backup port.</param>
        ///// <param name="addpServerTimeOut">The addp server time out.</param>
        ///// <param name="addpClientTimeOut">The addp client time out.</param>
        ///// <param name="agentIdentifier">The agent identifier.</param>
        ///// <param name="timeOut">The time out.</param>
        ///// <param name="attempts">The attempts.</param>
        ///// <returns></returns>
        //public StatServerProtocol CreateConnection(string host, string port, string backupHost,
        //    string backupPort, int addpServerTimeOut, int addpClientTimeOut, string agentIdentifier,
        //    int timeOut, short attempts)
        //{
        //    StatServerProtocol protocol = null;
        //    try
        //    {
        //        logger.Debug("ReportingServer : CreateConnection Method: Entry");
        //        logger.Info("Reporting Server AddpServerTimeOut : " + addpServerTimeOut);
        //        logger.Info("Reporting Server AddpClientTimeOut : " + addpClientTimeOut);
        //        Settings.statisticsProperties.Uri = new Uri("tcp://" + host + ":" + port);
        //        Settings.statisticsProperties.ClientName = "StatTicker_" + agentIdentifier;

        //        Settings.statisticsProperties.FaultTolerance = FaultToleranceMode.WarmStandby;
        //        Settings.statisticsProperties.WarmStandbyTimeout = timeOut;
        //        Settings.statisticsProperties.WarmStandbyAttempts = attempts;
        //        Settings.statisticsProperties.WarmStandbyUri = new Uri("tcp://" + backupHost +
        //            ":" + backupPort);

        //        Settings.statisticsProperties.UseAddp = true;
        //        Settings.statisticsProperties.AddpServerTimeout = addpServerTimeOut;
        //        Settings.statisticsProperties.AddpClientTimeout = addpClientTimeOut;
        //        Settings.statisticsProperties.AddpTrace = "both";


        //        Settings.rptProtocolManager.Register(Settings.statisticsProperties);

        //        protocol = (StatServerProtocol)Settings.rptProtocolManager[StatisticsSetting.StatServer];

        //        protocol.Received += StatisticsBase.GetInstance().ReportingSuccessMessage;

        //        //Settings.reportingBroker = new EventBrokerService(Settings.rptProtocolManager.Receiver);

        //        //Settings.reportingBroker.Activate();

        //        //Settings.reportingBroker.Register(StatisticsBase.GetInstance().ReportingSuccessMessage);

        //        Settings.rptProtocolManager[StatisticsSetting.StatServer].Open();


        //        if (Settings.rptProtocolManager[StatisticsSetting.StatServer].State == ChannelState.Opened)
        //        {
        //            logger.Trace("ReportingServer : CreateConnection Method : StatServer Connection Opened : " + Settings.statisticsProperties.ClientName);
        //        }
        //        else
        //        {
        //            logger.Warn("ReportingServer : CreateConnection Method: StatServer Connection Closed");
        //        }
        //    }
        //    catch (ProtocolException protocolException)
        //    {
        //        if (protocolException.Message == "Exception occured during channel opening")
        //        {
        //            logger.Error("ReportingServer : CreateConnection Method: " + protocolException.Message);
        //        }
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("ReportingServer : CreateConnection Method: " + generalException.Message);
        //    }
        //    finally
        //    {
        //        logger.Debug("ReportingServer : CreateConnection Method: Exit");
        //        Settings.statisticsProperties = null;
        //        GC.Collect();
        //    }
        //    return protocol;
        //}

        //public void CloseStatServer()
        //{
        //    try
        //    {
        //        if (Settings.rptProtocolManager[StatisticsSetting.StatServer].State == ChannelState.Opened)
        //        {
        //            logger.Info("ReportingServer : Stat server protocol state : " + Settings.rptProtocolManager[StatisticsSetting.StatServer].State.ToString());
        //            Settings.rptProtocolManager[StatisticsSetting.StatServer].Close();
        //            logger.Info("ReportingServer : Stat server protocol has been closed.");
        //        }
        //        else
        //            logger.Info("ReportingServer : Stat server protocol is in " + Settings.rptProtocolManager[StatisticsSetting.StatServer].State + " state.");
        //    }

        //    catch (Exception ex)
        //    {
        //        logger.Error("Error occurred while closing the " + "Stat server protocol : " + ex.Message);
        //    }
        //}

        //#endregion

        #region Create StatserverConnection

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="backupHost">The backup host.</param>
        /// <param name="backupPort">The backup port.</param>
        /// <param name="addpServerTimeOut">The addp server time out.</param>
        /// <param name="addpClientTimeOut">The addp client time out.</param>
        /// <param name="agentIdentifier">The agent identifier.</param>
        /// <param name="timeOut">The time out.</param>
        /// <param name="attempts">The attempts.</param>
        /// <returns></returns>
        public void CreateConnection(Uri primaryUri, Uri backupUri, int addpServerTimeOut, int addpClientTimeOut, string agentIdentifier, int timeout, short attempts, string protocolName)
        {
            if (Settings.statProtocols== null)
            {
                Settings.statProtocols = new List<StatServerProtocol>();
            }
            try
            {
                logger.Debug("ReportingServer : CreateConnection Method: Entry");
                logger.Info("Reporting Server : CreateConnection Method AddpServerTimeOut : " + addpServerTimeOut);
                logger.Info("Reporting Server : CreateConnection Method AddpClientTimeOut : " + addpClientTimeOut);
                logger.Info("Reporting Server : CreateConnection Method primaryUri : " + primaryUri);
                logger.Info("Reporting Server : CreateConnection Method StatProtocol Name : " + protocolName);
                Settings.statisticsProperties = new StatServerConfiguration(protocolName);
                Settings.statisticsProperties.Uri = primaryUri;
                Settings.statisticsProperties.ClientName = "StatTicker_" + agentIdentifier;

                Settings.statisticsProperties.FaultTolerance = FaultToleranceMode.WarmStandby;
                Settings.statisticsProperties.WarmStandbyTimeout = timeout;
                Settings.statisticsProperties.WarmStandbyAttempts = attempts;
                Settings.statisticsProperties.WarmStandbyUri = backupUri;
                Settings.statisticsProperties.UseAddp = true;
                Settings.statisticsProperties.AddpServerTimeout = addpServerTimeOut;
                Settings.statisticsProperties.AddpClientTimeout = addpClientTimeOut;
                Settings.statisticsProperties.AddpTrace = "both";


                Settings.rptProtocolManager.Register(Settings.statisticsProperties);
                if (Settings.statProtocols != null)
                {
                    Settings.statProtocols.Add((StatServerProtocol)Settings.rptProtocolManager[protocolName]);  
               
                }
                Settings.rptProtocolManager[protocolName].Received += StatisticsBase.GetInstance().ReportingSuccessMessage;
                
                //Settings.reportingBroker = new EventBrokerService(Settings.rptProtocolManager.Receiver);

                //Settings.reportingBroker.Activate();

                //Settings.reportingBroker.Register(StatisticsBase.GetInstance().ReportingSuccessMessage);

                Settings.rptProtocolManager[protocolName].Open();


                if (Settings.rptProtocolManager[protocolName].State == ChannelState.Opened)
                {
                    if (StatisticsSetting.GetInstance().serverNames == null)
                    {
                        StatisticsSetting.GetInstance().serverNames = new Dictionary<string, bool>();
                    }
                    if (!StatisticsSetting.GetInstance().serverNames.ContainsKey(protocolName))
                        StatisticsSetting.GetInstance().serverNames.Add(protocolName,true);
                    logger.Trace("ReportingServer : CreateConnection Method : StatServer Connection Opened" );
                    logger.Info("Reporting Server : CreateConnection Method Stat Server Protocol : " + protocolName + "opened");
                }
                else
                {
                    logger.Warn("ReportingServer : CreateConnection Method: StatServer Connection Closed");
                    logger.Warn("Reporting Server : CreateConnection Method Stat Server Protocol : " + protocolName + "closed");
                }
            }
            catch (ProtocolException protocolException)
            {
                if (protocolException.Message == "Exception occured during channel opening")
                {
                    logger.Error("ReportingServer : CreateConnection Method: " + protocolException.Message);
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ReportingServer : CreateConnection Method: " + generalException.Message);
            }
            finally
            {
                logger.Debug("ReportingServer : CreateConnection Method: Exit");
                Settings.statisticsProperties = null;
                GC.Collect();
            }            
        }

        public void CloseStatServer()
        {
            try
            {
                logger.Info("ReportingServer : CloseStatServer Method - Entry");  
                if((StatisticsSetting.GetInstance().serverNames!=null)&&(StatisticsSetting.GetInstance().serverNames.Count>0))
                {
                    foreach (string name in StatisticsSetting.GetInstance().serverNames.Keys)
                    {
                        if (Settings.rptProtocolManager[name]!=null)
                        {
                            if (Settings.rptProtocolManager[name].State == ChannelState.Opened)
                            {
                                logger.Info("ReportingServer : CloseStatServer Method Stat server protocol state : " + Settings.rptProtocolManager[name].State.ToString());
                                Settings.rptProtocolManager[name].Close();
                                logger.Info("ReportingServer : CloseStatServer Method Stat server protocol has been closed.");
                            }
                            else
                                logger.Info("ReportingServer : CloseStatServer Method Stat server protocol is in " + Settings.rptProtocolManager[name].State + " state.");
                        }
                        
                    }                   
                    
                }
                Settings.rptProtocolManager = null;
                StatisticsSetting.GetInstance().serverNames = null;

               
            }

            catch (Exception ex)
            {
                logger.Error("Error occurred while closing the " + "Stat server protocol : " + ex.Message);
            }
            logger.Info("ReportingServer : CloseStatServer Method - Exit");
        }

        #endregion
    }
}
