using System.Collections;
using System.Collections.Generic;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.Commons.Protocols;
using System;

namespace Pointel.Interactions.IPlugins
{
    public interface IWorkbinPlugin
    {
        /// <summary>
        /// Shows the work bin form.
        /// </summary>
        /// <returns></returns>
        object ShowWorkBinForm();

        /// <summary>
        /// Workbins the flag.
        /// </summary>
        /// <param name="isEnableWorkbin">if set to <c>true</c> [is enable workbin].</param>
        void WorkbinFlag(bool isEnableWorkbin);

        /// <summary>
        /// Shows the Contact directory address.
        /// </summary>
        /// <param name="to">The automatic.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        void ShowContactDirectoryAddress(string to, string cc, string bcc);

        /// <summary>
        /// Initializes the work bin.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="agentId">The agent unique identifier.</param>
        /// <param name="placeId">The place unique identifier.</param>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <param name="Object">The object.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="windowPostioning">The window postioning.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="mediaPlugins">The media plugins.</param>
        void InitializeWorkBin(string placeId, int proxyId, IPluginCallBack listener);
       
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="getresponse">The getresponse.</param>
        //void GetResponse(string Name, string getresponse);

        /// <summary>
        /// Notifies the interaction protocol.
        /// </summary>
        /// <param name="ixnProtocol">The ixn protocol.</param>
        void NotifyInteractionProtocol(InteractionServerProtocol ixnProtocol);

        /// <summary>
        /// Notifies the contact protocol.
        /// </summary>
        /// <param name="ucsProtocol">The ucs protocol.</param>
        void NotifyContactProtocol(UniversalContactServerProtocol ucsProtocol);

        /// <summary>
        /// Intimates the selected agent and consult type
        /// </summary>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="searchedType">Type of the searched.</param>
        /// <param name="searchedValue">The searched value.</param>
        //void DialEvent(Pointel.Interactions.IPlugins.OperationTypes operationType, string searchedType, string searchedValue, string place);

        void NotifyWorkbinContentChanged(IMessage message);

        List<string> GetPersonalWorkbinList();

        List<string> GetTeamAgentList();

        List<string> GetTeamWorkbinList();

        bool NotifyEmailOpen(string interactionId);
        
        void NotifyAgentLogin(bool isLogedin, int? proxyClientID = null);

        void NotifyContactServerState(bool isOpen = false);

        void UpdateSelectedWorkbin();

        void NotifyEmailClose(string interactionId);
       

    }
}
