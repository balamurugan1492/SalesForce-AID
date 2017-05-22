namespace Pointel.Interactions.Chat.Core.ConnectionManager
{
    using System;

    using Pointel.Connection.Manager;

    internal class ChatConnectionManager
    {
        #region Fields

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Closes the chat protocol.
        /// </summary>
        /// <returns></returns>
        public Pointel.Interactions.Chat.Core.General.OutputValues CloseChatProtocol()
        {
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();

            try
            {
                Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Close();
                output.MessageCode = "200";
                output.Message = "Closed Chat Protocol Successful";
            }
            catch (Genesyslab.Platform.Commons.Protocols.ProtocolException protocolException)
            {
                output.MessageCode = "2001";
                output.Message = "Can't Close Chat Protocol";
                logger.Error("Error occurred while Close Chat Protocol " + protocolException.ToString());
            }
            catch (Exception generalException)
            {
                output.MessageCode = "2001";
                output.Message = "Can't Close Chat Protocol";
                logger.Error("Error occurred while Close Chat Protocol " + generalException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Opens the chat protocol.
        /// </summary>
        /// <param name="nickName">Name of the nick.</param>
        /// <param name="personId">The person identifier.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public Pointel.Interactions.Chat.Core.General.OutputValues OpenChatProtocol(string nickName, string personId,
            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgApplication primaryServer,
            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects.CfgApplication secondaryServer, bool switchoverServer = true)
        {
            var output = new Pointel.Interactions.Chat.Core.General.OutputValues();

            var chatServerConfProperties = new BasicChatServerConfProperties();
            try
            {
                if (primaryServer != null)
                {

                    logger.Debug("OpenChatProtocol : Applied primary server properties to Chat protocol");
                    logger.Debug("OpenChatProtocol : Primary server uri " + "tcp://" + primaryServer.ServerInfo.Host.IPaddress
                                                                                + ":" + primaryServer.ServerInfo.Port);
                    if (secondaryServer != null)
                    {
                        logger.Debug("OpenChatProtocol : Applied secondary server properties to Chst protocol");
                        logger.Debug("OpenChatProtocol : Secondary server uri " + "tcp://" + secondaryServer.ServerInfo.Host.IPaddress
                                                                                    + ":" + secondaryServer.ServerInfo.Port);
                    }
                    else
                    {
                        logger.Warn("OpenChatProtocol : Secondary server is not mentioned");
                        logger.Info("OpenChatProtocol : Application has no backup servers");
                    }
                    ProtocolManagers.Instance().DisConnectServer(ServerType.Chatserver);
                    chatServerConfProperties.Create(new Uri("tcp://" + primaryServer.ServerInfo.Host.IPaddress + ":" + primaryServer.ServerInfo.Port),
                        nickName, Genesyslab.Platform.WebMedia.Protocols.BasicChat.UserType.Agent, personId,
                        new Uri("tcp://" + secondaryServer.ServerInfo.Host.IPaddress + ":" + secondaryServer.ServerInfo.Port),
                        Convert.ToInt32(primaryServer.ServerInfo.Timeout), Convert.ToInt16(primaryServer.ServerInfo.Attempts),
                       Convert.ToInt32(Pointel.Interactions.Chat.Core.Util.Settings.AddpServerTimeout),
                       Convert.ToInt32(Pointel.Interactions.Chat.Core.Util.Settings.AddpClientTimeout),
                        Genesyslab.Platform.Commons.Connection.Configuration.AddpTraceMode.Both);
                    var ChatserverProtocolConfiguration = chatServerConfProperties.Protocolconfiguration;
                    string error = "";
                    if (!ProtocolManagers.Instance().ConnectServer(ChatserverProtocolConfiguration, out error))
                    {
                        logger.Error("Chat protocol is not opened due to, " + error);
                        output.MessageCode = "2001";
                        output.Message = error;
                        return output;
                    }
                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol =
                        ProtocolManagers.Instance().ProtocolManager[ChatserverProtocolConfiguration.Name] as Genesyslab.Platform.WebMedia.Protocols.BasicChatProtocol;

                    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Received += Pointel.Interactions.Chat.Core.Listener.BasicChatListener.GetInstance().BasicExtractor;
                    #region Old code
                    //if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol == null)
                    //{
                    //    logger.Debug("ConnectBasicChatServer : Applied primary server properties to Basic Chat protocol");

                    //    //Primary Server settings
                    //    logger.Debug("OpenChatProtocol : Primary server uri " + "tcp://" + primaryServer.ServerInfo.Host.IPaddress
                    //                                                              + ":" + primaryServer.ServerInfo.Port);
                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.Uri = new Uri("tcp://" + primaryServer.ServerInfo.Host.IPaddress
                    //                                                              + ":" + primaryServer.ServerInfo.Port);

                    //    if (secondaryServer != null)
                    //    {
                    //        logger.Debug("OpenChatProtocol : Applied secondary server properties to Basic Chat protocol");
                    //        //Backup Server settings
                    //        ChatConnectionSettings.GetInstance().ChatProtocolProperties.FaultTolerance = Genesyslab.Platform.ApplicationBlocks.Commons.Protocols.FaultToleranceMode.WarmStandby;
                    //        //HardCoded Values
                    //        ChatConnectionSettings.GetInstance().ChatProtocolProperties.WarmStandbyTimeout = Convert.ToInt32(primaryServer.ServerInfo.Timeout);
                    //        ChatConnectionSettings.GetInstance().ChatProtocolProperties.WarmStandbyAttempts = Convert.ToInt16(primaryServer.ServerInfo.Attempts);
                    //        //End

                    //        ChatConnectionSettings.GetInstance().ChatProtocolProperties.WarmStandbyUri = new Uri("tcp://" + secondaryServer.ServerInfo.Host.IPaddress +
                    //                                                                            ":" + secondaryServer.ServerInfo.Port);
                    //        logger.Debug("OpenChatProtocol : Secondary server uri " + "tcp://" + secondaryServer.ServerInfo.Host.IPaddress
                    //                                                                  + ":" + secondaryServer.ServerInfo.Port);
                    //    }
                    //    else
                    //    {
                    //        logger.Warn("OpenChatProtocol : Secondary server is not mentioned");
                    //        logger.Warn("OpenChatProtocol : Application has no backup servers");
                    //    }
                    //    //Set ADDP
                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.UseAddp = true;

                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.AddpServerTimeout = Convert.ToInt32(Pointel.Interactions.Chat.Core.Util.Settings.AddpServerTimeout);
                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.AddpClientTimeout = Convert.ToInt32(Pointel.Interactions.Chat.Core.Util.Settings.AddpClientTimeout);

                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.AddpTrace = "both";

                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.UserNickname = nickName;
                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.UserType = Genesyslab.Platform.WebMedia.Protocols.BasicChat.UserType.Agent;
                    //    ChatConnectionSettings.GetInstance().ChatProtocolProperties.PersonId = personId;

                    //    logger.Debug("OpenChatProtocol : Trying to register protocol manger for Basic Chat protocol");
                    //    if (ChatConnectionSettings.GetInstance().ChatProtocolManager == null)
                    //    {
                    //        ChatConnectionSettings.GetInstance().ChatProtocolManager = new Genesyslab.Platform.ApplicationBlocks.Commons.Protocols.ProtocolManagementService();
                    //        ChatConnectionSettings.GetInstance().ChatProtocolManager.Register(ChatConnectionSettings.GetInstance().ChatProtocolProperties);
                    //        logger.Debug("OpenChatProtocol : Registered protocol manager");
                    //    }
                    //    else
                    //    {
                    //        ChatConnectionSettings.GetInstance().ChatProtocolManager.Register(ChatConnectionSettings.GetInstance().ChatProtocolProperties);
                    //        logger.Debug("OpenChatProtocol : Registered protocol manager");
                    //    }
                    //    //Open the connection
                    //    ChatConnectionSettings.GetInstance().ChatProtocolManager[ChatConnectionSettings.ChatServer].Open();
                    //    Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol = (Genesyslab.Platform.WebMedia.Protocols.BasicChatProtocol)ChatConnectionSettings.GetInstance().ChatProtocolManager[ChatConnectionSettings.ChatServer];

                    //    ChatConnectionSettings.GetInstance().ChatEventBroker[0] = Genesyslab.Platform.ApplicationBlocks.Commons.Broker.BrokerServiceFactory.CreateEventBroker(ChatConnectionSettings.GetInstance().ChatProtocolManager.Receiver);
                    //    ChatConnectionSettings.GetInstance().ChatEventBroker[0].Activate();

                    //    ChatConnectionSettings.GetInstance().ChatEventBroker[0].Register(Pointel.Interactions.Chat.Core.Listener.BasicChatListener.GetInstance().BasicExtractor);

                    #endregion

                    if (Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                    {
                        Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Opened += new EventHandler(ChatConnectionManager_Opened);
                        Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.Closed += new EventHandler(ChatConnectionManager_Closed);
                        logger.Debug("OpenChatProtocol : Basic Chat protocol is opened ");
                        logger.Debug("OpenChatProtocol : Basic Chat protocol object id is "
                            + Pointel.Interactions.Chat.Core.Util.Settings.ChatProtocol.GetHashCode().ToString());
                        output.MessageCode = "200";
                        output.Message = "Chat Server Connected";
                    }
                    else
                    {
                        logger.Warn("OpenChatProtocol : Basic Chat protocol is closed ");
                    }
                }
                else
                {
                    logger.Error("OpenChatProtocol : No primary server configured.");
                    output.MessageCode = "2002";
                    output.Message = "No primary server configured. Could not able to connect Chat server";
                }
            }
            catch (Exception generalException)
            {
                if (switchoverServer)
                    output = OpenChatProtocol(nickName, personId, secondaryServer, primaryServer, false);
                else
                {
                    logger.Error("OpenChatProtocol :" + generalException.ToString());
                    output.MessageCode = "2001";
                    output.Message = generalException.Message;
                }
            }
            return output;
        }

        void ChatConnectionManager_Closed(object sender, EventArgs e)
        {
            Pointel.Interactions.Chat.Core.Util.Settings.MessageToClient.NotifyChatState("Closed");
        }

        void ChatConnectionManager_Opened(object sender, EventArgs e)
        {
            Pointel.Interactions.Chat.Core.Util.Settings.MessageToClient.NotifyChatState("Opened");
        }

        #endregion Methods
    }
}