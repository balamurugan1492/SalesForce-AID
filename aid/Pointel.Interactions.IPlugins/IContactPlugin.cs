using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using System.Collections;

namespace Pointel.Interactions.IPlugins
{
    public interface IContactPlugin
    {
        /// <summary>
        /// Shows the response.
        /// </summary>
        /// <param name="response">The response.</param>
        UserControl GetResponseUserControl(bool isWebHostEnable, Func<string, string, string, string, AttachmentList, string> refFunction);

        /// <summary>
        /// Shows the contact directory.
        /// </summary>
        /// <param name="contactDirectory">The contact directory.</param>
        /// <returns></returns>
        UserControl GetContactDirectoryUserControl(bool enableOkCancelButton, string interactionId, Func<string, string, string, bool> notifySelectedEmailId
            , string toAddress = null, string ccAddress = null, string bccAddress = null);

        /// <summary>
        /// Shows the Contact directory address.
        /// </summary>
        /// <param name="to">The automatic.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        void ShowContactDirectoryAddress(string to, string cc, string bcc);

        /// <summary>
        /// Gets the contact unique identifier.
        /// </summary>
        /// <param name="tenantId">The tenant unique identifier.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        Dictionary<ContactDetails, string> GetContactId(string tenantId, MediaTypes mediaType, KeyValueCollection userData);

        /// <summary>
        /// Shows the contact.
        /// </summary>
        /// <param name="contacts">The contacts.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="interactionId">The interaction unique identifier.</param>
        UserControl GetContactUserControl(string contactId, MediaTypes mediaType, bool isEnableControl = true);

        /// <summary>
        /// Updates the contact user control with new values
        /// </summary>
        /// <param name="contactUsercontrol">The contact user control</param>
        /// <param name="contacts">The contacts.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="interactionId">The interaction unique identifier.</param>
        void UpdateContactUserControl(System.Windows.Controls.UserControl contactUsercontrol, string contactId, MediaTypes mediaType);

        /// <summary>
        /// Gets the contact user control.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <param name="interactionId">The interaction id.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="notifyContactUpdated">The notify contact updated.</param>
        /// <returns></returns>
        System.Windows.Controls.UserControl GetContactUserControl(string contactId, string interactionId, MediaTypes mediaType, Func<string> notifyContactUpdated);

        /// <summary>
        /// Initializes the ucs.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="placeId">The place unique identifier.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="Object">The object.</param>
        /// <param name="listener">The listener.</param>
        void InitializeContact(string username, string agetnId, string placeId, int tenantDbId, string applicationName, ConfService comObject, IPluginCallBack listener, int ixnProxyId);

        void SubscribeUpdateNotification(Func<string, string, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList, string> eventUpdateNotification);

        void UnSubscribeUpdateNotification(Func<string, string, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList, string> eventUpdateNotification);

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="getresponse">The get response.</param>
        void GetResponse(string Name, string getresponse);

        /// <summary>
        /// Gets the contact history user control.
        /// </summary>
        /// <param name="OwnerIDorPersonDBID">The owner attribute dor person dbid.</param>
        /// <returns></returns>
        UserControl GetContactHistoryUserControl(string OwnerIDorPersonDBID);

        /// <summary>
        /// The method read the response from genesys the send configure signature content.
        /// </summary>
        /// <returns></returns>
        EmailSignature GetEmailSignature(string responsePath, ref bool isHTML);

        void NotifyWorkbinContentChanged(string interactionId, bool isPlacedIn);

        void NotifyInteractionServerState(bool isOpen);

        void NotifyTServerState(bool isOpen);

        void NotifyAgentLogin(bool isLogedin, int? proxyId = null);

        void RefreshContactHistory(string interactionId, bool isDelete);

        /// <summary>
        /// Notifies the voice media status.
        /// </summary>
        /// <param name="isVoiceEnabled">if set to <c>true</c> [is voice enabled].</param>
        void NotifyVoiceMediaStatus(bool isVoiceEnabled);

        void NotifyEmailMediaState(bool state);

        #region Add by smoorthy 17-11-2014 for chat

        /// <summary>
        /// Documents the content of the get interaction.
        /// </summary>
        /// <param name="universalContactServerProtocol">The universal contact server protocol.</param>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <returns></returns>
        IMessage GetInteractionContent(string interactionID, bool isAttachments);

        /// <summary>
        /// Documents the get all attributes.
        /// </summary>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="attributeNames">The attribute names.</param>
        /// <returns></returns>
        IMessage GetAllAttributes(string contactId, List<string> attributeNames);

        /// <summary>
        /// Documents the get interaction list.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <returns></returns>
        IMessage GetInteractionList(string contactID, int tenantId, string interactionId, List<string> attributesNames);

        /// <summary>
        /// Gets the interaction list.
        /// </summary>
        /// <param name="ownerID">The owner identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="pagemaxSize">Size of the pagemax.</param>
        /// <param name="searchCriteriaCollection">The search criteria collection.</param>
        /// <param name="attributesNames">The attributes names.</param>
        /// <returns></returns>
        IMessage GetInteractionList(int tenantId, int pagemaxSize, EntityTypes entitypes, SearchCriteriaCollection searchCriteriaCollection, List<string> attributesNames);

        /// <summary>
        /// Documents the get count interactions.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <returns></returns>
        IMessage GetTotalInteractionCount(string contactId, string interactionId);


        /// <summary>
        /// Gets the media vice interaction count.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="contactId">The contact unique identifier.</param>
        /// <param name="interactionId">The interaction unique identifier.</param>
        /// <returns></returns>
        IMessage GetMediaViceInteractionCount(string mediaType, string contactId, string interactionId);

        /// <summary>
        /// Documents the update interaction.
        /// </summary>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        IMessage UpdateInteraction(string interactionID, int owerId, string comment, KeyValueCollection userData, int status, string dtEndDate = null);


        /// <summary>
        /// Documents the insert interaction.
        /// </summary>
        /// <param name="interactionID">The interaction unique identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        IMessage RequestToInsertInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes);


        /// <summary>
        /// Requests the automatic update.
        /// </summary>
        /// <param name="interactionAttributes">The interaction attributes.</param>
        /// <param name="baseEntityAttributes">The base entity attributes.</param>
        /// <returns></returns>
        IMessage RequestToUpdate(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes);

        IMessage GetDocument(string documentId);

        IMessage AddAttachmentDocument(string interactionId, string filePath, bool isDocID = false);

        IMessage DeleteInteraction(string interactionId);

        IMessage RemoveAttachDocument(string interactionId, string documentId);

        IMessage GetContactsforForwardMail(string searchText, int maxresults,out bool isIndex, string[] contactIDs = null);
        #endregion

        string GetResponseFieldContents(AgentProperties agentProperties, ContactProperties contactProperties, ContactInteractionProperties interactionProperties,
            string renderText);

    }

    public class EmailSignature
    {
        public string Subject
        {
            get;
            set;
        }

        public string EmailBody
        {
            get;
            set;
        }

        public AttachmentList AttachmentList
        {
            get;
            set;
        }
    }
}
