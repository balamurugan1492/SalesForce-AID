using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class used to register DN's in the given Place
    /// </summary>
    internal class RequestRegisterPlace
    {
        /// <summary>
        /// This method used to register ACD Position or Extension to TServer
        /// </summary>
        /// <param name="dnNumber">ACD Position/Extension</param>
        /// <returns>output</returns>

        #region RegisterDN

        public static Pointel.Softphone.Voice.Common.OutputValues RegisterDN(string dnNumber)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var output = Pointel.Softphone.Voice.Common.OutputValues.GetInstance();
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(dnNumber);

                var requestRegisterAddress = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestRegisterAddress.Create(dnNumber, Genesyslab.Platform.Voice.Protocols.TServer.RegisterMode.ModeShare, Genesyslab.Platform.Voice.Protocols.TServer.ControlMode.RegisterDefault, Genesyslab.Platform.Voice.Protocols.TServer.AddressType.DN);
                Settings.GetInstance().VoiceProtocol.Send(requestRegisterAddress);
                logger.Info("---------------RegisterDN------------------");
                logger.Info("DN:" + dnNumber);
                logger.Info("--------------------------------------------");
                output.MessageCode = "200";
                output.Message = dnNumber + " registration request send to T-Server ";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while registering " + dnNumber + " : " + commonException.ToString());
                output.MessageCode = "2001";
                output.Message = dnNumber + " registration request failed ";
            }
            return output;
        }

        public static Genesyslab.Platform.Commons.Protocols.IMessage RequestRegisterDN(string dnNumber)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            try
            {
                //Input Validation
                Pointel.Softphone.Voice.Core.Exceptions.CheckException.CheckDialValues(dnNumber);

                var requestRegisterAddress = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn.RequestRegisterAddress.Create(dnNumber, Genesyslab.Platform.Voice.Protocols.TServer.RegisterMode.ModeShare, Genesyslab.Platform.Voice.Protocols.TServer.ControlMode.RegisterDefault, Genesyslab.Platform.Voice.Protocols.TServer.AddressType.DN);
                Genesyslab.Platform.Commons.Protocols.IMessage message = Settings.GetInstance().VoiceProtocol.Request(requestRegisterAddress);
                return message;
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while registering " + dnNumber + " : " + commonException.ToString());
                return null;
            }
        }

        #endregion RegisterDN
    }
}