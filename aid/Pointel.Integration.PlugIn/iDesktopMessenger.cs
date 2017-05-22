namespace Pointel.Integration.PlugIn
{
    using System.Collections.Generic;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;

    using Pointel.Desktop.Access.Control;
    using Pointel.Integration.PlugIn.Common;

    /// <summary>
    /// This interface provide to subscribe events
    /// </summary>
    public interface IDesktopMessenger
    {
        #region Methods

        /// <summary>
        /// Gets the agent information.
        /// </summary>
        /// <param name="agentInfo">The agent information.</param>
        void GetAgentInfo(AgentInfo agentInfo);

        /// <summary>
        /// Initializes the integration.
        /// </summary>
        /// <param name="confProtocol">The conf protocol.</param>
        /// <param name="applicationName">Name of the application.</param>
        void InitializeIntegration(ConfService confProtocol, string applicationName, System.Collections.Generic.Dictionary<string, bool> integrationMediaList = null);

        void NotifyEvent(Genesyslab.Platform.Commons.Protocols.IMessage message, int media = 0);

        void StartHIMMSIntegration();

        void StartWebIntegration(List<ApplicationDataDetails> lstApplication);

        void PopupMID(string MID);

        /// <summary>
        /// Subscribes the specified message to client.
        /// </summary>
        /// <param name="messagetoClient">The message to client.</param>
        void Subscribe(IDesktopCommunicator messagetoClient);

        /// <summary>
        /// Unsubcribe.
        /// </summary>
        void UnSubcribe();

        #endregion Methods
    }
}