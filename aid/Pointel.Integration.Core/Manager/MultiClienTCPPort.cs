/*
* =====================================
* Pointel.Integration.Core
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 15-October-2015
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* ====================================
*/

namespace Pointel.Integration.Core.Manager
{

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Genesyslab.Platform.Commons.Protocols;
    using Pointel.Integration.Core.Helper;

    public enum RequestType { TCPClient, HTTPGet, HTTPPost };
    public class MultiClienTCPPort
    {
        #region Data Member
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
          "AID");
        private static List<TcpClient> IncomingClients = new List<TcpClient>();
        private static List<TcpClient> OutGoingClients = new List<TcpClient>();
        private static bool isdisconnect = false;
        public static int ClientCount
        {
            get
            {
                return IncomingClients.Count + OutGoingClients.Count;
            }
        }
        public static int InComingClientCount
        {
            get
            {
                return IncomingClients.Count;
            }
        }
        public static int OutgoingClientCount
        {
            get
            {
                return OutGoingClients.Count;
            }
        }
        public static int IncomingPort
        {
            get;
            private set;
        }
        public static int OutcomingPort
        {
            get;
            private set;
        }
        public delegate void ClientConnected(string host, int port);
        public delegate void ClientDisconnected(string host, int port);
        public delegate void DataRead(object sender, byte[] data);
        #endregion

        #region Events
        public static event ClientConnected ClientConnectedEvent;
        public static event ClientDisconnected ClientDisconnectedEvent;
        public static event DataRead DataReadEvent;
        #endregion

        #region Methods

        /// <summary>
        /// Starts the port server to listen income and out going port
        /// </summary>
        /// <param name="incomingport">The incomingport number.</param>
        /// <param name="outGoingPort">The out going port number.</param>
        public void Start(int incomingport, int outGoingPort)
        {
            try
            {
                logger.Info("------------MultiClienTCPPort-------");
                logger.Info("Incoming port: " + incomingport);
                logger.Info("Outgoing port: " + outGoingPort);
                IncomingPort = incomingport;
                OutcomingPort = outGoingPort;
                Thread incomingListener = new Thread(new ParameterizedThreadStart(StartServerListerner));
                incomingListener.Start(IncomingPort);
                Thread outGoingListener = new Thread(new ParameterizedThreadStart(StartServerListerner));
                outGoingListener.Start(OutcomingPort);
            }
            catch (Exception ex)
            {
                logger.Error("Start() : " + ex.Message);
            }
        }

        /// <summary>
        /// Stops all client to send and receive the data.
        /// </summary>
        public void Stop()
        {
            try
            {
                isdisconnect = true;
            }
            catch (Exception ex)
            {
                logger.Error("Stop() : " + ex.Message);
            }

        }

        /// <summary>
        /// Sends the data to all outgoing client.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SendData(object msg)
        {
            try
            {
                IMessage message = msg as IMessage;
                if(OutgoingClientCount>0)
                {
                    byte[] data = ASCIIEncoding.ASCII.GetBytes(PortServiceHelper.GetSendCallDetailString(message));
                    int count = OutgoingClientCount;
                    foreach (TcpClient client in OutGoingClients)
                    {
                        if (CheckClientConnection(client) && !isdisconnect)
                        {
                            try
                            {
                                NetworkStream stream = client.GetStream();
                                stream.Write(data, 0, data.Length);
                                Thread.Sleep(10);
                                //stream.Close();
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Error occurred while sending data as " + ex.Message);
                            }
                        }
                    }
                }               
               
            }
            catch (Exception ex)
            {
                logger.Error("SendData() : " + ex.Message);
            }
        }

