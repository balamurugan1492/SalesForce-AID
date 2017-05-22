using System.Collections;
using System.Collections.Generic;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.IPlugins
{
    public enum WINPosition { Height, Width, Left};
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailPlugin 
    {
        /// <summary>
        /// Notifies the email interaction.
        /// </summary>
        /// <param name="message">The message.</param>
        void NotifyEmailInteraction(IMessage message);
        
        /// <summary>
        /// Notifies the contact protocol.
        /// </summary>
        /// <param name="ucsProtocol">The ucs protocol.</param>
        void NotifyContactProtocol(UniversalContactServerProtocol ucsProtocol);

        /// <summary>
        /// Initializes the email.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="agentId">The agent unique identifier.</param>
        /// <param name="placeId">The place unique identifier.</param>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <param name="windowPostioning">The window postioning.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="Object">The object.</param>
        /// <param name="applicationName">Name of the application.</param>
        //void InitializeEmail(string username, string placeName, string agentId, ConfService Object,int tenantDbId, string applicationName, Dictionary<WINPosition, double> windowPostioning, IPluginCallBack listener, Hashtable mediaPlugins);

        void InitializeEmail(ConfService Object, IPluginCallBack listener, Dictionary<string, string> agentInfo);

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="getresponse">The getresponse.</param>
        //void GetResponse(string response, string name);
        /// <summary>
        /// Shows the Contact directory address.
        /// </summary>
        /// <param name="to">The automatic.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        void ShowContactDirectoryAddress(string to, string cc, string bcc);

        void NotifyEmailReply(string parentIxnId, bool isReplyAll = false);

        bool IsEmailOpened(string interactionId);
        /// <summary>
        /// Intimates the selected agent and consult type
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="searchedType">Type of the searched.</param>
        /// <param name="searchedValue">The searched value.</param>
        //void DialEvent(Pointel.Interactions.IPlugins.OperationType operationType, string searchedType, string searchedValue, string place);

        void NotifyEmailLoginState(bool isLoggedIn = true);

        void NotifyContactProtocolState(bool isLoggedIn = true);

        void NotifyNewEmail(string emailAddress, string contactID,string outboundIXNID=null);

        void NotifyIXNStatus(bool isConnected, int? proxyClientID = null);

        void NotifyContactServerState(bool isOpen = false);

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