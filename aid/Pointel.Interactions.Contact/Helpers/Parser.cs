#region System Namespaces
using System;
using System.IO;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
#endregion

#region Lucene.Net Namespaces
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

#endregion

#region Pointel Namespace

#endregion

namespace Pointel.Interactions.Contact.Helpers
{
    public class Parser
    {
        #region Private Members Declaration

        private Analyzer _analyzer = new StandardAnalyzer(Version.LUCENE_29);
        //private Lucene.Net.Store.Directory _directory = new RAMDirectory();
        
        private Lucene.Net.Store.Directory _directory = FSDirectory.Open(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\temp"));


        private IndexWriter _writer;

        private DataTable _contactsMetaDataTable;
        private DateTime _leastDate = new DateTime();
        private BooleanQuery _tempQuery = new BooleanQuery();
        private List<string> _contactAttributeDateFields = new List<string>();

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion

        #region Method Definition

        /// <summary>
        /// Gets the parsed query.
        /// </summary>
        /// <param name="searchCriterias">The search criterias.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns></returns>
        private Query GetParsedQuery(List<Criteria> searchCriterias, MatchCondition matchCondition)
        {
            BooleanQuery mainQuery = new BooleanQuery();
            BooleanQuery.SetMaxClauseCount(0x2710);
            _tempQuery = new BooleanQuery();
            foreach (Criteria searchCriteria in searchCriterias)
            {
                DateTime chkDate;
                if (DateTime.TryParse(searchCriteria.Value.ToString(), out chkDate))
                {
                    if (chkDate != null)
                    {
                        _logger.Debug("Search Field is DateTime value");
                        GetNumericQuery(searchCriteria, matchCondition);
                    }
                }
                else
                {
                    _logger.Debug("Search Field is String value");
                    if (searchCriteria.Field == "Status" && searchCriteria.Value == "All")
                    {
                        searchCriteria.Value="In Progress";
                        GetStringQuery(searchCriteria, MatchCondition.MatchAny);
                        searchCriteria.Value="Done";
                        GetStringQuery(searchCriteria, MatchCondition.MatchAny);
                    }
                    else
                        GetStringQuery(searchCriteria, matchCondition);
                }

                #region Old
                //if (searchCriteria.Field == "Status" || searchCriteria.Field == "Subject")
                //{
                //    Lucene.Net.Analysis.Token token = null;
                //    Lucene.Net.Analysis.Token token2 = null;
                //    TokenStream stream = _analyzer.TokenStream(searchCriteria.Field.ToString(), new StringReader(searchCriteria.Value));
                //    do
                //    {
                //        token2 = token;
                //        token = stream.Next();
                //        if (token2 != null)
                //        {
                //            string stoken = token2.TermText();
                //            BooleanQuery outputQuery = new BooleanQuery();
                //            this.TokenToQuery(searchCriteria.Field.ToString(), stoken, ref outputQuery);
                //            if (matchCondition == MatchCondition.MatchAll)
                //                query11.Add(outputQuery, BooleanClause.Occur.MUST);
                //            else
                //                query11.Add(outputQuery, BooleanClause.Occur.SHOULD);
                //        }
                //    }
                //    while (token != null);
                //}
                //else
                //{
                //    DateTime SearchFrom = new DateTime();
                //    DateTime SearchTo = new DateTime();
                //    if (searchCriteria.Condition == SearchCondition.Before)
                //    {
                //        SearchFrom = _leastDate;
                //        SearchTo = Convert.ToDateTime(searchCriteria.Value);
                //    }
                //    else if (searchCriteria.Condition == SearchCondition.On)
                //    {
                //        SearchFrom = Convert.ToDateTime(searchCriteria.Value);
                //        SearchTo = Convert.ToDateTime(searchCriteria.Value);
                //    }
                //    else
                //    {
                //        SearchFrom = Convert.ToDateTime(searchCriteria.Value);
                //        SearchTo = DateTime.Now;
                //    }

                //    var numericQuery = NumericRangeQuery.NewIntRange(searchCriteria.Field.ToString(), int.MaxValue, int.Parse(SearchFrom.ToString("yyyyMMddHHmmss")),
                //        int.Parse(SearchTo.ToString("yyyyMMddHHmmss")), true,
                //        (searchCriteria.Condition == SearchCondition.Before) ? true : false);//To avoid inclusive of given date for before condition in searching

                //    if (matchCondition == MatchCondition.MatchAll)
                //        query11.Add(numericQuery, BooleanClause.Occur.MUST);
                //    else
                //        query11.Add(numericQuery, BooleanClause.Occur.SHOULD);
                //}
                #endregion

            }
            if (matchCondition == MatchCondition.MatchAll)
                mainQuery.Add(_tempQuery, BooleanClause.Occur.MUST);
            else
                mainQuery.Add(_tempQuery, BooleanClause.Occur.SHOULD);

            return mainQuery;
        }

        /// <summary>
        /// Gets the string query.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="matchCondition">The match condition.</param>
        private void GetStringQuery(Criteria searchCriteria, MatchCondition matchCondition)
        {
            try
            {
                Lucene.Net.Analysis.Token token = null;
                Lucene.Net.Analysis.Token token2 = null;
                TokenStream stream = _analyzer.TokenStream(searchCriteria.Field.ToString(), new StringReader(searchCriteria.Value));
                do
                {
                    token2 = token;
                    token = stream.Next();
                    if (token2 != null)
                    {
                        string stoken = token2.TermText();
                        BooleanQuery outputQuery = new BooleanQuery();
                        this.TokenToQuery(searchCriteria.Field.ToString(), stoken, searchCriteria.Condition.ToString(), ref outputQuery);
                        if (matchCondition == MatchCondition.MatchAll)
                            _tempQuery.Add(outputQuery, BooleanClause.Occur.MUST);
                        else
                            _tempQuery.Add(outputQuery, BooleanClause.Occur.SHOULD);
                    }
                }
                while (token != null);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while creating String Query :" + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Gets the numeric query.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="matchCondition">The match condition.</param>
        private void GetNumericQuery(Criteria searchCriteria, MatchCondition matchCondition)
        {
            try
            {
                DateTime SearchFrom = new DateTime();
                DateTime SearchTo = new DateTime();
                if (searchCriteria.Condition == SearchCondition.Before)
                {
                    SearchFrom = _leastDate;
                    SearchTo = Convert.ToDateTime(searchCriteria.Value);

                    var numericQuery = NumericRangeQuery.NewLongRange(searchCriteria.Field.ToString(), int.MaxValue, long.Parse(SearchFrom.ToString("yyyyMMddHHmmss")),
                        long.Parse(SearchTo.ToString("yyyyMMddHHmmss")), true,
                        (searchCriteria.Condition == SearchCondition.Before) ? true : false);//To avoid inclusive of given date for before condition in searching

                    if (matchCondition == MatchCondition.MatchAll)
                        _tempQuery.Add(numericQuery, BooleanClause.Occur.MUST);
                    else
                        _tempQuery.Add(numericQuery, BooleanClause.Occur.SHOULD);
                }
                else if (searchCriteria.Condition == SearchCondition.On)
                {
                    SearchFrom = Convert.ToDateTime(searchCriteria.Value);
                    SearchTo = SearchFrom;

                    this.GetTermQuery(searchCriteria, SearchFrom, matchCondition);
                }
                else
                {
                    SearchFrom = Convert.ToDateTime(searchCriteria.Value);
                    SearchTo = DateTime.Now;

                    var numericQuery = NumericRangeQuery.NewLongRange(searchCriteria.Field.ToString(), int.MaxValue, long.Parse(SearchFrom.ToString("yyyyMMddHHmmss")),
                        long.Parse(SearchTo.ToString("yyyyMMddHHmmss")), true,
                        (searchCriteria.Condition == SearchCondition.Before) ? true : false);//To avoid inclusive of given date for before condition in searching

                    if (matchCondition == MatchCondition.MatchAll)
                        _tempQuery.Add(numericQuery, BooleanClause.Occur.MUST);
                    else
                        _tempQuery.Add(numericQuery, BooleanClause.Occur.SHOULD);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while creating Numeric Query :" + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Gets the slider search results.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <returns></returns>
        //public DataTable GetSliderSearchResults(DateTime startDate)
        //{
        //    DataTable tempTable = new DataTable();
        //    try
        //    {
        //        if (_contactsMetaDataTable != null)
        //        {
        //            tempTable = _contactsMetaDataTable.Clone();
        //            BooleanQuery mainQuery = new BooleanQuery();
        //            BooleanQuery.SetMaxClauseCount(0x2710);
        //            IndexSearcher MyIndexSearcher = new IndexSearcher(_directory);
        //            DateTime SearchTo = startDate;

        //            var numericQuery = NumericRangeQuery.NewLongRange("StartDate", int.MaxValue, long.Parse(SearchTo.ToString("yyyyMMddHHmmss")),
        //                    long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), true, false);//To avoid inclusive of given date for before condition in searching

        //            mainQuery.Add(numericQuery, BooleanClause.Occur.SHOULD);

        //            Hits hits = MyIndexSearcher.Search(mainQuery);
        //            for (int i = 0; i < hits.Length(); i++)
        //            {
        //                Document doc = hits.Doc(i);
        //                if (doc != null)
        //                {
        //                    DataRow row = tempTable.NewRow();
        //                    foreach (DataColumn clm in tempTable.Columns)
        //                    {
        //                        if (_contactAttributeDateFields.Contains(clm.ColumnName))
        //                        {
        //                            row[clm] = DateTime.ParseExact(doc.GetField(clm.ColumnName).StringValue(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
        //                                DateTimeStyles.None).ToString();
        //                            //row[clm] = Convert.ToDateTime(doc.GetField(clm.ColumnName).StringValue().ToString("yyyyMMddHHmmss").ToString();
        //                        }
        //                        else
        //                            row[clm] = doc.GetField(clm.ColumnName).StringValue();
        //                    }
        //                    tempTable.Rows.Add(row);
        //                    //Datacontext.GetInstance().result += "StartDate : " + doc.GetField("StartDate").StringValue() + " EndDate : " + doc.GetField("EndDate").StringValue();
        //                    //Datacontext.GetInstance().result += "\n";
        //                }
        //            }
        //            return tempTable;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Error in GetSliderSearchResults :" + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
        //    }
        //    return null;
        //}

        /// <summary>
        /// Tokens the automatic query.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="stoken">The stoken.</param>
        /// <param name="outputQuery">The output query.</param>
        private void TokenToQuery(string fieldName, string stoken, string searchCriteria, ref BooleanQuery outputQuery)
        {
            if (!stoken.Contains("*"))
            {
                if (searchCriteria.Contains(SearchCondition.StartsWith.ToString()))
                    stoken = stoken + "*";
                else if (searchCriteria.Contains(SearchCondition.Contains.ToString()))
                    stoken = "*" + stoken + "*";
            }
            Term term = new Term(fieldName, stoken);
            WildcardQuery query = new WildcardQuery(term);
            outputQuery.Add(query, BooleanClause.Occur.SHOULD);
        }

        #region Old GetAdvancedSearchResults

        ///// <summary>
        ///// Gets the search results.
        ///// </summary>
        ///// <param name="searchCriterias">The search criterias.</param>
        ///// <param name="matchCondition">The match condition.</param>
        ///// <returns></returns>
        //public DataTable GetAdvancedSearchResults(List<Criteria> searchCriterias, MatchCondition matchCondition)
        //{
        //    DataTable tempTable = new DataTable();
        //    try
        //    {
        //        tempTable = _contactsMetaDataTable.Clone();
        //        IndexSearcher MyIndexSearcher = new IndexSearcher(_directory);
        //        BooleanQuery.SetMaxClauseCount(0x2710);
        //        Query mainQuery = this.GetParsedQuery(searchCriterias, matchCondition);
        //        Hits hits = MyIndexSearcher.Search(mainQuery);
        //        for (int i = 0; i < hits.Length(); i++)
        //        {
        //            Document doc = hits.Doc(i);
        //            if (doc != null)
        //            {
        //                DataRow row = tempTable.NewRow();
        //                foreach (DataColumn clm in tempTable.Columns)
        //                {
        //                    if (_contactAttributeDateFields.Contains(clm.ColumnName))
        //                    {
        //                        row[clm] = DateTime.ParseExact(doc.GetField(clm.ColumnName).StringValue(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
        //                            DateTimeStyles.None).ToString();
        //                        //row[clm] = Convert.ToDateTime(doc.GetField(clm.ColumnName).StringValue().ToString("yyyyMMddHHmmss").ToString();
        //                    }
        //                    else
        //                        row[clm] = doc.GetField(clm.ColumnName).StringValue();
        //                }
        //                tempTable.Rows.Add(row);
        //                //Datacontext.GetInstance().result += "StartDate : " + doc.GetField("StartDate").StringValue() + " EndDate : " + doc.GetField("EndDate").StringValue();
        //                //Datacontext.GetInstance().result += "\n";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Error while getting advance search results : " + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
        //        return null;
        //    }
        //    return tempTable;
        //}
        
        #endregion

        public DataTable GetAdvancedSearchResults(bool isContactDirectorytSearch, string mediaType, DateTime startDate, List<Criteria> searchCriterias, MatchCondition matchCondition)
        {
            DataTable tempTable = new DataTable();
            try
            {
                tempTable = _contactsMetaDataTable.Clone();
                IndexSearcher MyIndexSearcher = new IndexSearcher(IndexReader.Open(_directory, true));
                BooleanQuery mainQuery = new BooleanQuery();
                BooleanQuery.SetMaxClauseCount(0x2710);
                DateTime chkDate;
                if (DateTime.TryParse(startDate.ToString(), out chkDate))
                {
                    if (chkDate != null)
                    {
                        _logger.Debug("Search Field is DateTime value");
                        var numericQuery = NumericRangeQuery.NewLongRange("StartDate", int.MaxValue, long.Parse(startDate.ToString("yyyyMMddHHmmss")),
                             long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), true, false);//To avoid inclusive of given date for before condition in searching
                        if (!isContactDirectorytSearch)
                                mainQuery.Add(numericQuery, BooleanClause.Occur.MUST);
                    }
                }
                if (_contactsMetaDataTable.Columns.Contains("Media"))
                {
                    if (mediaType != string.Empty)
                    {
                        if (mediaType.ToLower() != "all")
                        {
                            Lucene.Net.Analysis.Token token = null;
                            Lucene.Net.Analysis.Token token2 = null;
                            TokenStream stream = _analyzer.TokenStream("Media", new StringReader(mediaType));
                            BooleanQuery tempQuery = new BooleanQuery();
                            do
                            {
                                token2 = token;
                                token = stream.Next();
                                if (token2 != null)
                                {
                                    string stoken = token2.TermText();
                                    BooleanQuery outputQuery = new BooleanQuery();
                                    this.TokenToQuery("Media", stoken, SearchCondition.Contains.ToString(),  ref outputQuery);
                                    tempQuery.Add(outputQuery, BooleanClause.Occur.SHOULD);
                                }
                            }
                            while (token != null);
                            mainQuery.Add(tempQuery, BooleanClause.Occur.MUST);
                        }
                    }
                }
                Query searchCriteriaQuery = this.GetParsedQuery(searchCriterias, matchCondition);
                mainQuery.Add(searchCriteriaQuery, BooleanClause.Occur.MUST);

                Hits hits = MyIndexSearcher.Search(mainQuery);
                for (int i = 0; i < hits.Length(); i++)
                {
                    Document doc = hits.Doc(i);
                    if (doc != null)
                    {
                        DataRow row = tempTable.NewRow();
                        foreach (DataColumn clm in tempTable.Columns)
                        {
                            System.Type columnType = clm.DataType;
                            if (_contactAttributeDateFields.Contains(clm.ColumnName) || columnType==typeof(DateTime))
                            {
                                if (!string.IsNullOrEmpty(doc.GetField(clm.ColumnName).StringValue()))
                                {
                                    row[clm] = DateTime.ParseExact(doc.GetField(clm.ColumnName).StringValue(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None).ToString();
                                }
                                else
                                    row[clm] = DBNull.Value;
                            }
                            else
                                row[clm] = doc.GetField(clm.ColumnName).StringValue();
                        }
                        tempTable.Rows.Add(row);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while getting GetAdvancedSearchResults : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
                return null;
            }
            return tempTable;
        }



        #region Old Quick Search Result

        ///// <summary>
        ///// Gets the quick search results.
        ///// </summary>
        ///// <param name="searchText">The search text.</param>
        ///// <returns></returns>
        //public DataTable GetQuickSearchResults(string searchText)
        //{
        //    DataTable tempTable = new DataTable();
        //    try
        //    {
        //        tempTable = _contactsMetaDataTable.Clone();
        //        IndexSearcher MyIndexSearcher = new IndexSearcher(_directory);
        //        BooleanQuery.SetMaxClauseCount(0x2710);
        //        Query mainQuery = this.GetQuickSearchStringQuery(searchText);
        //        Hits hits = MyIndexSearcher.Search(mainQuery);
        //        for (int i = 0; i < hits.Length(); i++)
        //        {
        //            Document doc = hits.Doc(i);
        //            if (doc != null)
        //            {
        //                DataRow row = tempTable.NewRow();
        //                foreach (DataColumn clm in tempTable.Columns)
        //                {
        //                    if (_contactAttributeDateFields.Contains(clm.ColumnName))
        //                    {
        //                        row[clm] = DateTime.ParseExact(doc.GetField(clm.ColumnName).StringValue(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
        //                            DateTimeStyles.None).ToString();
        //                    }
        //                    else
        //                        row[clm] = doc.GetField(clm.ColumnName).StringValue();
        //                }
        //                tempTable.Rows.Add(row);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Error while getting quick search results : " + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
        //        return null;
        //    }
        //    return tempTable;
        //}
        
        #endregion

        public DataTable GetQuickSearchResults(bool isContactDirectorytSearch, string mediaType, DateTime startDate, string searchText)
        {
            DataTable tempTable = new DataTable();
            try
            {
                if (_contactsMetaDataTable == null) return null;

                tempTable = _contactsMetaDataTable.Clone();
                IndexSearcher MyIndexSearcher = new IndexSearcher(_directory);
                BooleanQuery mainQuery = new BooleanQuery();
                BooleanQuery.SetMaxClauseCount(0x2710);
                DateTime chkDate;
                if (DateTime.TryParse(startDate.ToString(), out chkDate))
                {
                    if (chkDate != null)
                    {
                        _logger.Debug("Search Field is DateTime value");
                        var numericQuery = NumericRangeQuery.NewLongRange("StartDate", int.MaxValue, long.Parse(startDate.ToString("yyyyMMddHHmmss")),
                             long.Parse(DateTime.Today.AddDays(1).AddTicks(-1).ToString("yyyyMMddHHmmss")), true, false);//To avoid inclusive of given date for before condition in searching
                        if (!isContactDirectorytSearch)
                             mainQuery.Add(numericQuery, BooleanClause.Occur.MUST);
                    }
                }
                if (searchText != string.Empty)
                {
                    if (DateTime.TryParse(searchText.ToString(), out chkDate))
                    {
                        if (chkDate != null)
                        {
                            BooleanQuery dateQuery = new BooleanQuery();
                            var numericStartDateQuery = NumericRangeQuery.NewLongRange("StartDate", int.MaxValue, long.Parse(chkDate.ToString("yyyyMMddHHmmss")),
                            long.Parse(chkDate.AddDays(1).AddTicks(-1).ToString("yyyyMMddHHmmss")), true, false);//To avoid inclusive of given date for before condition in searching

                            dateQuery.Add(numericStartDateQuery, BooleanClause.Occur.SHOULD);

                            var numericEndDateQuery = NumericRangeQuery.NewLongRange("EndDate", int.MaxValue, long.Parse(chkDate.ToString("yyyyMMddHHmmss")),
                           long.Parse(chkDate.AddDays(1).AddTicks(-1).ToString("yyyyMMddHHmmss")), true, false);//To avoid inclusive of given date for before condition in searching

                           //dateQuery.Add(numericEndDateQuery, BooleanClause.Occur.SHOULD);

                            //mainQuery = new BooleanQuery();
                            mainQuery.Add(numericStartDateQuery, BooleanClause.Occur.MUST);
                            mainQuery.Add(numericEndDateQuery, BooleanClause.Occur.MUST);

                            Query subQuery = this.GetQuickSearchStringQuery(searchText);
                           // mainQuery.Add(subQuery, BooleanClause.Occur.MUST);
                        }
                    }
                    else
                    {
                        Query subQuery = this.GetQuickSearchStringQuery(searchText);
                        mainQuery.Add(subQuery, BooleanClause.Occur.MUST);
                    }
                }
                if (_contactsMetaDataTable.Columns.Contains("Media"))
                {
                    if (mediaType != string.Empty)
                    {
                        if (mediaType.ToLower() != "all")
                        {
                            Lucene.Net.Analysis.Token token = null;
                            Lucene.Net.Analysis.Token token2 = null;
                            TokenStream stream = _analyzer.TokenStream("Media", new StringReader(mediaType));
                            BooleanQuery tempQuery = new BooleanQuery();
                            do
                            {
                                token2 = token;
                                token = stream.Next();
                                if (token2 != null)
                                {
                                    string stoken = token2.TermText();
                                    BooleanQuery outputQuery = new BooleanQuery();
                                    this.TokenToQuery("Media", stoken, SearchCondition.Contains.ToString(), ref outputQuery);
                                    tempQuery.Add(outputQuery, BooleanClause.Occur.SHOULD);
                                }
                            }
                            while (token != null);
                            mainQuery.Add(tempQuery, BooleanClause.Occur.MUST);
                        }
                    }
                }
                Hits hits = MyIndexSearcher.Search(mainQuery);
                for (int i = 0; i < hits.Length(); i++)
                {
                    Document doc = hits.Doc(i);
                    if (doc != null)
                    {
                        DataRow row = tempTable.NewRow();
                        foreach (DataColumn clm in tempTable.Columns)
                        {
                            System.Type columnType = clm.DataType;
                            if (_contactAttributeDateFields.Contains(clm.ColumnName) || columnType==typeof(DateTime))
                            {
                                if (!string.IsNullOrEmpty(doc.GetField(clm.ColumnName).StringValue()))
                                {
                                    row[clm] = DateTime.ParseExact(doc.GetField(clm.ColumnName).StringValue(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None).ToString();
                                }
                                else
                                    row[clm] = DBNull.Value;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(doc.GetField(clm.ColumnName).StringValue()))
                                    row[clm] = doc.GetField(clm.ColumnName).StringValue();
                            }
                        }
                        tempTable.Rows.Add(row);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while getting GetQuickSearchResults : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
                return null;
            }
            return tempTable;
        }

        /// <summary>
        /// Gets the quick search string query.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        private Query GetQuickSearchStringQuery(string searchText)
        {
            try
            {
                BooleanQuery query = new BooleanQuery();
                BooleanQuery.SetMaxClauseCount(0x2710);
                if (searchText.Length > 0)
                {
                    BooleanQuery subQuery = new BooleanQuery();
                   // foreach (string fieldName in _contactsMetaDataTable.Columns)
                    //foreach (DataColumn column in _contactsMetaDataTable.Columns)
                    //{
                    //    if (!_contactAttributeDateFields.Contains(column.ColumnName))
                    //    {
                    //        QueryParser queryParser = new QueryParser(column.ColumnName, _analyzer);
                    //        subQuery.Add(queryParser.Parse(searchText), BooleanClause.Occur.SHOULD);
                    //    }
                    //}
                    //query.Add(subQuery,BooleanClause.Occur.SHOULD);
                    //Lucene.Net.Analysis.Token token = null;
                    foreach (DataColumn column in _contactsMetaDataTable.Columns)
                    {
                        if (!_contactAttributeDateFields.Contains(column.ColumnName) && !column.ColumnName.ToLower().Contains("media") && !column.ColumnName.ToLower().Contains("callvisibility") && !column.ColumnName.ToLower().Contains("emailvisibility") && !column.ColumnName.ToLower().Contains("contactid"))
                            //&& (column.ColumnName.ToLower().Contains("status") ||
                            //column.ColumnName.ToLower().Contains("subject"))) //this check is made to avoid showing search criteria including media column for keywords chat, mail
                        {
                            //if (!searchText.Contains("*"))
                            //{
                            //    searchText = searchText + "*";
                            //}
                            //Term term = new Term(column.ColumnName, searchText);
                            //WildcardQuery wildCardQuery = new WildcardQuery(term);
                            //query.Add(wildCardQuery, BooleanClause.Occur.SHOULD);

                            Lucene.Net.Analysis.Token token = null;
                            Lucene.Net.Analysis.Token token2 = null;
                            TokenStream stream = _analyzer.TokenStream(column.ColumnName, new StringReader(searchText));
                            BooleanQuery tempQuery = new BooleanQuery();
                            do
                            {
                                token2 = token;
                                token = stream.Next();
                                if (token2 != null)
                                {
                                    string stoken = token2.TermText();
                                    BooleanQuery outputQuery = new BooleanQuery();
                                    this.TokenToQuery(column.ColumnName, stoken, SearchCondition.Contains.ToString(), ref outputQuery);
                                    tempQuery.Add(outputQuery, BooleanClause.Occur.SHOULD);
                                }
                            }
                            while (token != null);
                            query.Add(tempQuery, BooleanClause.Occur.SHOULD);


                        }
                    }
                    return query;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("Error while creating String Query :" + ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the term query.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <param name="SearchFrom">The search from.</param>
        /// <param name="matchCondition">The match condition.</param>
        private void GetTermQuery(Criteria searchCriteria, DateTime SearchFrom, MatchCondition matchCondition)
        {
            DateTime SearchTo = SearchFrom.AddDays(1).AddTicks(-1);
            //Query termQuery = new TermQuery(new Term(searchCriteria.Field.ToString(),
            //    Lucene.Net.Util.NumericUtils.LongToPrefixCoded(long.Parse(SearchFrom.ToString("yyyyMMddHHmmss")))));
            var numericQuery = NumericRangeQuery.NewLongRange(searchCriteria.Field.ToString(), int.MaxValue, long.Parse(SearchFrom.ToString("yyyyMMddHHmmss")),
                  long.Parse(SearchTo.ToString("yyyyMMddHHmmss")), true,
                  (searchCriteria.Condition == SearchCondition.Before) ? true : false);//To avoid inclusive of given date for before condition in searching

            if (matchCondition == MatchCondition.MatchAll)
                _tempQuery.Add(numericQuery, BooleanClause.Occur.MUST);
            else
                _tempQuery.Add(numericQuery, BooleanClause.Occur.SHOULD);
        }

        /// <summary>
        /// Loads the initial data.
        /// </summary>
        /// <param name="MetaDataTable">The meta data table.</param>
        public void LoadInitialData(DataTable MetaDataTable)
        {
            try
            {
                _contactsMetaDataTable = MetaDataTable;
                if (_writer != null)
                    _writer = null;
                 if(_analyzer != null)
                     _analyzer = null;
                 if (_directory != null)
                 {
                     _directory.Close();
                     _directory = null;
                 }
                
                _analyzer = new StandardAnalyzer(Version.LUCENE_29);
                _directory =  new RAMDirectory();
                _writer = new IndexWriter(_directory, _analyzer, true);
                _writer.SetMergeScheduler(new SerialMergeScheduler());
                int count = 1;
                DateTime date;
                foreach (DataRow dtRow in _contactsMetaDataTable.Rows)
                {
                    Document _document = new Document();

                    foreach (DataColumn column in _contactsMetaDataTable.Columns)
                    {
                        try
                        {
                            DateTime chkDate;
                            if (DateTime.TryParse(dtRow[column.ToString()].ToString(), out chkDate))
                            {
                                if (chkDate != null)
                                {
                                    if (!_contactAttributeDateFields.Contains(column.ColumnName))
                                        _contactAttributeDateFields.Add(column.ColumnName);
                                    var numericField = new NumericField(column.ColumnName, count, Field.Store.YES, true);
                                    date = Convert.ToDateTime(dtRow[column.ToString()].ToString());
                                    numericField.SetLongValue(long.Parse(date.ToString("yyyyMMddHHmmss")));
                                    _document.Add(numericField);
                                    count++;
                                }
                            }
                            else
                            {
                                if (_contactAttributeDateFields.Contains(column.ColumnName) && !string.IsNullOrEmpty(dtRow[column.ToString()].ToString()))
                                    _contactAttributeDateFields.Remove(column.ColumnName);
                                Field stringField = new Field(column.ColumnName, dtRow[column.ToString()].ToString(), Field.Store.YES, Field.Index.ANALYZED);
                                _document.Add(stringField);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    #region old

                    //var numericStartDate = new NumericField("StartDate", count, Field.Store.YES, true);
                    //date = Convert.ToDateTime(dtRow["StartDate"].ToString());
                    //numericStartDate.SetIntValue(int.Parse(date.ToString("yyyyMMddHHmmss")));

                    //var numericEndDate = new NumericField("EndDate", count, Field.Store.YES, true);
                    //date = Convert.ToDateTime(dtRow["EndDate"].ToString());
                    //numericEndDate.SetIntValue(int.Parse(date.ToString("yyyyMMddHHmmss")));

                    //Field subject = new Field("Subject", dtRow["Subject"].ToString(), Field.Store.YES, Field.Index.ANALYZED);

                    //Field mediaType = new Field("MediaType", dtRow["MediaType"].ToString(), Field.Store.YES, Field.Index.ANALYZED);

                    //Field interactionID = new Field("InteractionId", dtRow["InteractionId"].ToString(), Field.Store.YES, Field.Index.ANALYZED);

                    //Field contactId = new Field("ContactId", dtRow["ContactId"].ToString(), Field.Store.YES, Field.Index.ANALYZED);

                    //Field status = new Field("Status", dtRow["Status"].ToString(), Field.Store.YES, Field.Index.ANALYZED);

                    //_document.Add(subject);
                    //_document.Add(mediaType);
                    //_document.Add(interactionID);
                    //_document.Add(contactId);
                    //_document.Add(status);
                    //_document.Add(numericStartDate);
                    //_document.Add(numericEndDate);

                    #endregion
                    _writer.AddDocument(_document);
                }
                _writer.Commit();
                _writer.Close();
            }
            catch (Exception ex)
            {
                _logger.Error("Error while adding Initial Data : " + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        #endregion
    }
}
