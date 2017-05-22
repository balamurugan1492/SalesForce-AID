/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar and Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Interactions.Email.DataContext;
using Pointel.Interactions.IPlugins;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;

namespace Pointel.Interactions.Email.Helper
{
    /// <summary>
    /// Class ContactServerHelper.
    /// </summary>
    public class ContactServerHelper
    {
        /// <summary>
        /// Requests the content of the interaction.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="isNeedAttachment">if set to <c>true</c> [is need attachment].</param>
        /// <returns>EventGetInteractionContent.</returns>
        public static EventGetInteractionContent RequestInteractionContent(string interactionId, bool isNeedAttachment)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionContent(interactionId, false);
                if (result.Id == EventGetInteractionContent.MessageId)
                {
                    return result as EventGetInteractionContent;

                }
            }

            return null;
        }

        /// <summary>
        /// Requests the get contact attribute.
        /// </summary>
        /// <param name="contactId">The contact identifier.</param>
        /// <param name="retrievelist">The retrievelist.</param>
        /// <returns>EventGetAttributes.</returns>
        public static EventGetAttributes RequestGetContactAttribute(string contactId, List<string> retrievelist)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetAllAttributes(contactId, retrievelist);
                if (result.Id == EventGetAttributes.MessageId)
                {
                    return result as EventGetAttributes;

                }
            }
            return null;
        }

        /// <summary>
        /// Updates the interaction.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        /// <param name="owerId">The ower identifier.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="status">The status.</param>
        /// <param name="dtEndDate">The dt end date.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage UpdateInteraction(string interactionID, int owerId, string comment, KeyValueCollection userData, int status, string dtEndDate = null)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                var result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(interactionID, owerId, comment, userData, status, dtEndDate);
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Inserts the interaction.
        /// </summary>
        /// <param name="interactionContent">Content of the interaction.</param>
        /// <param name="interactionAttributes">The interaction attributes.</param>
        /// <param name="baseEntityAttributes">The base entity attributes.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage InsertInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                var result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).RequestToInsertInteraction(interactionContent, interactionAttributes, baseEntityAttributes);
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Updates the interaction.
        /// </summary>
        /// <param name="interactionContent">Content of the interaction.</param>
        /// <param name="interactionAttributes">The interaction attributes.</param>
        /// <param name="baseEntityAttributes">The base entity attributes.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage UpdateInteraction(InteractionContent interactionContent, InteractionAttributes interactionAttributes, BaseEntityAttributes baseEntityAttributes)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                var result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).RequestToUpdate(interactionContent, interactionAttributes, baseEntityAttributes);
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Adds the attach document.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage AddAttachDocument(string interactionId, string filePath, bool isDocID = false)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                Genesyslab.Platform.Commons.Protocols.IMessage result = null;
                if (!isDocID)
                    result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).AddAttachmentDocument(interactionId, filePath);
                else
                    result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).AddAttachmentDocument(interactionId, filePath, true);
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Deletes the interaction.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage DeleteInteraction(string interactionId)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                var result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).DeleteInteraction(interactionId);
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Removes the attach document.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>Genesyslab.Platform.Commons.Protocols.IMessage.</returns>
        public static Genesyslab.Platform.Commons.Protocols.IMessage RemoveAttachDocument(string interactionId, string documentId)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                var result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).RemoveAttachDocument(interactionId, documentId);
                    return result;
            }
            return null;
        }

    }
}
