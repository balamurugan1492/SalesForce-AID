//using Pointel.Softphone.Voice.Core;

namespace Pointel.Softphone.Voice.Core.Common
{
    /// <summary>
    /// This class provide button status based on events
    /// </summary>
    internal class ButtonStatusController
    {
        /// <summary>
        /// This method used to get login button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetLoginStatus

        public static SoftPhoneStatusController GetLoginStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = true;
            status.ReadyButtonStatus = true;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = true;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetLoginStatus

        /// <summary>
        /// This method used to get logout button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetLogoutStatus

        public static SoftPhoneStatusController GetLogoutStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = true;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetLogoutStatus

        /// <summary>
        /// This method used to get Ready button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetReadyStatus

        public static SoftPhoneStatusController GetReadyStatus(bool enable)
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            //Enable/Disable Logout button when agent is on Ready state
            //control by "LogoffEnable" key
            if (enable)
                status.LogoutButtonStatus = true;
            else
                status.LogoutButtonStatus = false;
            //end
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = true;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = true;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetReadyStatus

        /// <summary>
        /// This method used to get NotReady button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetNotReadyStatus

        public static SoftPhoneStatusController GetNotReadyStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = true;
            status.ReadyButtonStatus = true;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = true;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetNotReadyStatus

        /// <summary>
        /// This method used to get Ringing button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCallRiningStatus

        public static SoftPhoneStatusController GetCallRiningStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = true;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCallRiningStatus

        /// <summary>
        /// This method used to get dialing button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCallDialingStatus

        public static SoftPhoneStatusController GetCallDialingStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = true;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCallDialingStatus

        /// <summary>
        /// This method used to get OnCall button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetOnCallStatus

        public static SoftPhoneStatusController GetOnCallStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = true;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = true;
            status.ConferenceInitiateStatus = true;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = true;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;
            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetOnCallStatus

        /// <summary>
        /// This method used to get Delete Conference button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetDeleteConferenceStatus

        public static SoftPhoneStatusController GetDeleteConferenceStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = true;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = true;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = true;
            //End
            status.TransferInitiateStatus = true;
            status.TransferCompleteStatus = false;

            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetDeleteConferenceStatus

        /// <summary>
        /// This method used to get OnHold button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCallOnHoldStatus

        public static SoftPhoneStatusController GetCallOnHoldStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = true;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCallOnHoldStatus

        /// <summary>
        /// This method used to get CallOnRelease button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCallOnReleaseStatus

        public static SoftPhoneStatusController GetCallOnReleaseStatus(bool logOffStatus)
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            if (logOffStatus)
                status.LogoutButtonStatus = true;
            else
                status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = true;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = true;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCallOnReleaseStatus

        /// <summary>
        /// This method used to get Initiate Transfer button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetInitiateTransferStatus

        public static SoftPhoneStatusController GetInitiateTransferStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            //Below condition should be false while initiating trasfer
            //status.TransferCompleteStatus = true;
            status.TransferCompleteStatus = false;
            //End
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = true;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetInitiateTransferStatus

        /// <summary>
        /// This method used to get initiate Conference button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetInitiateConferenceStatus

        public static SoftPhoneStatusController GetInitiateConferenceStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = true;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetInitiateConferenceStatus


        #region GetAlternateStatus

        /// <summary>
        /// Gets the alternate status.
        /// </summary>
        /// <returns></returns>
        public static SoftPhoneStatusController GetAlternateStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = true;
            status.MergeButtonStatus = true;
            //End
            return status;
        }

        #endregion GetAlternateStatus

        /// <summary>
        /// This method used to get accept conference status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetAcceptConferenceStatus

        public static SoftPhoneStatusController GetAcceptConferenceStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = true;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = true;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = true;
            status.MergeButtonStatus = true;
            //End
            return status;
        }

        #endregion GetAcceptConferenceStatus

        /// <summary>
        /// This method used to get Accept Transfer button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetAcceptTransferStatus

        public static SoftPhoneStatusController GetAcceptTransferStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = true;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = true;
            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = true;
            status.MergeButtonStatus = true;
            //End
            return status;
        }

        #endregion GetAcceptTransferStatus

        /// <summary>
        /// This method used to get Complete Transfer status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCompleteTransferStatus

        public static SoftPhoneStatusController GetCompleteTransferStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = true;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCompleteTransferStatus

        /// <summary>
        /// This method used to get Complete conference status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCompleteConferenceStatus

        public static SoftPhoneStatusController GetCompleteConferenceStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = true;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = true;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCompleteConferenceStatus

        /// <summary>
        /// This method used to get all button disable status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetAllButtonDisableStatus

        public static SoftPhoneStatusController GetAllButtonDisableStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetAllButtonDisableStatus

        /// <summary>
        /// This method used to get initial softphone status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetInitialSoftPhoneStatus

        public static SoftPhoneStatusController GetInitialSoftPhoneStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = true;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = false;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetInitialSoftPhoneStatus

        /// <summary>
        /// This method used to get Release button status
        /// </summary>
        /// <returns>SoftPhoneStatusController</returns>

        #region GetCallReleaseStatus

        public static SoftPhoneStatusController GetCallReleaseStatus()
        {
            SoftPhoneStatusController status = new SoftPhoneStatusController();

            status.LoginButtonStatus = false;
            status.LogoutButtonStatus = false;
            status.ReadyButtonStatus = false;
            status.NotreadyButtonStatus = false;
            status.HoldButtonStatus = false;
            status.RetrieveButtonStatus = false;
            status.AnswerButtonStatus = false;
            status.ReleaseButtonStatus = true;
            status.ConferenceInitiateStatus = false;
            status.CompleteConferenceStatus = false;
            status.TransferInitiateStatus = false;
            status.TransferCompleteStatus = false;
            //Code Added - V.Palaniappan
            //09.12.2013
            status.DeleteConferenceStatus = false;
            //End
            status.CancelConferenceStatus = false;
            status.CancelTransferStatus = false;

            status.DialButtonStatus = false;
            //Code Added -V.Palaniappan
            //04.12.2013
            status.AlternateButtonStatus = false;
            status.MergeButtonStatus = false;
            //End
            return status;
        }

        #endregion GetCallReleaseStatus
    }
}