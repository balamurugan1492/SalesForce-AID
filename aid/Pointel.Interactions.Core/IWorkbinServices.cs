using Genesyslab.Platform.Commons.Protocols;

namespace Pointel.Interactions.Core
{
    public interface IWorkbinServices
    {
        /// <summary>
        /// Notifies the work-bin interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="types">The types.</param>
        void NotifyWorkbinInteraction(IMessage message, InteractionTypes types);
    }
}
