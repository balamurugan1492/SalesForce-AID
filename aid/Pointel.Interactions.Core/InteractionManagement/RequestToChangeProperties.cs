using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Core.Common;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Pointel.Interactions.Core.Util;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Core.InteractionManagement
{
    class RequestToChangeProperties
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        #endregion

        #region RequestToChangeProperties
        /// <summary>
        /// Changes the properties.
        /// </summary>
        /// <param name="interactionId">The interaction identifier.</param>
        /// <param name="proxyId">The proxy identifier.</param>
        /// <param name="dispositionNotes">The disposition notes.</param>
        /// <returns></returns>
        public OutputValues ChangeProperties(string interactionId, int proxyId, KeyValueCollection addedProperties, KeyValueCollection changedProperties)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            RequestChangeProperties requestChangeProperties = RequestChangeProperties.Create();
            try
            {
                requestChangeProperties.InteractionId = interactionId;
                requestChangeProperties.ProxyClientId = proxyId;
                if (changedProperties != null)
                    requestChangeProperties.ChangedProperties = changedProperties;
                if (addedProperties != null)
                    requestChangeProperties.AddedProperties = addedProperties;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(requestChangeProperties);
                    if (response.Id == EventPropertiesChanged.MessageId || response.Id == EventAck.MessageId)
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
            catch (ProtocolException protocolException)
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
        public OutputValues UpdateProperties(string interactionId, int proxyClientID, KeyValueCollection keyvalues)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestChangeProperties updateProperties = RequestChangeProperties.Create();
                updateProperties.InteractionId = interactionId;
                updateProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    updateProperties.ChangedProperties = keyvalues;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(updateProperties);
                    if (response.Id == EventPropertiesChanged.MessageId || response.Id == EventAck.MessageId)
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
        public OutputValues AddCaseInformation(string interactionId, int proxyClientID, KeyValueCollection keyvalues)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestChangeProperties addedProperties = RequestChangeProperties.Create();
                addedProperties.InteractionId = interactionId;
                addedProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    addedProperties.AddedProperties = keyvalues;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(addedProperties);
                    if (response.Id == EventPropertiesChanged.MessageId || response.Id == EventAck.MessageId)
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

        public OutputValues DeletedProperties(string interactionId, int proxyClientID, KeyValueCollection keyvalues)
        {
            OutputValues output = OutputValues.GetInstance();
            output.Message = string.Empty;
            output.MessageCode = string.Empty;
            output.ErrorCode = 0;
            try
            {
                RequestChangeProperties deletedProperties = RequestChangeProperties.Create();
                deletedProperties.InteractionId = interactionId;
                deletedProperties.ProxyClientId = proxyClientID;
                if (keyvalues != null)
                    deletedProperties.DeletedProperties = keyvalues;
                if (Settings.InteractionProtocol != null && Settings.InteractionProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.InteractionProtocol.Request(deletedProperties);
                    if (response.Id == EventPropertiesChanged.MessageId || response.Id == EventAck.MessageId)
                    {
                        logger.Info("------------DeletedProperties-------------");
                        logger.Info("InteractionId  :" + interactionId);
                        logger.Info("ProxyClientId    :" + proxyClientID);
                        logger.Info(" DeletedProperties : response :" + response.ToString());
                        logger.Info("-----------------------------------------------");
                        logger.Trace(response.ToString());
                        output.MessageCode = "200";
                        output.Message = "Deleted Properties Successful";
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Don't Deleted Properties Successful";
                    }
                }
                else
                {
                    logger.Warn("Deleted Properties() : Interaction Server Protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Deleted Properties request " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }
        #endregion
    }
}
