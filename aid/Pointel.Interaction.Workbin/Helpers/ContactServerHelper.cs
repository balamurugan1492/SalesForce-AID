/*
 *======================================================
 * Pointel.Interaction.Workbin.Helpers.ContactServerHelper
 *======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 *======================================================
 */

using System.Collections.Generic;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Interactions.IPlugins;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class ContactServerHelper
    {
        public static EventGetInteractionContent RequestInteractionContent(string interactionId, bool isNeedAttachment)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetInteractionContent(interactionId, isNeedAttachment);
                if (result!=null &&  result.Id == EventGetInteractionContent.MessageId)
                    return result as EventGetInteractionContent;
            }

            return null;
        }

        public static EventGetAttributes RequestGetContactAttribute(string contactId, List<string> retrievelist)
        {
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
            {
                Genesyslab.Platform.Commons.Protocols.IMessage result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetAllAttributes(contactId, retrievelist);
                if (result != null && result.Id == EventGetAttributes.MessageId)
                    return result as EventGetAttributes;
            }
            return null;
        }

        public static Genesyslab.Platform.Commons.Protocols.IMessage UpdateInteraction(string interactionID, int owerId, string comment, KeyValueCollection userData, int status, string dtEndDate = null)
        {
            Genesyslab.Platform.Commons.Protocols.IMessage result = null;
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                result = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).UpdateInteraction(interactionID, owerId, comment, userData, status, dtEndDate);
            return result;

        }

    }
}