        /// <summary>
        /// Starts the port server listerner for both incoming and outgoing server.
        /// </summary>
        /// <param name="portno">The portno.</param>
        private void StartServerListerner(object portno)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(portno.ToString()));
                TcpClient client;
                listener.Start();                
                while (!isdisconnect)                {
                    client = listener.AcceptTcpClient();
                    if (int.Parse(portno.ToString()) == IncomingPort)
                    {
                        ThreadPool.QueueUserWorkItem(ProcessIncomingClient, client);
                    }
                    else
                    {
                        OutGoingClients.Add(client);
                        if (OutgoingClientCount == 1)
                        {
                            Thread t = new Thread(new ThreadStart(CheckOutGoingListerner));
                            t.Start();
                        }
                        //while (client.Available > 0 && !isdisconnect)
                        //{
                        //    NetworkStream stream = client.GetStream();
                        //    byte[] data = new byte[client.Available];
                        //    if (stream.Read(data, 0, client.Available) > 0)
                        //    {
                        //        string msg = ASCIIEncoding.ASCII.GetString(data);
                        //        if (msg.Contains("/favicon.ico"))
                        //        {
                        //            byte[] respons = ASCIIEncoding.ASCII.GetBytes("+OK");
                        //            stream.Write(respons, 0, respons.Length);
                        //        }
                        //    }
                        //    stream.Close();
                        //}
                    }

                    string host;
                    int port;
                    GetHostAndPort(client, out host, out port);
                    ClientConnectedEvent.Invoke(host, port);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("SendData() : " + ex.Message);
            }
        }

        /// <summary>
        /// Checks outgoing client state, if they disconnect means the server will remove it all instance
        /// </summary>
        private void CheckOutGoingListerner()
        {
            try
            {

                while (!isdisconnect)
                {
                    Thread.Sleep(300);
                    try
                    {
                        TcpClient[] outGoingClientArray = OutGoingClients.ToArray();
                        for (int i = 0; i < outGoingClientArray.Length; i++)
                        {
                            if (!CheckClientConnection(outGoingClientArray[i]))
                            {
                                OutGoingClients.Remove(outGoingClientArray[i]);
                                ClientDisconnectedEvent.Invoke("sdfsd", 0);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error("CheckOutGoingListerner() : (while remove client from outgoing client) " + ex.Message);
                    }

                }

            }
            catch (Exception ex)
            {
                logger.Error("CheckOutGoingListerner() : " + ex.Message);
            }
        }

        /// <summary>
        /// Checks the client connection state. true means the client is alive. otherwise the client is gone.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        private bool CheckClientConnection(TcpClient client)
        {
            try
            {
                if (!client.Connected)
                    return false;
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        return false;
                    }
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                logger.Error("Error occurred while open the socket connection as " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                logger.Error("CheckClientConnection() : " + ex.Message);

            }
            return true;
        }

        /// <summary>
        /// Gets the host name and port number of the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        private void GetHostAndPort(TcpClient client, out string host, out int port)
        {
            host = null;
            port = 0;
            try
            {
                IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                IPAddress ipAddress = endPoint.Address;
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                host = hostEntry.HostName;
                port = endPoint.Port;
            }
            catch (Exception ex)
            {
                logger.Error("GetHostAndPort() : " + ex.Message);
            }
        }

        /// <summary>
        /// Checks the data availability to read in incoming client.
        /// </summary>
        /// <param name="client">The client.</param>
        private void CheckDataAvailabilityToRead(TcpClient client)
        {
            try
            {
                if (client.Available > 0 && !isdisconnect)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = new byte[client.Available];
                    if (stream.Read(data, 0, client.Available) > 0)
                    {
                        DataReadEvent.Invoke(client, data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CheckDataAvailabilityToRead() : " + ex.Message);
            }
        }

        /// <summary>
        /// Handle the process of incoming client
        /// </summary>
        /// <param name="client">The client.</param>
        private void ProcessIncomingClient(object client)
        {
            try
            {
                TcpClient tcpClient = client as TcpClient;
                IncomingClients.Add(tcpClient);
                while (!isdisconnect && CheckClientConnection(tcpClient))
                {
                    Thread.Sleep(200);
                    CheckDataAvailabilityToRead(tcpClient);
                }
                IncomingClients.Remove(tcpClient);
                string host;
                int port;
                GetHostAndPort(tcpClient, out host, out port);
                ClientDisconnectedEvent.Invoke(host, port);

            }
            catch (Exception ex)
            {
                logger.Error("RequestProcessClient() : " + ex.Message);
            }

        }

        #endregion
    }
}
