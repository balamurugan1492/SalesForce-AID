using System.Collections;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;

namespace Pointel.Interactions.IPlugins
{
    public interface IChatPlugin
    {
        /// <summary>
        /// Notifies the chat interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        void NotifyChatInteraction(IMessage message);

        /// <summary>
        /// Notifies the interaction protocol.
        /// </summary>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        void NotifyInteractionProtocol(InteractionServerProtocol ixnProtocol);
        
        /// <summary>
        /// Initializes the chat.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="comObject">The COM object.</param>
        /// <param name="applicationName">Name of the application.</param>
        void InitializeChat(string userName, ConfService comObject, string applicationName, string placeName, IPluginCallBack chatListener);

        /// <summary>
        /// Notifies the state of the interaction server.
        /// </summary>
        /// <param name="isConnected">if set to <c>true</c> [is connected].</param>
        /// <param name="proxyClientID">The proxy client unique identifier.</param>
        void NotifyIXNState(bool isConnected, int? proxyClientID = null);

        /// <summary>
        /// Notifies the voice media status.
        /// </summary>
        /// <param name="isVoiceEnabled">if set to <c>true</c> [is voice enabled].</param>
        void NotifyVoiceMediaStatus(bool isVoiceEnabled);


        /// <summary>
        /// Notifies the place whenever refine place functionality invoked.
        /// </summary>
        /// <param name="place">The place.</param>
        void NotifyPlace(string place);
    }
}
