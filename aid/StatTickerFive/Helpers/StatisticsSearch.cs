using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Pointel.Statistics.Core;

using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace StatTickerFive.Helpers
{
    public class StatisticsSearch
    {
        IndexWriter writer;
        Lucene.Net.Store.Directory LuceneDirectory;
        Analyzer analyzer;
        StatisticsBase statBase = new StatisticsBase();

        public StatisticsSearch()
        {
            LuceneDirectory = new RAMDirectory();// FSDirectory.GetDirectory("LuceneIndex5");
            analyzer = new StandardAnalyzer();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            writer = new IndexWriter(LuceneDirectory, analyzer, true);
            writer.SetMergeScheduler(new SerialMergeScheduler());

            Settings.GetInstance().LstAgentList = statBase.ReadAllAgents();
            Settings.GetInstance().LstAgentGroupList = statBase.ReadAllAgentGroups();

            foreach (string names in Settings.GetInstance().LstAgentList)
            {
                //string Dilimitor = "*&";
                //string[] name = names.Split(new[] { Dilimitor }, StringSplitOptions.None);
                AddRecord(names, "Users");
            }

            foreach (string names in Settings.GetInstance().LstAgentGroupList)
            {
                AddRecord(names, "Users");
            }

            writer.Commit();
        }

        private void AddRecord(string record, string type,string Uid ="")
        {
            try
            {
                string Dilimitor = "*&";
                string[] name = record.Split(new[] { Dilimitor }, StringSplitOptions.None);
                Document document = new Document();
                //document.Add(new Field("Uid", Uid, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("Name", name[0], Field.Store.YES, Field.Index.ANALYZED));
                if(name.Length>1)
                document.Add(new Field("Id", name[1], Field.Store.YES, Field.Index.ANALYZED));
                else
                    document.Add(new Field("Id", name[0], Field.Store.YES, Field.Index.ANALYZED));

                document.Add(new Field("UserType", type, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(document);
            }
            catch (Exception ex)
            {
                
            }
        }

        public List<string> Search(string text)
        {
            List<string> lstFilteredValue = new List<string>();
            try
            {
                IndexSearcher MyIndexSearcher = new IndexSearcher(LuceneDirectory);

                Query mainQuery = this.GetParsedQuerywc(text);
                //Do the search
                Hits hits = MyIndexSearcher.Search(mainQuery);

                int results = hits.Length();

                for (int i = 0; i < results; i++)
                {
                    Document doc = hits.Doc(i);
                    float score = hits.Score(i);
                    lstFilteredValue.Add(doc.Get("Name")+","+doc.Get("Id"));
                }
            }
            catch (Exception GeneralException)
            {

            }
            return lstFilteredValue;
        }

        private Query GetParsedQuerywc(string text)
        {
            BooleanQuery query = new BooleanQuery();
            BooleanQuery.SetMaxClauseCount(0x2710);
            if (text.Length > 0)
            {
                BooleanQuery query2 = new BooleanQuery();
                QueryParser parser = new QueryParser("UserType", analyzer);
                query2.Add(parser.Parse("Users"), BooleanClause.Occur.SHOULD);
                query.Add(query2, BooleanClause.Occur.MUST);
            }
            Lucene.Net.Analysis.Token token = null;
            Lucene.Net.Analysis.Token token2 = null;
            TokenStream stream = analyzer.TokenStream("UserType", new StringReader(text));
            do
            {
                token2 = token;
                token = stream.Next();
                if (token2 != null)
                {
                    string stoken = token2.TermText();
                    BooleanQuery outputQuery = new BooleanQuery();
                    this.TokenToQuery("Name", stoken, ref outputQuery);
                    query.Add(outputQuery, BooleanClause.Occur.MUST);
                }
            }
            while (token != null);
            return query;
        }

        private void TokenToQuery(string fieldName, string stoken, ref BooleanQuery outputQuery)
        {
            try
            {
                if (!stoken.Contains("*"))
                {
                    stoken = stoken + "*";
                }
                Term term = new Term(fieldName, stoken);
                WildcardQuery query = new WildcardQuery(term);
                outputQuery.Add(query, BooleanClause.Occur.SHOULD);
            }
            catch (Exception GeneralException)
            {

            }
        }
    }
}
