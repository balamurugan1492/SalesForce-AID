namespace Pointel.Integration.Core.Observers
{
   
    using System;
    using System.Text;
    using System.Net.Sockets;
    using Pointel.Integration.Core.Providers;
    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Util;
    using Pointel.Integration.Core.Manager;
    using Pointel.Integration.Core.Helper;
    using System.Threading;


    internal class PortSubscriber : IObserver<iCallData>
    {
        #region Field Declaration

        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private MultiClienTCPPort portListerner = null;
        private Settings setting = Settings.GetInstance();
        private string result = string.Empty;

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
                portListerner = new MultiClienTCPPort();
                MultiClienTCPPort.DataReadEvent += new MultiClienTCPPort.DataRead(MultiClienTCPPort_DataReadEvent);
                MultiClienTCPPort.ClientConnectedEvent += new MultiClienTCPPort.ClientConnected(MultiClienTCPPort_ClientConnectedEvent);
                MultiClienTCPPort.ClientDisconnectedEvent += new MultiClienTCPPort.ClientDisconnected(MultiClienTCPPort_ClientDisconnectedEvent);
                portListerner.Start(Settings.GetInstance().PortSetting.IncomingPortNumber, Settings.GetInstance().PortSetting.OutGoingPortNumber);
            }
            catch (Exception ex)
            {
                logger.Error("Error Occurred as  : " + ex.Message);
            }
        }

        #endregion Subscribe

        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>

        #region Unsubscribe

        public virtual void Unsubscribe()
        {
            //cancellation.Dispose();
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
                portListerner.Stop();
                Unsubscribe();
            }
            catch (Exception ex)
            {
                logger.Error("Error Occurred as " + ex.Message);
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
                if (value != null)
                {
                    if (value.EventMessage != null && (setting.PortSetting.CallDataEventType.Contains(value.EventMessage.Name) || setting.PortSetting.CallDataEventType.Contains(value.EventMessage.Name.ToLower())))
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(PortServiceHelper.InsertCallDetails));
                        Thread threadSend = new Thread(new ParameterizedThreadStart(portListerner.SendData));
                        t.Start(value.EventMessage);
                        threadSend.Start(value.EventMessage);
                        
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while send call data to client application " + generalException.ToString());
            }
        }

        #endregion OnNext
        
        /// <summary>
        /// Receives the client connection.
        /// </summary>
        /// <param name="callData">The call data.</param>

        #region ReceiveClientConn

        public void ReceiveClientConn(IAsyncResult callData)
        {
        }

        #endregion ReceiveClientConn

        #region  Multiclient Listerner Events
        void MultiClienTCPPort_ClientDisconnectedEvent(string host, int port)
        {
            try
            {
                logger.Info("Client disconnected from Host:" + host + ",port:" + port);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            //throw new NotImplementedException();
        }

        void MultiClienTCPPort_ClientConnectedEvent(string host, int port)
        {
            try
            {
                logger.Info("Client connected from Host:" + host + ",port:" + port);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
            // throw new NotImplementedException();
        }

        void MultiClienTCPPort_DataReadEvent(object sender, byte[] data)
        {
            try
            {
                string message = ASCIIEncoding.ASCII.GetString(data);
                PortDataParser parser = new PortDataParser();
                if (parser.GetRequestType(message) == RequestType.HTTPGet)
                {
                    message = parser.ParseGetMethodData(message);
                    int dataIndex = message.IndexOf("?");
                    if (dataIndex >= 0)
                        dataIndex++;
                    if (dataIndex > 0)
                    {
                        message = message.Substring(dataIndex);
                    }                    
                    if (!message.Equals("/favicon.ico"))
                    {
                        message = message.Replace("%20", " ");
                    }
                    TcpClient client = sender as TcpClient;
                    NetworkStream stream = client.GetStream();
                    byte[] respons = ASCIIEncoding.ASCII.GetBytes("+OK");
                    stream.Write(respons, 0, respons.Length);
                    stream.Close();
                }
                if (message!=null && !message.Equals("/favicon.ico"))
                {
                    logger.Info("Data received : " + message);
                }                
                PortServiceHelper.UpdateCallDetails(message);
            }
            catch (Exception ex)
            {
                logger.Error("Error Occurred as " + ex.Message);
            }
        }

        #endregion




    }
}