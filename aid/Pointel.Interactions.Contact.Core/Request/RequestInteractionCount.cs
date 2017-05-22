using System;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;

namespace Pointel.Interactions.Contact.Core.Request
{
    /// <summary>
    /// This request returns the number of Interactions that match the specified filter. 
    /// </summary>
    public class RequestInteractionCount
    {
        #region RequestInteractionCount
        /// <summary>
        /// Gets the interaction count.
        /// </summary>
        /// <param name="universalContactServerProtocol">The universal contact server protocol.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="contactID">The contact unique identifier.</param>
        /// <returns></returns>
        public static OutputValues GetTotalInteractionCount(string contactID, string interactionId)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestCountInteractions requestCountInteractions = RequestCountInteractions.Create();
                requestCountInteractions.TenantId = Settings.tenantDBID;

                SimpleSearchCriteria sSC1 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.MediaTypeId, AttrValue = "voice", Operator = new NullableOperators(Operators.NotEqual) };
                SimpleSearchCriteria sSC2 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Status, AttrValue = ((int)Statuses.InProcess).ToString(), Operator = new NullableOperators(Operators.Equal) };
                SimpleSearchCriteria sSC3 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Id, AttrValue = null, Operator = new NullableOperators(Operators.NotEqual) };
                SimpleSearchCriteria sSC4 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.ContactId, AttrValue = contactID, Operator = new NullableOperators(Operators.Equal) };

                ComplexSearchCriteria cmpSC = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC.Criterias = new SearchCriteriaCollection();
                cmpSC.Criterias.Add(sSC1);

                ComplexSearchCriteria cmpSC2 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC2.Criterias = new SearchCriteriaCollection();
                cmpSC2.Criterias.Add(sSC2);

                ComplexSearchCriteria cmpSC3 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC3.Criterias = new SearchCriteriaCollection();
                cmpSC3.Criterias.Add(sSC3);

                ComplexSearchCriteria cmpSC4 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC4.Criterias = new SearchCriteriaCollection();
                cmpSC4.Criterias.Add(sSC4);

                SearchCriteriaCollection srchCrit = new SearchCriteriaCollection();
                srchCrit.Add(cmpSC);
                srchCrit.Add(cmpSC2);
                srchCrit.Add(cmpSC3);
                srchCrit.Add(cmpSC4);

                requestCountInteractions.SearchCriteria = new SearchCriteriaCollection();
                requestCountInteractions.SearchCriteria = srchCrit;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestCountInteractions-------------");
                    logger.Info("Interaction ID :" + interactionId);
                    logger.Info("Contact ID :" + contactID);
                    logger.Info("-------------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestCountInteractions);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Received Total InProcess Interaction count Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Received Total InProcess Interaction count Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetInteractionCount() : Universal Contact Server Protocol is Null");
                }
            }
            catch (Exception error)
            {
                logger.Error("GetInteractionCount(): Error occurred while retrieving total in-process interaction count :" + error.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = error.Message;
            }
            return output;
        }

        public static OutputValues GetMediaViceInteractionCount(string mediaType, string contactID, string interactionId)  
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestCountInteractions requestCountInteractions = RequestCountInteractions.Create();
                requestCountInteractions.TenantId = Settings.tenantDBID;

                SimpleSearchCriteria sSC1 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.MediaTypeId, AttrValue = mediaType.ToLower(), Operator = new NullableOperators(Operators.Equal) };
                SimpleSearchCriteria sSC2 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Status, AttrValue = ((int)Statuses.InProcess).ToString(), Operator = new NullableOperators(Operators.Equal) };
                SimpleSearchCriteria sSC3 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Id, AttrValue = null, Operator = new NullableOperators(Operators.NotEqual) };
                SimpleSearchCriteria sSC4 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.ContactId, AttrValue = contactID, Operator = new NullableOperators(Operators.Equal) };

                ComplexSearchCriteria cmpSC = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC.Criterias = new SearchCriteriaCollection();
                cmpSC.Criterias.Add(sSC1);

                ComplexSearchCriteria cmpSC2 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC2.Criterias = new SearchCriteriaCollection();
                cmpSC2.Criterias.Add(sSC2);

                ComplexSearchCriteria cmpSC3 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC3.Criterias = new SearchCriteriaCollection();
                cmpSC3.Criterias.Add(sSC3);

                ComplexSearchCriteria cmpSC4 = new ComplexSearchCriteria() { Prefix = Prefixes.And };
                cmpSC4.Criterias = new SearchCriteriaCollection();
                cmpSC4.Criterias.Add(sSC4);

                SearchCriteriaCollection srchCrit = new SearchCriteriaCollection();
                srchCrit.Add(cmpSC);
                srchCrit.Add(cmpSC2);
                //srchCrit.Add(cmpSC3);
                srchCrit.Add(cmpSC4);

                requestCountInteractions.SearchCriteria = new SearchCriteriaCollection();
                requestCountInteractions.SearchCriteria = srchCrit;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestCountInteractions (GetMediaViceInteractionCount)-------------");
                    logger.Info("Interaction ID :" + interactionId);
                    logger.Info("Contact ID :" + contactID);
                    logger.Info("Media Type Id : " + mediaType);
                    logger.Info("--------------------------------------------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestCountInteractions);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Received Media Vice InProcess Interaction count Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Don't Received Media Vice InProcess Interaction count Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetMediaViceInteractionCount() : Universal Contact Server Protocol is Null");
                }
            }
            catch (Exception error)
            {
                logger.Error("GetMediaViceInteractionCount(): Error occurred while retrieving total in-process interaction count :" + error.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = error.Message;
            }
            return output;
        }

        #endregion RequestInteractionCount
    }
}
