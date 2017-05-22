using Pointel.Softphone.Voice.Core.Util;
namespace Pointel.Softphone.Voice.Core.Request
{
    /// <summary>
    /// This class provide to handle Merge call
    /// </summary>
    internal class RequestAgentMergeCall
    {
        // <summary>
        /// This method is used to do Merge Call.
        /// </summary>
        /// <returns>OutputValues.</returns>

        # region MergeCall

        public static Pointel.Softphone.Voice.Common.OutputValues MergeCall(string type)
        {
            var logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            var outputValues = new Pointel.Softphone.Voice.Common.OutputValues();
            Settings.GetInstance().IsAlternateCallClicked = false;
            Genesyslab.Platform.Voice.Protocols.ConnectionId connId;
            Genesyslab.Platform.Voice.Protocols.ConnectionId prevConnId;
            try
            {
                var requestMergeCall = Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party.RequestMergeCalls.Create();
                requestMergeCall.ThisDN = (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN));
                //Code Added - V.Palaniappan
                //09.12.2013
                if (Settings.GetInstance().ConsultConnectionID == Settings.GetInstance().ConnectionID)
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                }
                else
                {
                    //Code Added - This condition has been added while doing after Accepting Initiate Transfer by another agent
                    //10.12.2013 - V.Palaniappan
                    if (Settings.GetInstance().ConsultConnectionID == Settings.GetInstance().ConsultationConnID)
                    {
                        connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                    }
                    else
                    {
                        connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultConnectionID);
                    }//End
                }

                prevConnId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultationConnID);
                //Code Added - To check whether prevconnid is greater than connid and interchange both connectionid to do merge call
                //20.1.2014 - V.Palaniappan
                int result = prevConnId.ToString().CompareTo(connId.ToString());
                if (result < 0)
                {
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                    prevConnId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultationConnID);
                }
                else if (result > 0)
                {
                    prevConnId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConnectionID);
                    connId = new Genesyslab.Platform.Voice.Protocols.ConnectionId(Settings.GetInstance().ConsultationConnID);
                }
                //End
                requestMergeCall.ConnID = prevConnId;
                requestMergeCall.TransferConnID = connId;
                if (string.Compare(type, "ForTransfer") == 0)
                {
                    requestMergeCall.MergeType = Genesyslab.Platform.Voice.Protocols.TServer.MergeType.ForTransfer;
                }
                else
                {
                    requestMergeCall.MergeType = Genesyslab.Platform.Voice.Protocols.TServer.MergeType.ForConference;
                }
                logger.Info("ThisDN:" + (Settings.GetInstance().CallControl == "both" ? Settings.GetInstance().ActiveDN : (Settings.GetInstance().CallControl == "acd" ?
                                        Settings.GetInstance().ACDPosition : Settings.GetInstance().ExtensionDN)));
                //End
                logger.Info("ConnectionID:" + connId);
                logger.Info("PreviousConnID:" + prevConnId);
                logger.Info("--------------------------------------------");
                Settings.GetInstance().VoiceProtocol.Send(requestMergeCall);
                outputValues.MessageCode = "200";
                outputValues.Message = "MergeCall Successful";
            }
            catch (System.Exception commonException)
            {
                logger.Error("Error occurred while doing Merge call " + commonException.ToString());
                outputValues.MessageCode = "2001";
                outputValues.Message = commonException.Message;
            }
            return outputValues;
        }

        # endregion
    }
}