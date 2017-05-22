using System;
//using Genesyslab.Platform.Commons.Collections;
//using Genesyslab.Platform.Commons.Protocols;
//using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
//using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;

//using Pointel.Interactions.Chat.Core.General;
//using Pointel.Interactions.Chat.Core.Util;

namespace Pointel.Interactions.Chat.Core.ChatRequest
{
    internal class RequestToChangeProperties
    {
        #region RequestToChangeProperties
        /// <summary>
        /// Changes the properties.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <param name="dispositionNotes">The disposition notes.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues ChangeProperties(string interactionId, int proxyId, Genesyslab.Platform.Commons.Collections.KeyValueCollection addedProperties, Genesyslab.Platform.Commons.Collections.KeyValueCollection changedProperties)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties requestChangeProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
            try
            {
                requestChangeProperties.InteractionId = interactionId;
                requestChangeProperties.ProxyClientId = proxyId;
                if (changedProperties != null)
                    requestChangeProperties.ChangedProperties = changedProperties;
                if (addedProperties != null)
                    requestChangeProperties.AddedProperties = addedProperties;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage response = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(requestChangeProperties);
                    if (response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPropertiesChanged.MessageId || response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId)
                    {
                        logger.Info("------------RequestChangeProperties-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyId);
                        logger.Info(" RequestChangeProperties : response :" + response.ToString());
                        logger.Info("-------------------------------------------------");
                        logger.Trace(response.ToString());
                        output.MessageCode = "200";
                        output.Message = "Change Properties Successful";
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Don't Change Properties Successful";
                    }
                }
                else
                {
                    logger.Warn("ChangeProperties() : Interaction Server Protocol is Null..");
                }

            }
            catch (Genesyslab.Platform.Commons.Protocols.ProtocolException protocolException)
            {
                logger.Error("Error occurred while change properties request " + protocolException.ToString());
                output.MessageCode = "2001";
                output.Message = protocolException.Message;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while change properties request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }


        /// <summary>
        /// Updates the properties.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="keyvalues">The key values.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues UpdateProperties(string interactionId, int proxyClientID, Genesyslab.Platform.Commons.Collections.KeyValueCollection keyvalues)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties updateProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
                updateProperties.InteractionId = interactionId;
                updateProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    updateProperties.ChangedProperties = keyvalues;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage response = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(updateProperties);
                    if (response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPropertiesChanged.MessageId || response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId)
                    {
                        logger.Info("------------updateProperties-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyClientID);
                        logger.Info(" UpdateProperties : response :" + response.ToString());
                        logger.Info("-----------------------------------------------");
                        logger.Trace(response.ToString());
                        output.MessageCode = "200";
                        output.Message = "Update Properties Successful";
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Don't Update Properties Successful";
                    }
                }
                else
                {
                    logger.Warn("UpdateProperties() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while update properties request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Adds the case information.
        /// </summary>
        /// <param name="interactionid">The interactionid.</param>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues AddCaseInformation(string interactionId, int proxyClientID, Genesyslab.Platform.Commons.Collections.KeyValueCollection keyvalues)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties addedProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
                addedProperties.InteractionId = interactionId;
                addedProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    addedProperties.AddedProperties = keyvalues;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage response = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(addedProperties);
                    if (response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPropertiesChanged.MessageId || response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId)
                    {
                        logger.Info("------------AddCaseInformation-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyClientID);
                        logger.Info(" AddCaseInformation : response :" + response.ToString());
                        logger.Info("-----------------------------------------");
                        logger.Trace(response.ToString());
                        output.MessageCode = "200";
                        output.Message = "Add Case Information Successful";
                    }
                    else
                    {
                        logger.Trace(response.ToString());
                        output.MessageCode = "2001";
                        output.Message = "Add Properties UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AddCaseInformation() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while doing Add Case Information:" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        /// <summary>
        /// Deletes the case information.
        /// </summary>
        /// <param name="interactionid">The interactionid.</param>
        /// <param name="keyvalues">The keyvalues.</param>
        /// <returns></returns>
        public static Pointel.Interactions.Chat.Core.General.OutputValues DeleteCaseInformation(string interactionId, int proxyClientID, Genesyslab.Platform.Commons.Collections.KeyValueCollection keyvalues)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            Pointel.Interactions.Chat.Core.General.OutputValues output = Pointel.Interactions.Chat.Core.General.OutputValues.GetInstance();
            try
            {
                Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties deletedProperties = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement.RequestChangeProperties.Create();
                deletedProperties.InteractionId = interactionId;
                deletedProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    deletedProperties.DeletedProperties = keyvalues;
                if (Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol != null && Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    Genesyslab.Platform.Commons.Protocols.IMessage response = Pointel.Interactions.Chat.Core.Util.Settings.IxnServerProtocol.Request(deletedProperties);
                    if (response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventPropertiesChanged.MessageId || response.Id == Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventAck.MessageId)
                    {
                        logger.Info("------------DeleteCaseInformation-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyClientID);
                        logger.Info("KVC : " + keyvalues.ToString());
                        logger.Info("AddCaseInformation : response :" + response.ToString());
                        logger.Info("---------------------------------------------");
                        logger.Trace(response.ToString());
                        output.MessageCode = "200";
                        output.Message = "Delete Case Information Successful";
                    }
                    else
                    {
                        logger.Trace(response.ToString());
                        output.MessageCode = "2001";
                        output.Message = "Add Properties UnSuccessful";
                    }
                }
                else
                {
                    logger.Warn("AddCaseInformation() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while doing Add Case Information:" + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
