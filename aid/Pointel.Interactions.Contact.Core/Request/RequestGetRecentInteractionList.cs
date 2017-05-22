namespace Pointel.Interactions.Contact.Core.Request
{
    using System;

    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.Commons.Protocols;
    using Genesyslab.Platform.Contacts.Protocols;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer;
    using Genesyslab.Platform.Contacts.Protocols.ContactServer.Requests;

    using Pointel.Interactions.Contact.Core.Common;
    using Pointel.Interactions.Contact.Core.Util;

    /// <summary>
    /// This request provides a list of Interactions that you can scroll through. The request can be filtered by specifying constraints, or sorted using specified criteria.
    /// </summary>
    public class RequestGetRecentInteractionList
    {
        #region Methods

        /// <summary>
        /// Gets the recent interaction list.
        /// </summary>
        /// <param name="universalContactServerProtocol">The universal contact server protocol.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public static OutputValues GetRecentInteractionList(string mediaType, string contactID)
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

                SimpleSearchCriteria sSC1 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.MediaTypeId, AttrValue = mediaType.ToLower(), Operator = new NullableOperators(Operators.Equal) };
                SimpleSearchCriteria sSC2 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.StartDate, AttrValue = DateTime.Now.ToString("yyyy") + "-" + DateTime.Now.Month.ToString() + "-" + (DateTime.Now.Day).ToString() + "T00:00:00.000Z", Operator = new NullableOperators(Operators.GreaterOrEqual) };
                SimpleSearchCriteria sSC3 = new SimpleSearchCriteria() { AttrName = InteractionSearchCriteriaConstants.Status, AttrValue = ((int)Statuses.Stopped).ToString(), Operator = new NullableOperators(Operators.Equal) };
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
                stringList.Add("StartDate");
                stringList.Add("Subject");
                requestInteractionListGet.AttributeList = stringList;

                requestInteractionListGet.TenantId = Settings.tenantDBID;
                if (Settings.UCSProtocol != null && Settings.UCSProtocol.State == ChannelState.Opened)
                {
                    logger.Info("------------RequestGetRecentInteractionList-------------");
                    logger.Info("Media Type :" + mediaType);
                    logger.Info("Contact ID :" + contactID);
                    logger.Info("----------------------------------------------");
                    IMessage response = Settings.UCSProtocol.Request(requestInteractionListGet);
                    if (response != null)
                    {
                        logger.Trace(response.ToString());
                        output.IContactMessage = response;
                        output.MessageCode = "200";
                        output.Message = "Received Recent Interaction List Successfully";
                    }
                    else
                    {
                        output.IContactMessage = null;
                        output.MessageCode = "200";
                        output.Message = "Doesn't Received Recent Interaction List Successfully";
                    }
                }
                else
                {
                    output.IContactMessage = null;
                    output.MessageCode = "2001";
                    output.Message = "Universal Contact Server protocol is Null or Closed";
                    logger.Warn("GetRecentInteractionList() : Contact Server protocol is Null..");
                }

            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred while Request Get Recent Interaction List " + generalException.ToString());
                output.IContactMessage = null;
                output.MessageCode = "2001";
                output.Message = generalException.Message;
            }
            return output;
        }

        #endregion Methods
    }
}