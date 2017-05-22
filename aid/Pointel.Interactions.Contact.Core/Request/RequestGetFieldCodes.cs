using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.Contact.Core.Common;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Interactions.Contact.Core.Util;
using Genesyslab.Platform.Commons.Protocols;

namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetFieldCodes
    {
        public static OutputValues GetRenderFieldCodes(AgentProperties agentProperties, ContactProperties contactProperties, ContactInteractionProperties interactionProperties,
            string renderText)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            logger.Debug("RequestGetFieldCodes class GetRenderFieldCodes() : Entry");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestRenderFieldCodes renderFieldCodes = RequestRenderFieldCodes.Create();
                if (interactionProperties != null)
                {
                    renderFieldCodes.Interaction = interactionProperties;
                    logger.Info("Interaction Id : " + interactionProperties.Id);
                    logger.Info("From Address : " + interactionProperties.FromAddress);
                    logger.Info("To Address : " + interactionProperties.ToAddress);
                    logger.Info("Subject : " + interactionProperties.Subject);
                }
                else
                    logger.Warn("Interaction properties is null");
                if (agentProperties != null)
                {
                    renderFieldCodes.Agent = agentProperties;
                    logger.Info("Agent First Name : " + agentProperties.FirstName);
                    logger.Info("Agent Last Name : " + agentProperties.LastName);
                    logger.Info("Agent Full Name : " + agentProperties.FullName);
                }
                logger.Warn("Agent properties is null");
                if (contactProperties != null)
                {
                    renderFieldCodes.Contact = contactProperties;
                    logger.Info("Contact Id : " + contactProperties.Id);
                    logger.Info("Contact Title : " + contactProperties.Title);
                    logger.Info("Contact First Name : " + contactProperties.FirstName);
                    logger.Info("Contact Last Name : " + contactProperties.LastName);
                    logger.Info("Contact Full Name : " + contactProperties.FullName);
                    logger.Info("Contact Primary Phone Number : " + contactProperties.PrimaryPhoneNumber);
                    logger.Info("Contact Primary Email Address : " + contactProperties.PrimaryEmailAddress);
                }
                logger.Warn("Contact properties is null.");
                if (!string.IsNullOrEmpty(renderText))
                {
                    renderFieldCodes.Text = renderText;
                    logger.Info("Rendering Text : " + renderText);
                }
                logger.Warn("Rendering text is null or empty.");
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                      IMessage response = Settings.UCSProtocol.Request(renderFieldCodes);
                      if (response != null)
                      {
                          output.MessageCode = "200";
                          output.Message = "Rendering text Successful";
                          output.IContactMessage = response;
                      }
                      else
                      {
                          output.MessageCode = "2001";
                          output.Message = "Rendering text UnSuccessful";
                          output.IContactMessage = null;
                      }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetRenderFieldCodes() : Universal Contact Server protocol is Null or channel is closed");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error while getting data from standard response filed codes : " + (ex.InnerException == null ? ex.Message : ex.InnerException.ToString()));
            }
            logger.Debug("RequestGetFieldCodes class GetRenderFieldCodes() : Exit");
            return output;
        }
    }
}
