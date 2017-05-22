namespace Pointel.Integration.Core.Observers
{
    #region System Namespaces

    using System;
    using System.IO.Pipes;
    using System.Linq;

    #endregion System Namespaces

    #region log4Net Namespace



    #endregion log4Net Namespace

    #region Genesys Namespaces

    using Genesyslab.Platform.Commons.Collections;

    #endregion Genesys Namespaces

    #region AID Namespaces

    using Pointel.Integration.Core.iSubjects;
    using Pointel.Integration.Core.Providers;
    using System.Threading;
    using Pointel.Integration.Core.Helper;
    using Pointel.Integration.Core.Data;

    #endregion AID Namespaces

    internal class PipeSubscriber : IObserver<iCallData>
    {
        #region Field Declaration

        private IDisposable cancellation;
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private NamedPipeServerStream server;
        private NamedPipeClientStream pipeClient;
        private PipeIntegrationData objConfiguration;

        #endregion Field Declaration

        #region Constructors

        internal PipeSubscriber(PipeIntegrationData objConfiguration)
        {
            this.objConfiguration = objConfiguration;
        }

        #endregion

        #region Method implementation

        /// <summary>
        /// Subscribes the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>

        public virtual void Subscribe(CallDataProviders provider)
        {
            try
            {
                cancellation = provider.Subscribe(this);
                if (objConfiguration.IsServer)
                {
                    logger.Info("The pipe '" + objConfiguration.PipeName + "' is enabled server.");
                    Thread pipeServerThread = new Thread(new ThreadStart(StartServer));
                    pipeServerThread.Start();
                }
            }
            catch (Exception generalExcetion)
            {
                logger.Error("Error occurred as " + generalExcetion.Message);
            }
        }

        /// <summary>
        /// Unsubscribe this instance.
        /// </summary>

        public virtual void Unsubscribe()
        {
            objConfiguration = null;
            if (pipeClient != null)
            {
                if (pipeClient.IsConnected)
                {
                    pipeClient.Flush();
                    pipeClient.Close();
                }
                pipeClient.Dispose();
            }
        }

        #endregion

        #region Observer Method implementation

        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>

        public void OnNext(iCallData value)
        {

            Thread pipeThread = new Thread(delegate()
            {
                try
                {
                    if (value == null || value.EventMessage == null)
                    {
                        logger.Warn("Event detail is null.");
                        return;
                    }

                    if (objConfiguration.MediaType != value.MediaType)
                        return;

                    if (objConfiguration.PipeEvent == null || objConfiguration.PipeEvent.Where(x=>x==value.EventMessage.Name.ToLower()).ToList().Count==0)
                        return;

                    Type objType = null;
                    object obj = null;
                    KeyValueCollection userdata = null;

                    // For Convert voice 
                    if (value.MediaType == MediaType.Voice)
                    {
                        MediaEventHelper objEventHelper = new MediaEventHelper();
                        if (!objEventHelper.ConvertVoiceEvent(ref objType, ref obj, ref userdata, value.EventMessage))
                            logger.Warn("Voice event conversion getting failed");
                    }
                    else if (value.MediaType == MediaType.Email)
                    {

                    }
                    else if (value.MediaType == MediaType.Chat)
                    {

                    }
                    else if (value.MediaType == MediaType.SMS)
                    {

                    }

                    // Functionality to send data in the specified format.
                    if (objType != null && obj != null)
                    {
                        switch (objConfiguration.PipeFormat)
                        {
                            case "text":
                                SendTextData(objType, obj, userdata);
                                break;
                            case "json":
                                SendJsonData(objType, obj, userdata);
                                break;
                            case "xml":
                                SendXMLData(objType, obj, userdata);
                                break;
                            case "custom":
                                SendTextData(objType, obj, userdata);
                                break;
                            default:
                                logger.Warn("The specified format not supported in the pipe integration");
                                break;
                        }

                    }
                    else
                        logger.Warn("Required data is null.");
                }
                catch (Exception generalException)
                {
                    logger.Error("Error occurred while writing call data to a file " + generalException.ToString());
                }

            });
            pipeThread.Start();
        }


        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>

        public void OnCompleted()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Data transmission Methods Implementation

        private void SendTextData(Type objType, object obj, KeyValueCollection userData)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string result = objDataParser.ParseTextString(objType, obj, userData, objConfiguration.DataToPost
                    , objConfiguration.Delimeter, objConfiguration.ValueSeperator, objConfiguration.DefaultValueToNull);
                SendData(result, objConfiguration.PipeName);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void SendJsonData(Type objType, object obj, KeyValueCollection userData)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string data = objDataParser.ParseJsonString(objType, obj, userData, objConfiguration.DataToPost, objConfiguration.DefaultValueToNull);
                SendData(data, objConfiguration.PipeName);

            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void SendXMLData(Type objType, object obj, KeyValueCollection userData)
        {
            try
            {
                DataParser objDataParser = new DataParser();
                string result = objDataParser.ParseXML(objType, obj, userData, objConfiguration.DataSection);
                SendData(result, objConfiguration.PipeName);
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }


        #endregion

        #region Pipe Implementation ( Both Client and Server with data transmission)

        private void StartServer()
        {
            try
            {
                if (server != null || !server.IsConnected)
                    server = new NamedPipeServerStream(objConfiguration.PipeName, PipeDirection.InOut, int.MaxValue, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
            }
        }

        private void SendDataThroughClient(string data, string pipeName)
        {
            try
            {
                if (pipeClient == null)
                    pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous);
                if (!pipeClient.IsConnected)
                    pipeClient.Connect();
                if (pipeClient.IsConnected)
                {
                    byte[] dataToWrite = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
                    pipeClient.Write(dataToWrite, 0, dataToWrite.Length);
                }
            }
            catch (Exception _generalException)
            {
                logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void SendDataThroughServer(string data)
        {
            try
            {
                if (server != null || server.IsConnected)
                {

                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.Message);
            }
        }

        private void SendData(string result, string pipeName)
        {
            try
            {
                if (objConfiguration.IsServer)
                    SendDataThroughServer(result);
                else
                    SendDataThroughClient(result, pipeName);
            }
            catch (Exception generalExcetion)
            {
                logger.Error("Error occurred as " + generalExcetion.Message);
            }
        }

        #endregion SendData

    }
}