using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Pointel.Interactions.Core.Common;

namespace Pointel.Interactions.Core
{
    public enum IXNServerState { None, Opened, Closed } 
    /// <summary>
    /// 
    /// </summary>
    public interface IInteractionServices
    {
        /// <summary>
        /// This method notify appropriate library about the current interaction
        /// ex.  If email and chat library subscribed this DLL, both library notified with correct interactions
        /// email library will not be notified about chat interaction and same chat will not be notified about email interaction.
        /// </summary>
        void NotifyEServiceInteraction(IMessage message,InteractionTypes types);


        /// <summary>
        /// This method will provide interaction protocol
        /// </summary>
        void NotifyInteractionProtocol(InteractionServerProtocol interactionProtocol);


        /// <summary>
        /// Notifies the ixn agent media status.
        /// </summary>
        /// <param name="outputvalues">The outputvalues.</param>
        void NotifyIxnAgentMediaStatus(IMessage message);



        /// <summary>
        /// Notifies the interaction server status.
        /// </summary>
        /// <param name="state">The state.</param>
        void NotifyInteractionServerStatus(IXNServerState state);
    }
}
