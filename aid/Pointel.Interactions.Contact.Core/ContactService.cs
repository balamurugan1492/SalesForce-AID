namespace Pointel.Interactions.Contact.Core
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Interactions.Contact.Core.Application;
    using Pointel.Interactions.Contact.Core.Common;
    using Pointel.Interactions.Contact.Core.ConnectionManager;
    using Pointel.Interactions.Contact.Core.Listener;
    using Pointel.Interactions.Contact.Core.Util;

    public class ContactService
    {
        #region Fields

        public static IContactService messageToClient = null;

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Connects the ucs.
        /// </summary>
        /// <param name="configObject">The configuration object.</param>
        /// <param name="contactAppName">Name of the application.</param>
        /// <returns></returns>
        public OutputValues ConnectUCS(ConfService configObject, NullableInt tenantDBID, string contactAppName, Func<bool, bool> contactServerNotification)
        {
            OutputValues output = null;
            ReadConfigObjects readConfigObjects = new ReadConfigObjects();
            try
            {
                Settings.comObject = configObject;
                Settings.tenantDBID = tenantDBID;
                readConfigObjects.ReadApplicationObject(contactAppName);
                ContactConnectionManager createProtocol = new ContactConnectionManager();
                createProtocol.ContactServerNotificationHandler += new NotifyContactServerState(contactServerNotification); 
                output = createProtocol.ConnectContactServer(Settings.PrimaryApplication, Settings.SecondaryApplication);
                if (output.MessageCode == "200") 
                {
                    messageToClient.NotifyContactProtocol(Settings.UCSProtocol);
                }
                else
                {
                    messageToClient.NotifyContactProtocol(Settings.UCSProtocol);
                }
            }
            catch (Exception commonException)
            {
                logger.Error("ContactLibrary:ConnectUCS:" + commonException.ToString());
            }
            return output;
        }

        /// <summary>
        /// Subscribers the specified client object.
        /// </summary>
        /// <param name="clientObject">The client object.</param>
        public void Subscriber(IContactService clientObject)
        {
            messageToClient = clientObject;
        }

        public void Subscriber(IContactDetailService clientObject)
        {
            ContactManager subscribe = ContactManager.GetInstance();

            subscribe.Subscribe(clientObject);
        }

        #endregion Methods

        #region Other

        //#region RequestAddAgentStdRespFavorite
        ///// <summary>
        ///// Documents the request add agent standard resp favorite. 
        ///// </summary>
        ///// <param name="stdResponse">The standard response.</param>
        ///// <param name="agentId">The agent unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestAddAgentStdRespFavorite(string stdResponse, string agentId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestAddStdRespFavorite requestAddStdRespFavorite = new RequestAddStdRespFavorite();
        //        output = requestAddStdRespFavorite.AddAgentFavoriteResponse(stdResponse, agentId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Add Agent Std Response Favorite " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestAddAgentStdRespFavorite
        //#region RequestGetAllAttributes
        ///// <summary>
        ///// Documents the request get all attributes.
        ///// </summary>
        ///// <param name="contactId">The contact unique identifier.</param>
        ///// <param name="attributeNames">The attribute names.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestGetAllAttributes(string contactId, List<string> attributeNames)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetAllAttributes requestGetAllAttributes = new RequestGetAllAttributes();
        //        output = RequestGetAllAttributes.GetAttributeValues(contactId, attributeNames);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get All Attributes " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestGetAllAttributes
        //#region RequestGetAllResponse
        ///// <summary>
        ///// Documents the request get all response.
        ///// </summary>
        ///// <param name="tenantID">The tenant unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestGetAllResponse(int tenantID)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetAllResponse requestGetAllResponse = new RequestGetAllResponse();
        //        output = requestGetAllResponse.GetResponseContent(tenantID);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get All Response " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestGetAllResponse
        //#region RequestGetContactInteractionList
        ///// <summary>
        ///// Documents the request get contact interaction list.
        ///// </summary>
        ///// <param name="contactId">The contact unique identifier.</param>
        ///// <param name="media">The media.</param>
        ///// <param name="tenantId">The tenant unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestGetContactInteractionList(string contactId, int tenantId, List<string> attributesNames)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetContactInteractionList requestGetContactInteractionList = new RequestGetContactInteractionList();
        //        output = requestGetContactInteractionList.GetContacts(contactId, tenantId, attributesNames);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Contact Interaction List" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestGetContactInteractionList
        //#region RequestGetRecentInteractionList
        ///// <summary>
        ///// Documents the request get recent interaction list.
        ///// </summary>
        ///// <param name="mediaType">Type of the media.</param>
        ///// <param name="contactID">The contact unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestGetRecentInteractionList(string mediaType, string contactID)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetRecentInteractionList requestGetRecentInteractionList = new RequestGetRecentInteractionList();
        //        output = requestGetRecentInteractionList.GetRecentInteractionList(mediaType, contactID);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Recent Interaction List" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestGetRecentInteractionList
        //#region RequestGetStdRespFavorite
        ///// <summary>
        ///// Documents the request get standard resp favorite.
        ///// </summary>
        ///// <param name="agentId">The agent unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestGetStdRespFavorite(NullableInt agentId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetStdRespFavorite requestGetStdRespFavorite = new RequestGetStdRespFavorite();
        //        output = requestGetStdRespFavorite.GetAgentFavoriteResponse(agentId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Std Response Favorite" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestGetStdRespFavorite
        //#region RequestInteractionCount
        ///// <summary>
        ///// Documents the request interaction count.
        ///// </summary>
        ///// <param name="mediaType">Type of the media.</param>
        ///// <param name="contactID">The contact unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestInteractionCount(string mediaType, string contactID)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestInteractionCount requestInteractionCount = new RequestInteractionCount();
        //        output = requestInteractionCount.GetInteractionCount(mediaType, contactID);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Interaction Count" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestInteractionCount
        //#region RequestRemoveAgentStdRespFavorite
        ///// <summary>
        ///// Documents the request remove agent standard resp favorite.
        ///// </summary>
        ///// <param name="stdResponse">The standard response.</param>
        ///// <param name="agentId">The agent unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestRemoveAgentStdRespFavorite(string stdResponse, string agentId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestRemoveAgentStdRespFavorite requestRemoveAgentStdRespFavorite = new RequestRemoveAgentStdRespFavorite();
        //        output = requestRemoveAgentStdRespFavorite.RemoveAgentFavoriteResponse(stdResponse, agentId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Remove Agent Std Response Favorite" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestRemoveAgentStdRespFavorite
        //#region RequestToDeleteInteraction
        ///// <summary>
        ///// Documents the request automatic delete interaction.
        ///// </summary>
        ///// <param name="interactionID">The interaction unique identifier.</param>
        ///// <returns></returns>
        ////public OutputValues DoRequestToDeleteInteraction(string interactionID)
        ////{
        ////    OutputValues output = OutputValues.GetInstance();
        ////    try
        ////    {
        ////        RequestToDeleteInteraction requestToDeleteInteraction = new RequestToDeleteInteraction();
        ////        output = requestToDeleteInteraction.DeleteInteractionFromUCS(interactionID);
        ////    }
        ////    catch (Exception generalException)
        ////    {
        ////        logger.Error("Error occurred while Do Delete Interaction From UCS" + generalException.ToString());
        ////    }
        ////    return output;
        ////}
        //#endregion RequestToDeleteInteraction
        //#region RequestToGetContacts
        ///// <summary>
        ///// Documents the request automatic get contacts.
        ///// </summary>
        ///// <param name="tenantId">The tenant unique identifier.</param>
        ///// <param name="attributeNames">The attribute names.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToGetContacts(int tenantId, List<string> attributeNames)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToGetContacts requestToGetContacts = new RequestToGetContacts();
        //        output = requestToGetContacts.GetContactList(tenantId, attributeNames);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Contacts" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToGetContacts
        //#region RequestToGetDocument
        ///// <summary>
        ///// Documents the request automatic get document.
        ///// </summary>
        ///// <param name="documentId">The document unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToGetDocument(string documentId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToGetDocument requestToGetDocument = new RequestToGetDocument();
        //        output = requestToGetDocument.GetAttachDocument(documentId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Document" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToGetDocument
        //#region RequestToGetInteractionContent
        ///// <summary>
        ///// Documents the content of the request automatic get interaction.
        ///// </summary>
        ///// <param name="interactionID">The interaction unique identifier.</param>
        ///// <returns></returns>
        //public IMessage DoRequestToGetInteractionContent(string interactionID, bool isAttachments)
        //{
        //    IMessage response = null;
        //    try
        //    {
        //        RequestToGetInteractionContent requestToGetInteractionContent = new RequestToGetInteractionContent();
        //        response = requestToGetInteractionContent.GetInteractionContent(interactionID, isAttachments);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Interaction Content" + generalException.ToString());
        //    }
        //    return response;
        //}
        //#endregion RequestToGetInteractionContent
        //#region RequestToGetInteractionsWithStatus
        ///// <summary>
        ///// Documents the request automatic get interactions with status.
        ///// </summary>
        ///// <param name="mediaType">Type of the media.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToGetInteractionsWithStatus(string mediaType)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToGetInteractionsWithStatus requestToGetInteractionsWithStatus = new RequestToGetInteractionsWithStatus();
        //        output = requestToGetInteractionsWithStatus.GetInteractionsCount(mediaType);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Interaction With Status" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToGetInteractionsWithStatus
        //#region RequestToGetStandardResponse
        ///// <summary>
        ///// Documents the request automatic get standard response.
        ///// </summary>
        ///// <param name="standardresponseId">The standard response unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToGetStandardResponse(string standardresponseId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToGetStandardResponse requestToGetStandardResponse = new RequestToGetStandardResponse();
        //        output = requestToGetStandardResponse.GetStandardResponse(standardresponseId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Get Standard Response" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToGetStandardResponse
        //#region RequestToIdentifyContact
        ///// <summary>
        ///// Documents the request automatic identify contact.
        ///// </summary>
        ///// <param name="mediaType">Type of the media.</param>
        ///// <param name="tenantId">The tenant unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToIdentifyContact(string mediaType, int tenantId, KeyValueCollection userData)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToIdentifyContact requestToIdentifyContact = new RequestToIdentifyContact();
        //        output = requestToIdentifyContact.IdentifyContact(mediaType, tenantId, userData);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Identify Contact" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToIdentifyContact
        //#region RequestToInsertInteraction
        ///// <summary>
        ///// Documents the request automatic insert interaction.
        ///// </summary>
        ///// <returns></returns>
        //public IMessage RequestToInsertInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        //{
        //    IMessage imessage = null;
        //    try
        //    {
        //        RequestToInsertInteraction requestToInsertInteraction = new RequestToInsertInteraction();
        //        imessage = requestToInsertInteraction.RequestInsertInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Insert Interaction" + generalException.ToString());
        //    }
        //    return imessage;
        //}
        //#endregion
        //#region RequestToRemoveDocument
        ///// <summary>
        ///// Documents the request automatic remove document.
        ///// </summary>
        ///// <param name="interactionId">The interaction unique identifier.</param>
        ///// <param name="documentId">The document unique identifier.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToRemoveDocument(string interactionId, string documentId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToRemoveDocument requestToRemoveDocument = new RequestToRemoveDocument();
        //        output = requestToRemoveDocument.RemoveAttachDocument(interactionId, documentId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Remove Document" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToRemoveDocument
        //#region RequestToUpdateInteraction
        ///// <summary>
        ///// Documents the request automatic update interaction.
        ///// </summary>
        ///// <param name="interactionID">The interaction unique identifier.</param>
        ///// <param name="comment">The comment.</param>
        ///// <param name="userData">The user data.</param>
        ///// <returns></returns>
        //public OutputValues DoRequestToUpdateInteraction(string interactionID, int ownerId, string comment, KeyValueCollection userData,int status)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToUpdateInteraction requestToUpdateInteraction = new RequestToUpdateInteraction();
        //        output = requestToUpdateInteraction.UpdateInteraction(interactionID, ownerId, comment, userData,status);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Update Interaction" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestToUpdateInteraction
        //#region RequestToUpdateInteraction
        ///// <summary>
        ///// Documents the request automatic insert interaction.
        ///// </summary>
        ///// <returns></returns>
        ////public IMessage RequestToUpdateInteraction(InteractionContent interactionContent,InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        ////{
        ////    IMessage imessage = null;
        ////    try
        ////    {
        ////        RequestToUpdateInteraction requestToUpdateInteraction = new RequestToUpdateInteraction();
        ////        imessage = requestToUpdateInteraction.RequestUpdateInteraction(interactionContent,interactionAttributes, baseEntityAttributes);
        ////    }
        ////    catch (Exception generalException)
        ////    {
        ////        logger.Error("Error occurred while updating interaction as : " + generalException.ToString());
        ////    }
        ////    return imessage;
        ////}
        //#endregion
        //#region RequestUpdateAttribute
        //public OutputValues DoRequestUpdateAttribute(string contactId, int tenantId, AttributesList insertedAttributes, AttributesList updatedAttributes, DeleteAttributesList deletedAttributes)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestUpdateAttribute requestUpdateAttribute = new RequestUpdateAttribute();
        //        //  requestUpdateAttribute.RequestDeleteAttribute(tenantId, contactId, deletedAttributes[0].AttrName, deletedAttributes[0].AttrId[0].ToString());
        //        requestUpdateAttribute.RequestUpdateAllAttribute(contactId, tenantId, insertedAttributes, updatedAttributes, deletedAttributes);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Do Update Attribute" + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestUpdateAttribute
        //#region RequestUpdateAttribute
        //public OutputValues DoDeleteContact(string contactId)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToDeleteContact requestDeleteContact = new RequestToDeleteContact();
        //        requestDeleteContact.RequestDeleteContact(contactId);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Delete Contact " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestUpdateAttribute
        //#region RequestUpdateAttribute
        //public OutputValues DoInsertContact(int tenantId, AttributesList attribute)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestToInsertContact requestDeleteContact = new RequestToInsertContact();
        //        requestDeleteContact.RequestInsertContact(tenantId, attribute);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Delete Contact " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestUpdateAttribute
        //#region RequestInteractionListGet
        //public OutputValues RequestInteractionListGet(string ownerID, int tenantId, List<string> attributesNames)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        RequestGetInteractionList requestGetInteractionList = new RequestGetInteractionList();
        //        output = requestGetInteractionList.GetInteractionList(ownerID, tenantId, attributesNames);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Request to get Interaction List for Agent : " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion RequestUpdateAttribute
        //#region RequestGetRecentInteractionList
        //public OutputValues RequestRecentInteractionListGet(string mediaType, string contactID, int tenantId, List<string> attributesNames)
        //{
        //    OutputValues output = OutputValues.GetInstance();
        //    try
        //    {
        //        output =Pointel.Interactions.Contact.Core.Request.RequestGetInteractionList.GetRecentInteractionList(mediaType, contactID, tenantId, attributesNames);
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("Error occurred while Request to get Interaction List for Agent : " + generalException.ToString());
        //    }
        //    return output;
        //}
        //#endregion

        #endregion Other
    }
}