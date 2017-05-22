namespace Pointel.Salesforce.Plugin
{
    using Genesyslab.Platform.Commons.Protocols;

    #region Enumerations

    public enum AgentStatus
    {
        Unknown = 0,
        Login = 1,
        Ready = 2,
        NotReady = 3,
        NotReadyActionCode = 4,
        NotReadyAfterCallWork = 5,
        DndOn = 6,
        LogoutDndOn = 7,
        Logout = 8,
        OutOfService = 9,
    }

    public enum MessageMode
    {
        Info = 0, Debug = 1, Warn = 2, Error = 3
    }

    public enum SFDCConnectionStatus
    {
        Connected = 0,
        NotConnected = 1,
    }

    #endregion Enumerations

    #region Delegates

    public delegate AgentState AgentStateHandler();

    public delegate void ConnectionStatusChange(SFDCConnectionStatus status);

    public delegate void ConnectionStatusMessage(MessageMode mode, string message);

    public delegate void LogMessage(MessageMode mode, string message);

    public delegate void NotifySFDCUrl(string url);

    #endregion Delegates

    public interface ISFDCConnector
    {
        #region Events

        event AgentStateHandler AgentState;

        event LogMessage LogMessage;

        event ConnectionStatusMessage NotifyConnectionStatusMessage;

        event ConnectionStatusChange NotifySFDCConnectionStatusChanges;

        event NotifySFDCUrl NotifyUrl;

        #endregion Events

        #region Methods

        void NotifyChatDispositionCode(string interactionId, string dispKeyName, string dispCode);

        void NotifyInteractionEvents(IMessage message);

        void NotifyVoiceDispositionCode(IMessage message);

        void PopupBrowser();

        void Subscribe(SFDCConnectionOptions options);

        void UnSubscribe();

        #endregion Methods
    }
}