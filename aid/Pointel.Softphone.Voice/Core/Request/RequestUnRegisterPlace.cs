using Pointel.Softphone.Voice.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle unregister DN
    /// </summary>
    internal class RequestUnRegisterPlace
    {
        /// <summary>
        /// This method used to unregisteredDN
        /// </summary>
        /// <param name="dnNumber">The dn number.</param>
        /// <returns>OutputValues.</returns>

        #region UnRegisterDN

        public static Pointel.Softphone.Voice.Common.OutputValues UnRegisterDN(string dnNumber)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
               "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(dnNumber);

                var extensions = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                var requestUnRegisterAddress = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestUnregisterAddress.Create(dnNumber, Genesyslab.Platform.Voice.Protocols.TServer.ControlMode.RegisterDefault, extensions);

                Settings.GetInstance().VoiceProtocol.Send(requestUnRegisterAddress);
                logger.Info("---------------UnRegisterDN------------------");
                logger.Info("DN:" + dnNumber);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = dnNumber + " is Unregistered ";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Unregister " + dnNumber + " : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = dnNumber + " is not Unregistered ";
            }
            return output;
        }

        public static IMessage UnRegisterDNRequest(string dnNumber) 
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
               "AID");
            IMessage message = null;
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(dnNumber);

                var extensions = new Genesyslab.Platform.Commons.Collections.KeyValueCollection();
                var requestUnRegisterAddress = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestUnregisterAddress.Create(dnNumber, Genesyslab.Platform.Voice.Protocols.TServer.ControlMode.RegisterDefault, extensions);

                message = Settings.GetInstance().VoiceProtocol.Request(requestUnRegisterAddress);
                logger.Info("---------------UnRegisterDNRequest------------------");
                logger.Info("DN:" + dnNumber);
                logger.Info("---------------------------------------------------");
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while Unregister " + dnNumber + " : " + commonException.ToString());
            }
            return message;
        }

        #endregion UnRegisterDN
    }
}