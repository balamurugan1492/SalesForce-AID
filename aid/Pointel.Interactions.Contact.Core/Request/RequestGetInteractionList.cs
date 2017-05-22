
using System;
using System.Collections.Generic;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Contacts.Protocols;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.Contact.Core.Util;
namespace Pointel.Interactions.Contact.Core.Request
{
    public class RequestGetInteractionList
    {
        #region RequestGetInteractionList
        public static OutputValues GetInteractionList(string ownerID, int tenantId, List<string> attributesNames)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = new OutputValues();
            try
            {
                RequestInteractionListGet requestGetInteractionList = RequestInteractionListGet.Create();
                //Inputs
                //DataSource
                requestGetInteractionList.DataSource = new NullableDataSourceType(DataSourceType.Main);

                requestGetInteractionList.SortCriteria = new SortCriteriaCollection();
                SortCriteriaCollection sortCC = new SortCriteriaCollection();
                SortCriteria sortc = new SortCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, SortOperator = new NullableSortMode(SortMode.Descending), SortIndex = 0 };
                sortCC.Add(sortc);
                requestGetInteractionList.SortCriteria = sortCC;
                SimpleSearchCriteria sSC3 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.OwnerId, AttrValue = ownerID, Operator = new NullableOperators(Operators.Equal) };
                
               
                
                //SimpleSearchCriteria sSC4 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.MediaTypeId, AttrValue = "voice", Operator = new NullableOperators(Operators.Equal) };

