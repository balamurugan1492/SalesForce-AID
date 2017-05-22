namespace Pointel.Softphone.Voice.Core
{
    /// <summary>
    /// This class provide to handle Button Status
    /// </summary>
    public class SoftPhoneStatusController
    {
        #region Fields

        private bool alternateButtonStatus;
        private bool answerButtonStatus;
        private bool cancelConference;
        private bool cancelTransfer;
        private bool completeConferenceStatus;
        private bool conferenceInitiateStatus;
        private bool deleteConferenceStatus;
        private bool dialButtonStatus;
        private bool holdButtonStatus;
        private bool loginButtonStatus;
        private bool logoutButtonStatus;
        private bool mergeButtonStatus;
        private bool notreadyButtonStatus;
        private bool readyButtonStatus;
        private bool releaseButtonStatus;
        private bool retrieveButtonStatus;
        private bool transferCompleteStatus;
        private bool transferInitiateStatus;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [alternate button status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [alternate button status]; otherwise, <c>false</c>.
        /// </value>
        public bool AlternateButtonStatus
        {
            get { return alternateButtonStatus; }
            set { alternateButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [answer button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [answer button status]; otherwise, <c>false</c>.
        /// </value>
        public bool AnswerButtonStatus
        {
            get { return answerButtonStatus; }
            set { answerButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel conference status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [cancel conference status]; otherwise, <c>false</c>.
        /// </value>
        public bool CancelConferenceStatus
        {
            get { return cancelConference; }
            set { cancelConference = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel transfer status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [cancel transfer status]; otherwise, <c>false</c>.
        /// </value>
        public bool CancelTransferStatus
        {
            get { return cancelTransfer; }
            set { cancelTransfer = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [complete conference status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [complete conference status]; otherwise, <c>false</c>.
        /// </value>
        public bool CompleteConferenceStatus
        {
            get { return completeConferenceStatus; }
            set { completeConferenceStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [conference initiate status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [conference initiate status]; otherwise, <c>false</c>.
        /// </value>
        public bool ConferenceInitiateStatus
        {
            get { return conferenceInitiateStatus; }
            set { conferenceInitiateStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [Delete conference status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [Delete conference status]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteConferenceStatus
        {
            get { return deleteConferenceStatus; }
            set { deleteConferenceStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dial button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dial button status]; otherwise, <c>false</c>.
        /// </value>
        public bool DialButtonStatus
        {
            get { return dialButtonStatus; }
            set { dialButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hold button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hold button status]; otherwise, <c>false</c>.
        /// </value>
        public bool HoldButtonStatus
        {
            get { return holdButtonStatus; }
            set { holdButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [login button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [login button status]; otherwise, <c>false</c>.
        /// </value>
        public bool LoginButtonStatus
        {
            get { return loginButtonStatus; }
            set { loginButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [logout button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [logout button status]; otherwise, <c>false</c>.
        /// </value>
        public bool LogoutButtonStatus
        {
            get { return logoutButtonStatus; }
            set { logoutButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [merge button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [merge button status]; otherwise, <c>false</c>.
        /// </value>
        public bool MergeButtonStatus
        {
            get { return mergeButtonStatus; }
            set { mergeButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [notready button status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [notready button status]; otherwise, <c>false</c>.
        /// </value>
        public bool NotreadyButtonStatus
        {
            get { return notreadyButtonStatus; }
            set { notreadyButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ready button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ready button status]; otherwise, <c>false</c>.
        /// </value>
        public bool ReadyButtonStatus
        {
            get { return readyButtonStatus; }
            set { readyButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [release button status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [release button status]; otherwise, <c>false</c>.
        /// </value>
        public bool ReleaseButtonStatus
        {
            get { return releaseButtonStatus; }
            set { releaseButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [retrieve button status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [retrieve button status]; otherwise, <c>false</c>.
        /// </value>
        public bool RetrieveButtonStatus
        {
            get { return retrieveButtonStatus; }
            set { retrieveButtonStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [transfer complete status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [transfer complete status]; otherwise, <c>false</c>.
        /// </value>
        public bool TransferCompleteStatus
        {
            get { return transferCompleteStatus; }
            set { transferCompleteStatus = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [transfer initiate status].
        /// </summary>
        /// <value>
        /// <c>true</c> if [transfer initiate status]; otherwise, <c>false</c>.
        /// </value>
        public bool TransferInitiateStatus
        {
            get { return transferInitiateStatus; }
            set { transferInitiateStatus = value; }
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            string details = "";
            if (AlternateButtonStatus)
                details += "AlternateButtonStatus\n";
            if (AnswerButtonStatus)
                details += "AnswerButtonStatus\n";
            if (CancelConferenceStatus)
                details += "CancelConferenceStatus\n";
            if (CancelTransferStatus)
                details += "CancelTransferStatus\n";
            if (ConferenceInitiateStatus)
                details += "ConferenceInitiateStatus\n";
            if (CompleteConferenceStatus)
                details += "CompleteConferenceStatus\n";
            if (DeleteConferenceStatus)
                details += "DeleteConferenceStatusd\n";
            if (DialButtonStatus)
                details += "DialButtonStatus\n";
            if (HoldButtonStatus)
                details += "HoldButtonStatus\n";
            if (LoginButtonStatus)
                details += "LoginButtonStatus\n";
            if (LogoutButtonStatus)
                details += "LogoutButtonStatus\n";
            if (MergeButtonStatus)
                details += "MergeButtonStatus\n";
            if (NotreadyButtonStatus)
                details += "NotreadyButtonStatus\n";
            if (ReadyButtonStatus)
                details += "ReadyButtonStatus\n";
            if (ReleaseButtonStatus)
                details += "ReleaseButtonStatus\n";
            if (RetrieveButtonStatus)
                details += "RetrieveButtonStatus\n";
            if (TransferCompleteStatus)
                details += "TransferCompleteStatus\n";
            if (TransferInitiateStatus)
                details += "TransferInitiateStatus\n";
            return string.IsNullOrEmpty(details) ? base.ToString() : details;
        }

        #endregion Methods
    }
}