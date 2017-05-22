/*
* =====================================
* Pointel.Interactions.TeamCommunicator.Helpers
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Pointel.Interactions.TeamCommunicator.Settings;

namespace Pointel.Interactions.TeamCommunicator.Helpers
{
    public class LuceneParser
    {
        #region Private Read Only Member

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion

        #region Image Files

        private BitmapImage statusImageSource = new BitmapImage();
        private BitmapImage callImageSource = new BitmapImage();
        private BitmapImage favImageSource = new BitmapImage();

        #endregion

        private Query GetParsedQuery(string text, Datacontext.SelectorFilters ContactType)
        {
            BooleanQuery query = new BooleanQuery();
            BooleanQuery.SetMaxClauseCount(0x2710);
            if (text.Length > 0)
            {
                //Add conditions for showall, agent, agentgroups etc
                BooleanQuery subQuery = new BooleanQuery();
                QueryParser queryParser = new QueryParser("ContactType", Datacontext.GetInstance().Analyzer);
                if (ContactType == Datacontext.SelectorFilters.AllTypes)
                {
                    if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Voice)
                    {
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.AgentGroup.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Skill.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Queue.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.RoutingPoint.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.DN.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Contact.ToString()), BooleanClause.Occur.SHOULD);
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.WorkBin)
                    {
                        //if (Datacontext.GetInstance().SelectedOperationType == IPlugins.OperationType.Queue)
                        //    subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                        if (Datacontext.GetInstance().SelectedOperationType == IPlugins.OperationType.Workbin)
                        {
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                            //subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.AgentGroup.ToString()), BooleanClause.Occur.SHOULD);
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                        }
                        else if (Datacontext.GetInstance().SelectedOperationType == IPlugins.OperationType.Transfer)
                        {
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                        }
                        else if (Datacontext.GetInstance().SelectedOperationType == IPlugins.OperationType.Supervisor)
                        {
                            if (Datacontext.GetInstance().IsSupervisorMoveWorkbinEnabled)
                                subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                            if (Datacontext.GetInstance().IsSupervisorMoveQueueEnabled)
                                subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                        }
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Email)
                    {
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Chat)
                    {
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Contact)
                    {
                        subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Contact.ToString()), BooleanClause.Occur.SHOULD);
                    }
                    //subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                    //subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.AgentGroup.ToString()), BooleanClause.Occur.SHOULD);
                    //subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                    ////subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.RoutingPoint.ToString()), BooleanClause.Occur.SHOULD);
                    //subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Skill.ToString()), BooleanClause.Occur.SHOULD);
                    ////subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Queue.ToString()), BooleanClause.Occur.SHOULD);
                    ////subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Contact.ToString()), BooleanClause.Occur.SHOULD);
                }
                else
                {
                    switch (ContactType)
                    {
                        case Datacontext.SelectorFilters.Agent:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Agent.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.AgentGroup:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.AgentGroup.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.InteractionQueue:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.InteractionQueue.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.RoutingPoint:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.RoutingPoint.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.Skill:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Skill.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.Queue:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Queue.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.Contact:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.Contact.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                        case Datacontext.SelectorFilters.DN:
                            subQuery.Add(queryParser.Parse(Datacontext.SelectorFilters.DN.ToString()), BooleanClause.Occur.SHOULD);
                            break;
                    }
                }
                query.Add(subQuery, BooleanClause.Occur.MUST);
            }
            Lucene.Net.Analysis.Token token = null;
            Lucene.Net.Analysis.Token token2 = null;
            TokenStream stream = Datacontext.GetInstance().Analyzer.TokenStream("ContactType", new StringReader(text));
            do
            {
                token2 = token;
                token = stream.Next();
                if (token2 != null)
                {
                    //Add conditions for showall, agent, agentgroups etc
                    string stoken = token2.TermText();
                    BooleanQuery outputQuery = new BooleanQuery();
                    if (ContactType == Datacontext.SelectorFilters.AllTypes)
                    {
                        this.TokenToQuery("DisplayName", stoken, ref outputQuery);
                        this.TokenToQuery("FirstName", stoken, ref outputQuery);
                        this.TokenToQuery("LastName", stoken, ref outputQuery);
                        this.TokenToQuery("UserName", stoken, ref outputQuery);
                        this.TokenToQuery("Number", stoken, ref outputQuery);
                        this.TokenToQuery("Name", stoken, ref outputQuery);
                        this.TokenToQuery("PhoneNumber", stoken, ref outputQuery);
                        this.TokenToQuery("EmailAddress", stoken, ref outputQuery);
                    }
                    else
                    {
                        if (ContactType == Datacontext.SelectorFilters.AgentGroup || ContactType == Datacontext.SelectorFilters.RoutingPoint ||
                            ContactType == Datacontext.SelectorFilters.Skill || ContactType == Datacontext.SelectorFilters.InteractionQueue ||
                             ContactType == Datacontext.SelectorFilters.Queue || ContactType == Datacontext.SelectorFilters.DN)
                        {
                            this.TokenToQuery("Name", stoken, ref outputQuery);
                            if (ContactType == Datacontext.SelectorFilters.Queue || ContactType == Datacontext.SelectorFilters.RoutingPoint ||
                                            ContactType == Datacontext.SelectorFilters.DN)
                                this.TokenToQuery("Number", stoken, ref outputQuery);
                            if (ContactType == Datacontext.SelectorFilters.InteractionQueue)
                                this.TokenToQuery("DisplayName", stoken, ref outputQuery);
                        }
                        else if (ContactType == Datacontext.SelectorFilters.Agent || ContactType == Datacontext.SelectorFilters.Contact)
                        {
                            this.TokenToQuery("FirstName", stoken, ref outputQuery);
                            this.TokenToQuery("LastName", stoken, ref outputQuery);
                            if (ContactType == Datacontext.SelectorFilters.Agent)
                            {
                                this.TokenToQuery("UserName", stoken, ref outputQuery);
                            }
                            if (ContactType == Datacontext.SelectorFilters.Contact)
                            {
                                this.TokenToQuery("Name", stoken, ref outputQuery);
                                this.TokenToQuery("PhoneNumber", stoken, ref outputQuery);
                                this.TokenToQuery("EmailAddress", stoken, ref outputQuery);
                            }
                        }
                    }
                    query.Add(outputQuery, BooleanClause.Occur.MUST);
                }
            }
            while (token != null);
            stream.Close();
            return query;
        }

        private void TokenToQuery(string fieldName, string stoken, ref BooleanQuery outputQuery)
        {
            if (!stoken.Contains("*"))
            {
                stoken = stoken + "*";
            }
            Term term = new Term(fieldName, stoken);
            WildcardQuery query = new WildcardQuery(term);
            outputQuery.Add(query, BooleanClause.Occur.SHOULD);
        }

        private void AddContacts(string firstName, string lastName, string emailAddress, string contactID, string phoneNumber)
        {
            string callToolTip = "";
            string toolTip = "";
            string favToolTip = "";
            favImageSource = null;

            callToolTip = "Add e-mail address";
            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
            statusImageSource = null;

            Datacontext.GetInstance().CurrentMediaImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
            Datacontext.GetInstance().currentMediaStatus = Datacontext.GetInstance().hshAgentEmailStatus;

            List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
            contactID).ToList();

            foreach (DataRow dr in foundRows)
            {
                favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                favToolTip = dr["Category"].ToString();
            }
            toolTip += firstName;
            if (lastName != string.Empty)
                toolTip += " " + lastName;

            if (!string.IsNullOrEmpty(emailAddress))
            {
                toolTip += "\nEmail Address : ";
                foreach (string emailID in emailAddress.Split(','))
                {
                    if (emailID != string.Empty)
                        toolTip += emailID + "\n";
                }

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    toolTip += "Phone Number : ";
                    foreach (string phoneno in phoneNumber.Split(','))
                    {
                        if (phoneno != string.Empty)
                            toolTip += phoneno + "\n";
                    }
                }
            }
            else
            {
                if (foundRows.Count == 0)
                {
                    callToolTip = "Add to Favorites";
                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                }
                else
                {
                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                    callToolTip = "Remove from Favorites";
                }
            }
            string searchedName = firstName + " " + lastName;
            if (string.IsNullOrEmpty(searchedName.Trim()) && !string.IsNullOrEmpty(phoneNumber))
                searchedName = phoneNumber.Split(',')[0];
            else if (!string.IsNullOrEmpty(emailAddress))
                searchedName = emailAddress.Split(',')[0];
            Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
             searchedName,
                // + "\n" + doc.GetField("EmailAddress").StringValue().Split(' ').First(),
              statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "",
              new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Contact.png", UriKind.Relative)),
              toolTip, Datacontext.SelectorFilters.Contact, "", contactID, favImageSource, favToolTip, emailAddress));
        }


        /// <summary>
        /// Gets the search results.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="selectedType">Type of the selected.</param>
        public void GetSearchResults(string text, Datacontext.SelectorFilters selectedType, string[] contactIDs = null)
        {
            _logger.Info("GetSeacrhResults - Search Text : " + text);
            _logger.Info("GetSeacrhResults - Selector Filters : " + selectedType);

            //Pass Contact Request and get response
            Genesyslab.Platform.Commons.Protocols.IMessage message = null;
            bool isIndex = true;
            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact &&
               Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact) &&
              (message = ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).GetContactsforForwardMail(text, Datacontext.GetInstance().MaxSuggestionSize, out isIndex, contactIDs)) != null)
            {
                Datacontext.GetInstance().InternalTargets = "";
                switch (message.Id)
                {
                    case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventContactListGet.MessageId:
                        Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventContactListGet contactList = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventContactListGet)message;
                        if (contactList.ContactData != null && contactList.ContactData.Count > 0)
                        {
                            if (!contactList.TotalCount.IsNull && contactList.TotalCount.Value > Datacontext.GetInstance().MaxSuggestionSize)
                                Datacontext.GetInstance().InternalTargets = contactList.TotalCount.Value == 2000 ? "2000+" : contactList.TotalCount.Value + " Matching Internal Targets";
                            else
                                Datacontext.GetInstance().InternalTargets = contactList.ContactData.Count + " " + "Matching Internal Targets";
                            for (int index = 0; index < contactList.ContactData.Count && index < 100; index++)
                            {
                                string firstName = "", lastName = "", emailAddress = "", phoneNumber = "";
                                foreach (Genesyslab.Platform.Contacts.Protocols.ContactServer.Attribute attribute in contactList.ContactData[index].ContactAttributesList)
                                {
                                    switch (attribute.AttributeName.ToLower())
                                    {
                                        case "firstname":
                                            firstName = attribute.AttributeValue.ToString();
                                            break;
                                        case "lastname":
                                            lastName = attribute.AttributeValue.ToString();
                                            break;
                                        case "emailaddress":
                                            emailAddress = attribute.AttributeValue.ToString();
                                            break;
                                        case "phonenumber":
                                            phoneNumber = attribute.AttributeValue.ToString();
                                            break;
                                    }
                                }
                                AddContacts(firstName, lastName, emailAddress, contactList.ContactData[index].Id, phoneNumber);
                            }
                        }
                        break;
                    case Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventSearch.MessageId:
                        Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventSearch eventSearch = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventSearch)message;
                        if (eventSearch.Documents != null && eventSearch.Documents.Count > 0)
                        {
                            Datacontext.GetInstance().InternalTargets = eventSearch.FoundDocuments + " " + "Matching Internal Targets";
                            _logger.Info("total no of contact received: " + eventSearch.Documents.Count);
                            for (int index = 0; index < eventSearch.Documents.Count && index < 100; index++)
                            {
                                string firstName = "", lastName = "", emailAddress = "", phonenumber = "";
                                foreach (string key in eventSearch.Documents[index].Fields.AllKeys.Distinct())
                                {
                                    object[] values = eventSearch.Documents[index].Fields.GetValues(key);
                                    switch (key.ToLower())
                                    {
                                        case "firstname":
                                            foreach (object value in values)
                                            {
                                                firstName = firstName + (string.IsNullOrEmpty(firstName) ? "" : ",") + value.ToString();
                                            }
                                            break;
                                        case "lastname":
                                            foreach (object value in values)
                                            {
                                                lastName = lastName + (string.IsNullOrEmpty(lastName) ? "" : ",") + value.ToString();
                                            }
                                            break;
                                        case "emailaddress":
                                            foreach (object value in values)
                                            {
                                                emailAddress = emailAddress + (string.IsNullOrEmpty(emailAddress) ? "" : ",") + value.ToString();
                                            }
                                            break;
                                        case "phonenumber":
                                            foreach (object value in values)
                                            {
                                                phonenumber = phonenumber + (string.IsNullOrEmpty(phonenumber) ? "" : ",") + value.ToString();
                                                break;
                                            }
                                            break;
                                    }

                                }
                                AddContacts(firstName, lastName, emailAddress, eventSearch.Documents[index].Fields["Id"].ToString(), phonenumber);
                            }
                        }
                        break;
                    default: break;
                }
            }
            else if (!isIndex && contactIDs != null && contactIDs.Length > 0)
            {
                List<string> attributes = new List<string>();
                attributes.Add("PhoneNumber");
                attributes.Add("EmailAddress");
                attributes.Add("FirstName");
                attributes.Add("LastName");
                for (int i = 0; i < contactIDs.Length; i++)
                {
                    message = ((IPlugins.IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[IPlugins.Plugins.Contact]).GetAllAttributes(contactIDs[i], attributes);
                    if (message != null && message.Id == Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes.MessageId)
                    {
                        Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes attribute = (Genesyslab.Platform.Contacts.Protocols.ContactServer.Events.EventGetAttributes)message;
                        if (attribute.Attributes != null && attribute.Attributes.Count > 0)
                        {
                            string firstName = "", lastName = "", emailAddress = "", phonenumber = "";
                            for (int j = 0; j < attribute.Attributes.Count; j++)
                            {
                                switch (attribute.Attributes[j].AttrName.ToLower())
                                {
                                    case "firstname":
                                        for (int k = 0; k < attribute.Attributes[j].AttributesInfoList.Count; k++)
                                        {
                                            firstName = firstName + (string.IsNullOrEmpty(firstName) ? "" : ",") + attribute.Attributes[j].AttributesInfoList[k].AttrValue.ToString();
                                        }
                                        break;
                                    case "lastname":
                                        for (int k = 0; k < attribute.Attributes[j].AttributesInfoList.Count; k++)
                                        {
                                            lastName = lastName + (string.IsNullOrEmpty(lastName) ? "" : ",") + attribute.Attributes[j].AttributesInfoList[k].AttrValue.ToString();
                                        }
                                        break;
                                    case "emailaddress":
                                        for (int k = 0; k < attribute.Attributes[j].AttributesInfoList.Count; k++)
                                        {
                                            emailAddress = emailAddress + (string.IsNullOrEmpty(emailAddress) ? "" : ",") + attribute.Attributes[j].AttributesInfoList[k].AttrValue.ToString();
                                        }
                                        break;
                                    case "phonenumber":
                                        for (int k = 0; k < attribute.Attributes[j].AttributesInfoList.Count; k++)
                                        {
                                            phonenumber = phonenumber + (string.IsNullOrEmpty(phonenumber) ? "" : ",") + attribute.Attributes[j].AttributesInfoList[k].AttrValue.ToString();
                                        }
                                        break;
                                }

                            }
                            if (text == "*" || text == "@" ||
                                   firstName.Contains(text) || lastName.Contains(text) ||
                                       emailAddress.Contains(text) || phonenumber.Contains(text))
                            {
                                AddContacts(firstName, lastName, emailAddress, contactIDs[i], phonenumber);
                            }
                        }
                    }
                }
            }
            else
                Datacontext.GetInstance().InternalTargets = "0 " + "Matching Internal Targets";

            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
            {
                if (text.Length > 0)
                {
                    bool result = ValidateEmailAddress(text);
                    if (!result)
                        return;

                    string callToolTip = "";
                    string toolTip = "";

                    callToolTip = "Add e-mail address";
                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                    statusImageSource = null;

                    toolTip += "Email Address : " + text;
                    //Document doc = new Document();
                    //doc.Add(new Lucene.Net.Documents.Field("EmailAddress", text, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.TOKENIZED));


                    //Datacontext.GetInstance().SearchedContactDocuments.Add(text, doc);

                    Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                      text, statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "",
                      new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Contact.png", UriKind.Relative)),
               toolTip, Datacontext.SelectorFilters.Contact, "", text, null, "", text));
                }
            }

            else
            {
                IndexSearcher MyIndexSearcher = new IndexSearcher(Datacontext.GetInstance().Directory, true);
                BooleanQuery.SetMaxClauseCount(0x2710);

                Query mainQuery = this.GetParsedQuery(text, selectedType);
                Hits hits = MyIndexSearcher.Search(mainQuery);
                mainQuery = null;
                Datacontext.GetInstance().SearchedAgentDocuments.Clear();
                Datacontext.GetInstance().SearchedAgentGroupDocuments.Clear();
                Datacontext.GetInstance().SearchedSkillDocuments.Clear();
                Datacontext.GetInstance().SearchedInteractionQueueDocuments.Clear();
                Datacontext.GetInstance().SearchedContactDocuments.Clear();

                if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email ||
                        Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                {
                    Datacontext.GetInstance().CurrentMediaImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                    Datacontext.GetInstance().currentMediaStatus = Datacontext.GetInstance().hshAgentEmailStatus;
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                {
                    Datacontext.GetInstance().CurrentMediaImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png", UriKind.Relative));
                    Datacontext.GetInstance().currentMediaStatus = Datacontext.GetInstance().hshAgentChatStatus;
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                {
                    Datacontext.GetInstance().CurrentMediaImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative));
                    Datacontext.GetInstance().currentMediaStatus = Datacontext.GetInstance().hshAgentVoiceStatus;
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                {
                    //if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                    {
                        Datacontext.GetInstance().CurrentMediaImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative));
                        Datacontext.GetInstance().currentMediaStatus = Datacontext.GetInstance().hshAgentEmailStatus;
                    }
                }

                if (hits != null && hits.Length() > 0)
                {
                    Pointel.Interactions.IPlugins.AgentMediaStatus status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                    for (int i = 0; i < hits.Length(); i++)
                    {
                        Document doc = hits.Doc(i);
                        _logger.Info(doc);
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                        if (doc.GetField("ContactType") != null)
                        {
                            #region Agent Group

                            if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.AgentGroup.ToString())
                            {
                                Datacontext.GetInstance().SearchedAgentGroupDocuments.Add(doc.GetField("Name").StringValue(), doc);
                                string callToolTip = "";
                                string toolTip = "";
                                string favToolTip = "";

                                if (Datacontext.GetInstance().hshAgentGroupStatus.ContainsKey(doc.GetField("Name").StringValue()))
                                    status = (Pointel.Interactions.IPlugins.AgentMediaStatus)Datacontext.GetInstance().hshAgentGroupStatus[doc.GetField("Name").StringValue()];

                                callToolTip = this.GetStatusData(status);

                                toolTip = "Agent Group : " + doc.GetField("Name").StringValue();
                                favImageSource = null;
                                List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                    doc.GetField("Name").StringValue()).ToList();

                                foreach (DataRow dr in foundRows)
                                {
                                    if (status != Pointel.Interactions.IPlugins.AgentMediaStatus.Ready && status != Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady)
                                    {
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                                        callToolTip = "Remove from Favorites";
                                    }
                                    favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                                    favToolTip = dr["Category"].ToString();
                                }

                                Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                                    doc.GetField("Name").StringValue(), statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType,
                                status.ToString(), new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Group.short.png", UriKind.Relative)),
                                toolTip, Datacontext.SelectorFilters.AgentGroup, Datacontext.GetInstance().RoutingAddress, doc.GetField("Name").StringValue(), favImageSource, favToolTip));
                            }

                            #endregion

                            #region Contact

                            //else if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.Contact.ToString())
                            //{
                            //    if (doc.GetField("FirstName").StringValue() == string.Empty && doc.GetField("LastName").StringValue() == string.Empty &&
                            //        doc.GetField("EmailAddress").StringValue() == string.Empty)
                            //        continue;
                            //    Datacontext.GetInstance().SearchedContactDocuments.Add(doc.GetField("DBID").StringValue(), doc);
                            //    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                            //    {
                            //        string callToolTip = "";
                            //        string toolTip = "";
                            //        string favToolTip = "";

                            //        callToolTip = "Add e-mail address";
                            //        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                            //        statusImageSource = null;

                            //        List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                            //        doc.GetField("DBID").StringValue()).ToList();

                            //        foreach (DataRow dr in foundRows)
                            //        {
                            //            favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            //            favToolTip = dr["Category"].ToString();
                            //        }
                            //        toolTip += doc.GetField("FirstName").StringValue();
                            //        if (doc.GetField("LastName").StringValue() != string.Empty)
                            //            toolTip += " " + doc.GetField("LastName").StringValue();
                            //        if (doc.GetField("EmailAddress").StringValue() != null && doc.GetField("EmailAddress").StringValue() != string.Empty)
                            //        {
                            //            toolTip += "\nEmail Address : ";
                            //            foreach (string emailAddress in doc.GetField("EmailAddress").StringValue().Split(' '))
                            //            {
                            //                if (emailAddress != string.Empty)
                            //                    toolTip += emailAddress + "\n";
                            //            }
                            //        }
                            //        else
                            //        {
                            //            if (foundRows.Count == 0)
                            //            {
                            //                callToolTip = "Add to Favorites";
                            //                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            //            }
                            //            else
                            //            {
                            //                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                            //                callToolTip = "Remove from Favorites";
                            //            }
                            //        }
                            //        Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                            //          doc.GetField("FirstName").StringValue() + " " + doc.GetField("LastName").StringValue(),
                            //            // + "\n" + doc.GetField("EmailAddress").StringValue().Split(' ').First(),
                            //          statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "",
                            //          new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Contact.png", UriKind.Relative)),
                            //   toolTip, Datacontext.SelectorFilters.Contact, "", doc.GetField("DBID").StringValue(), favImageSource, favToolTip));

                            //    }
                            //}

                            #endregion

                            #region Agent

                            //else
                            if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.Agent.ToString())
                            {
                                //if (doc.GetField("UserName").StringValue() == Datacontext.GetInstance().UserName)
                                //    continue;
                                Datacontext.GetInstance().SearchedAgentDocuments.Add(doc.GetField("UserName").StringValue(), doc);
                                string callToolTip = "";
                                string DN = "";
                                string toolTip = "";
                                string favToolTip = "";
                                string fName = doc.GetField("FirstName").StringValue();
                                string lname = doc.GetField("LastName").StringValue();
                                if (Datacontext.GetInstance().currentMediaStatus.ContainsKey(doc.GetField("EmployeeID").StringValue()))
                                    status = (Pointel.Interactions.IPlugins.AgentMediaStatus)Datacontext.GetInstance().currentMediaStatus[doc.GetField("EmployeeID").StringValue()];


                                if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Chat)
                                {
                                    if (!Datacontext.GetInstance().AvailableChatStatus.Contains(status))
                                        // || !Datacontext.GetInstance().AvailableStatus.Contains(Pointel.Interactions.IPlugins.AgentMediaStatus.All))
                                        continue;
                                }
                                else if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Email)
                                {
                                    if (!Datacontext.GetInstance().AvailableEmailStatus.Contains(status))
                                        // || !Datacontext.GetInstance().AvailableStatus.Contains(Pointel.Interactions.IPlugins.AgentMediaStatus.All))
                                        continue;
                                }

                                callToolTip = this.GetStatusData(status);

                                DN = "";
                                toolTip = "UserName: " + doc.GetField("UserName").StringValue() + "\n" + "FirstName: " + fName + "\n" + "LastName :" + lname;
                                favImageSource = null;
                                List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                    doc.GetField("UserName").StringValue()).ToList();

                                foreach (DataRow dr in foundRows)
                                {
                                    if (status != Pointel.Interactions.IPlugins.AgentMediaStatus.Ready && status != Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady
                                         && status != Pointel.Interactions.IPlugins.AgentMediaStatus.ConditionallyReady
                                        && Datacontext.GetInstance().CurrentInteractionType != IPlugins.InteractionType.WorkBin
                                        && Datacontext.GetInstance().CurrentInteractionType != IPlugins.InteractionType.Contact)
                                    {
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                                        callToolTip = "Remove from Favorites";
                                    }
                                    favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                                    favToolTip = dr["Category"].ToString();
                                }
                                Dispatcher.CurrentDispatcher.Invoke((Action)delegate
                                {
                                    Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                                        (fName + " " + lname), statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType,
                                    status.ToString(), new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Agent.png", UriKind.Relative)),
                                    toolTip, Datacontext.SelectorFilters.Agent, DN, doc.GetField("UserName").StringValue(), favImageSource, favToolTip));
                                });
                            }

                            #endregion

                            #region InteractionQueue

                            else if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.InteractionQueue.ToString())
                            {
                                Datacontext.GetInstance().SearchedInteractionQueueDocuments.Add(doc.GetField("Name").StringValue(), doc);
                                string callToolTip = "";
                                string toolTip = "";
                                string favToolTip = "";
                                toolTip = "Interaction Queue Name : " + doc.GetField("Name").StringValue();
                                favImageSource = null;
                                statusImageSource = null;

                                switch (Datacontext.GetInstance().CurrentInteractionType)
                                {
                                    case Pointel.Interactions.IPlugins.InteractionType.Email:
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                        callToolTip = "Start a Consult Interaction";
                                        break;

                                    case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                        callToolTip = "Start a Consult Interaction";
                                        break;

                                    case Pointel.Interactions.IPlugins.InteractionType.WorkBin:

                                        if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.InteractionQueue.ToString())
                                        {
                                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                            callToolTip = "Move to Queue";
                                        }
                                        //if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                        //{
                                        //    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                        //    callToolTip = "Move to Queue";
                                        //}
                                        else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                        {
                                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                            callToolTip = "Select a Workbin";
                                        }
                                        else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                        {
                                            callToolTip = "Start a Consult Interaction";
                                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                        }
                                        break;

                                    case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                        callToolTip = "Add to Favorites";
                                        statusImageSource = null;
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                                        //callToolTip = "Establish a new Phone Call";
                                        //callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                        break;

                                    default:
                                        callImageSource = null;
                                        callToolTip = "";
                                        break;
                                }

                                List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                    doc.GetField("Name").StringValue()).ToList();

                                foreach (DataRow dr in foundRows)
                                {
                                    // if (status != Pointel.Interactions.IPlugins.AgentMediaStatus.Ready && status != Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady)
                                    {
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                                        callToolTip = "Remove from Favorites";
                                    }
                                    favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                                    favToolTip = dr["Category"].ToString();
                                }

                                Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                                    doc.GetField("Name").StringValue(), statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType,
                                "", new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Queue.png", UriKind.Relative)),
                                toolTip, Datacontext.SelectorFilters.InteractionQueue, Datacontext.GetInstance().RoutingAddress, doc.GetField("Name").StringValue(), favImageSource, favToolTip));
                            }

                            #endregion

                            #region Skill

                            else if (doc.GetField("ContactType").StringValue() == Datacontext.SelectorFilters.Skill.ToString())
                            {
                                Datacontext.GetInstance().SearchedSkillDocuments.Add(doc.GetField("Name").StringValue(), doc);
                                string callToolTip = "";
                                string toolTip = "";
                                string favToolTip = "";
                                toolTip = "Skill Name : " + doc.GetField("Name").StringValue();
                                favImageSource = null;
                                statusImageSource = null;


                                callToolTip = "Establish a new Phone Call";

                                toolTip = "Skill Name : " + doc.GetField("Name").StringValue();
                                favImageSource = null;
                                List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                    doc.GetField("Name").StringValue()).ToList();

                                foreach (DataRow dr in foundRows)
                                {
                                    if (!Datacontext.GetInstance().IsStatAlive)
                                    {
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.Remove.png", UriKind.Relative));
                                        callToolTip = "Remove from Favorites";
                                    }
                                    favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                                    favToolTip = dr["Category"].ToString();
                                }
                                switch (Datacontext.GetInstance().CurrentInteractionType)
                                {
                                    case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                        callToolTip = "Establish a new Phone Call";
                                        break;
                                    case Pointel.Interactions.IPlugins.InteractionType.Email:
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                        callToolTip = "Start a Consult Interaction";
                                        break;
                                    case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                        callToolTip = "Start a Consult Interaction";
                                        break;
                                    default:
                                        callImageSource = null;
                                        callToolTip = "";
                                        break;
                                }

                                //Set the status image as Ready if stat server is opened else set the status image as disabled
                                if (Datacontext.GetInstance().IsStatAlive)
                                    statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Ready.png", UriKind.Relative));
                                else
                                    statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Disable.btn.png", UriKind.Relative));

                                Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                                    doc.GetField("Name").StringValue(), statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType,
                                "", new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Agent.skill.png", UriKind.Relative)),
                                toolTip, Datacontext.SelectorFilters.Skill, Datacontext.GetInstance().RoutingAddress, doc.GetField("Name").StringValue(), favImageSource, favToolTip));
                            }

                            #endregion
                        }
                    }
                    hits = null;
                }
                else
                {
                    if (Datacontext.GetInstance().TeamCommunicator != null)
                        Datacontext.GetInstance().TeamCommunicator.Clear();
                    Int64 EnteredIntValue = 0;
                    bool isIntOrNot = false;
                    isIntOrNot = Int64.TryParse(text, out EnteredIntValue);

                    if (isIntOrNot)//Entered text is a number(DN)
                    {
                        string favToolTip = "";
                        favImageSource = null;
                        bool isFav = false;
                        List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == text).ToList();
                        foreach (DataRow dr in foundRows)
                        {
                            isFav = true;
                            favImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            favToolTip = dr["DisplayName"].ToString();
                            break;
                        }
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                        {
                            Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator("Establish a new Phone Call",
                            new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative)), text,
                                null, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "",
                                new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Agent.png", UriKind.Relative)), text, Datacontext.SelectorFilters.DN,
                                text, text, favImageSource, favToolTip));

                            if (isFav)
                                Datacontext.GetInstance().InternalTargets = "1 " + "Matching Internal Targets";
                            else
                                Datacontext.GetInstance().InternalTargets = "0 " + "Matching Internal Targets";
                        }
                        else
                        {

                        }
                    }
                    //else //if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                    //{   //If CurrentInteractionType is contact or CurrentInteractionType is voice and entered string is email id
                    //    if (text.Length > 0)
                    //    {
                    //        bool result = ValidateEmailAddress(text);
                    //        if (!result)
                    //            return;

                    //        string callToolTip = "";
                    //        string toolTip = "";

                    //        callToolTip = "Add e-mail address";
                    //        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                    //        statusImageSource = null;

                    //        toolTip += "Email Address : " + text;
                    //        Document doc = new Document();
                    //        doc.Add(new Lucene.Net.Documents.Field("EmailAddress", text, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.TOKENIZED));


                    //        Datacontext.GetInstance().SearchedContactDocuments.Add(text, doc);

                    //        Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator(callToolTip, callImageSource,
                    //          text, statusImageSource, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "",
                    //          new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Contact.png", UriKind.Relative)),
                    //   toolTip, Datacontext.SelectorFilters.Contact, "", text, null, ""));

                    //    }
                    //}
                    //else         //Entered text is an email id so show send a new email address
                    //{

                    //}
                }
            }

        }

        public bool ValidateEmailAddress(string email)
        {
            email = email.Replace(";", ",");
            string[] emailAddress = (email.EndsWith(",") ? email.Remove(email.Length - 1) : email).Split(',');
            bool status = true;
            foreach (string address in emailAddress)
            {
                status = System.Text.RegularExpressions.Regex.IsMatch(address.TrimStart().TrimEnd(), @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
                if (!status)
                    break;
            }
            return status;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GetStatusData(Pointel.Interactions.IPlugins.AgentMediaStatus status)
        {
            string callToolTip = "";
            statusImageSource = null;
            try
            {
                switch (status)
                {
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut:

                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        {
                            status = Pointel.Interactions.IPlugins.AgentMediaStatus.LoggedOut;
                            statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Logout-state.png", UriKind.Relative));
                            if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                            {
                                callToolTip = "Move to Queue";
                                statusImageSource = null;
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                            }
                            else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                            {
                                callToolTip = "Select a Workbin";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                            }
                            else
                            {
                                callToolTip = "Add to Favorites";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            }
                        }
                        else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                        {
                            callToolTip = "Add e-mail address";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                            statusImageSource = null;
                        }
                        else
                        {
                            callToolTip = "Add to Favorites";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Logout-state.png", UriKind.Relative));
                        }
                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.Busy:

                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.Busy;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Busy.png", UriKind.Relative));
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                        {
                            if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                            {
                                callToolTip = "Move to Queue";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                statusImageSource = null;
                            }
                            else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                            {
                                callToolTip = "Select a Workbin";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                            }
                            else
                            {
                                callToolTip = "Add to Favorites";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                            }
                        }
                        else
                        {
                            callToolTip = "Add to Favorites";
                            callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                        }
                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.ConditionallyReady:
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.ConditionallyReady;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Busy.png", UriKind.Relative));
                        switch (Datacontext.GetInstance().CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }
                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.Ready:
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Ready.png", UriKind.Relative));
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.Ready;
                        switch (Datacontext.GetInstance().CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                callToolTip = "Establish a new Phone Call";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }

                        break;
                    case Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady:
                        status = Pointel.Interactions.IPlugins.AgentMediaStatus.NotReady;
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Not_Ready.png", UriKind.Relative));
                        switch (Datacontext.GetInstance().CurrentInteractionType)
                        {
                            case Pointel.Interactions.IPlugins.InteractionType.Email:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Voice:
                                callToolTip = "Establish a new Phone Call";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Voice.Short.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.Chat:
                                callToolTip = "Start a Consult Interaction";
                                callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Chat.png", UriKind.Relative));
                                break;

                            case Pointel.Interactions.IPlugins.InteractionType.WorkBin:
                                if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                                {
                                    callToolTip = "Move to Queue";
                                    statusImageSource = null;
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.queue.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                                {
                                    callToolTip = "Select a Workbin";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/move.to.workbin.png", UriKind.Relative));
                                }
                                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                {
                                    callToolTip = "Start a Consult Interaction";
                                    callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Email.png", UriKind.Relative));
                                }
                                break;

                            default:
                                callToolTip = "";
                                callImageSource = null;
                                break;
                        }
                        break;
                    default:
                        callToolTip = "Add to Favorites";
                        callImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Favourite.png", UriKind.Relative));
                        statusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.TeamCommunicator;component/Images/Disable.btn.png", UriKind.Relative));
                        break;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : GetStatusData() : " + generalException.Message.ToString());
            }
            return callToolTip;
        }
    }
}