                SearchCriteriaCollection srchCrit = new SearchCriteriaCollection();
                srchCrit.Add(sSC3);
                requestGetInteractionList.SearchCriteria = srchCrit;
                StringList stringList = new StringList();
                if (attributesNames.Count > 0)
                    foreach (string attribute in attributesNames)
                        stringList.Add(attribute);
                stringList.Add(InteractionSearchCriteriaConstants.MediaTypeId);
                stringList.Add(InteractionSearchCriteriaConstants.Id);
                stringList.Add(InteractionSearchCriteriaConstants.SubtypeId);
                requestGetInteractionList.AttributeList = stringList;
                requestGetInteractionList.TenantId = tenantId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestGetInteractionList-------------");
                    logger.Info("OwnerID  :" + ownerID);
                    logger.Info("TenantId    :" + tenantId);
                    logger.Info("--------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestGetInteractionList);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Getting Interactions For Agent Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Getting Interactions For Agent UnSuccessful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetInteractionList() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Interaction for Agent request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        public static OutputValues GetInteractionList(SearchCriteriaCollection searchCriteriaCollection, int tenantId, int pagemaxSize, List<string> attributesNames, DataSourceType dataSourceType = DataSourceType.Main)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = new  OutputValues();
            try
            {
                RequestInteractionListGet requestGetInteractionList = RequestInteractionListGet.Create();
                requestGetInteractionList.DataSource = new NullableDataSourceType(dataSourceType);
                requestGetInteractionList.SortCriteria = new SortCriteriaCollection();
                requestGetInteractionList.SortCriteria = new SortCriteriaCollection();
                SortCriteriaCollection sortCC = new SortCriteriaCollection();
                SortCriteria sortc = new SortCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, SortOperator = new NullableSortMode(SortMode.Descending), SortIndex = 0 };
                sortCC.Add(sortc);
                requestGetInteractionList.SortCriteria = sortCC;
                requestGetInteractionList.SearchCriteria = searchCriteriaCollection;
                StringList stringList = new StringList();
                if (attributesNames.Count > 0)
                    foreach (string attribute in attributesNames)
                        stringList.Add(attribute);
                stringList.Add(InteractionSearchCriteriaConstants.MediaTypeId);
                stringList.Add(InteractionSearchCriteriaConstants.Id);
                stringList.Add(InteractionSearchCriteriaConstants.SubtypeId);
                requestGetInteractionList.AttributeList = stringList;
                requestGetInteractionList.TenantId = tenantId;
                requestGetInteractionList.PageMaxSize = pagemaxSize;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    IMessage message = Settings.UCSProtocol.Request(requestGetInteractionList);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Getting Interactions For Agent Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Getting Interactions For Agent UnSuccessful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetInteractionList() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Interaction for Agent request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        public static OutputValues GetInteractionList(int tenantId, int pagemaxSize, EntityTypes entitypes, SearchCriteriaCollection searchCriteriaCollection, List<string> attributesNames)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = new OutputValues();
            try
            {
                RequestInteractionListGet requestGetInteractionList = RequestInteractionListGet.Create();
                requestGetInteractionList.DataSource = new NullableDataSourceType(DataSourceType.Main);
                requestGetInteractionList.TenantId = new Genesyslab.Platform.Commons.Collections.NullableInt(tenantId);
                requestGetInteractionList.PageMaxSize = new Genesyslab.Platform.Commons.Collections.NullableInt(pagemaxSize);
                requestGetInteractionList.SearchCriteria = searchCriteriaCollection;
                StringList stringList = new StringList();
                if (attributesNames.Count > 0)
                    foreach (string attribute in attributesNames)
                        stringList.Add(attribute);
                requestGetInteractionList.AttributeList = stringList;
                requestGetInteractionList.EntityTypeId = new NullableEntityTypes(entitypes);


                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestGetInteractionList-------------");
                    logger.Info("TenantId    :" + tenantId);
                    logger.Info("--------------------------------------------------");
                    IMessage message = Settings.UCSProtocol.Request(requestGetInteractionList);
                    if (message != null)
                    {
                        logger.Trace(message.ToString());
                        output.IContactMessage = message;
                        output.MessageCode = "200";
                        output.Message = "Getting Interactions For Agent Successful";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "2001";
                        output.Message = "Getting Interactions For Agent UnSuccessful";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetInteractionList() : Universal Contact Server protocol is Null..");
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error Occurred while Get Interaction for Agent request" + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        #endregion

        #region RequestGetRecentInteractionList
        /// <summary>
        /// Gets the recent interaction list.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="contactID">The contact unique identifier.</param>
        /// <param name="tenantId">The tenant unique identifier.</param>
        /// <param name="attributesNames">The attributes names.</param>
        /// <returns></returns>
        public static OutputValues GetRecentInteractionList(string contactID, int tenantId, string interactionID, List<string> attributesNames)
        {
            Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
            OutputValues output = OutputValues.GetInstance();
            try
            {
                RequestInteractionListGet requestInteractionListGet = RequestInteractionListGet.Create();
                requestInteractionListGet.DataSource = new NullableDataSourceType(DataSourceType.Main);
                requestInteractionListGet.SortCriteria = new SortCriteriaCollection();
                SortCriteriaCollection sortCC = new SortCriteriaCollection();
                SortCriteria sortc = new SortCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, SortOperator = new NullableSortMode(SortMode.Descending), SortIndex = 0 };
                sortCC.Add(sortc);
                requestInteractionListGet.SortCriteria = sortCC;


                SimpleSearchCriteria sSC1 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.MediaTypeId, AttrValue = "voice", Operator = new NullableOperators(Operators.NotEqual) };
                SimpleSearchCriteria sSC2 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, AttrValue = DateTime.Now.ToString("yyyy") + "-" + DateTime.Now.Month.ToString() + "-" + (DateTime.Now.Day).ToString() + "T00:00:00.000Z", Operator = new NullableOperators(Operators.GreaterOrEqual) };
                SimpleSearchCriteria sSC3 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Id, AttrValue = interactionID, Operator = new NullableOperators(Operators.NotEqual) };
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

                requestInteractionListGet.SearchCriteria = new SearchCriteriaCollection();
                requestInteractionListGet.SearchCriteria = srchCrit;
                StringList stringList = new StringList();
                if (attributesNames.Count > 0)
                    foreach (string attribute in attributesNames)
                        stringList.Add(attribute);
                //stringList.Add("StartDate");
                //stringList.Add("Subject");
                requestInteractionListGet.AttributeList = stringList;

                requestInteractionListGet.TenantId = tenantId;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    IMessage response = Settings.UCSProtocol.Request(requestInteractionListGet);
                    if (response != null && response.Id == EventInteractionListGet.MessageId)
                    {
                        //EventInteractionListGet eventInteractionListGet = (EventInteractionListGet)response;
                        //if (eventInteractionListGet != null && eventInteractionListGet.InteractionData != null)
                        //{
                        //    interactionDataList = eventInteractionListGet.InteractionData;
                        //    logger.Info("------------RequestGetRecentInteractionList-------------");
                        //    logger.Info("Media Type :" + mediaType);
                        //    logger.Info("Contact ID :" + contactID);
                        //    logger.Info("----------------------------------------------");
                        //    logger.Trace(eventInteractionListGet.ToString());
                        //    output.MessageCode = "200";
                        //    output.Message = "Received Recent Interaction List Successfully";
                        //    output.GetInteractionDataList = interactionDataList;
                        //}
                        output.MessageCode = "200";
                        output.Message = "Get Recent Interaction List Successful";
                        output.IContactMessage = response;
                    }
                    else
                    {
                        output.MessageCode = "2001";
                        output.Message = "Get Recent Interaction List Failed";
                        output.IContactMessage = response;
                    }
                }
                else
                {
                    logger.Warn("GetRecentInteractionList() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Request Get Recent Interaction List " + generalException.ToString());
                output.MessageCode = "2001";
                output.Message = generalException.Message;
                output.IContactMessage = null;
            }
            return output;
        }
        #endregion
    }
}
