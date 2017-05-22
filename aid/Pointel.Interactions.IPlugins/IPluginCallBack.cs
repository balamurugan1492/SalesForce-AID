
namespace Pointel.Interactions.IPlugins
{
    public enum IXNState { None, Opened, Closed }
    public enum PluginType { Chat, Contact, Email, Workbin, OutboundPreview, SalesForce }
    public interface IPluginCallBack
    {
        /// <summary>
        /// Chats the dial events.
        /// </summary>
        /// <param name="dialEvent">The dial event.</param>
        void PluginDialEvents(PluginType pluginType, string dialEvent);

        /// <summary>
        /// Chats the interaction status.
        /// </summary>
        /// <param name="ixnState">State of the ixn.</param>
        void PluginInteractionStatus(PluginType pluginType, IXNState ixnState, bool isNotifier = false);


        /// <summary>
        /// Chats the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void PluginErrorMessage(PluginType pluginType, string message);
    }
}
