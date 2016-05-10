using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Models;
using Lucene.Net.QueryParsers;
using Lucene.Net.Repository;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new BookRepository();
            var path = GetIndexPath("books_index", true);
            var directory = GetIndexDirectory(path);
            var writer = GetIndexWriter(directory);

            foreach (var book in db.GetAll())
            {
                AddToIndex(book, writer);
            }

            writer.Commit();


            // ====================================== REALIZANDO BUSCAS ================================

            var reader = GetIndexReader(directory);
            var searcher = GetIndexSearcher(reader);

            var query = GetTermsQuery("Summary", "C#");

            //var query = GetTermsQuery("Summary", "upda?e*"); 

            //var query = GetTermsQuery("Title", "Adaptive");

            //var query = GetTermsQuery("Isbn", "9781449320102");            

            //var query = GetPhraseQuery("Summary", "Fully updated");

            //var query = GetRangeQuery("PageNum", 300, 800);

            //var query = GetMultiFieldTermsQuery(new []{"Title", "Summary"}, "Edition");

            var results = searcher.Search(query, 20);

            PrintDocs(results, reader, db); 
           

            Console.WriteLine("DESEJA EXCLUIR? sim ou não");
            if("sim".ToLower() == Console.ReadLine().ToLower())
            {
                writer.DeleteDocuments(query);
                Console.WriteLine("Before Commit");
                Console.WriteLine(String.Format("Total Docs: {0}", writer.NumDocs()));
                writer.Commit();
                Console.WriteLine("After Commit");
                Console.WriteLine(String.Format("Total Docs: {0}", writer.NumDocs()));
                Console.WriteLine("====================================");
                Console.WriteLine("Query Result Older Reader");
                results = searcher.Search(query, 20);
                Console.WriteLine(String.Format("Total Hits: {0}", results.TotalHits));

                reader = GetIndexReader(directory);
                searcher = GetIndexSearcher(reader);
                Console.WriteLine("Query Result New Reader");
                results = searcher.Search(query, 20);
                Console.WriteLine(String.Format("Total Hits: {0}", results.TotalHits));
            }

            Console.ReadKey();
        }

        private static void PrintDocs(TopDocs results, IndexReader reader, BookRepository db)
        {
            foreach (var scoreDoc in results.ScoreDocs)
            {
                Console.WriteLine(String.Format("Doc Id: {0}\n", scoreDoc.Doc));
                Console.WriteLine(String.Format("Doc Score: {0}\n", scoreDoc.Score));

                var doc = reader.Document(scoreDoc.Doc);
                var title = doc.Get("Title");
                Console.WriteLine(String.Format("Doc Title: {0}\n", title));

                var isbn = doc.Get("Isbn");
                var book = db.Get(isbn);
                Console.WriteLine(String.Format("Db Summary: {0}\n", book.Summary));
                Console.WriteLine(String.Format("Db Pages: {0}\n", book.PageNum));

                Console.WriteLine("=============================================");
            }
        }

        public static string GetIndexPath(string indexName, bool reset = false)
        {
            var indexPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, indexName);
            if (reset)
                if (System.IO.Directory.Exists(indexPath))
                    System.IO.Directory.Delete(indexPath, true);

            return indexPath;
        }

        public static Directory GetIndexDirectory(string indexPath)
        {
            var indexDirectory = FSDirectory.Open(new System.IO.DirectoryInfo(indexPath));
            return indexDirectory;
        }

        public static IndexWriter GetIndexWriter(Directory indexDirectory)
        {
            var analyzer = new StandardAnalyzer(Util.Version.LUCENE_30);
            var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            return writer;
        }

        public static void AddToIndex(Book book, IndexWriter writer)
        {
            var doc = new Document();
            doc.Add(new Field("Isbn", book.Isbn, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Summary", book.Summary, Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new NumericField("PageNum", Field.Store.NO, true).SetIntValue(book.PageNum));
            writer.AddDocument(doc);
        }

        public static IndexReader GetIndexReader(Directory indexDirectory)
        {
            var reader = IndexReader.Open(indexDirectory, true);
            return reader;
        }

        public static IndexSearcher GetIndexSearcher(IndexReader reader)
        {
            var searcher = new IndexSearcher(reader);
            return searcher;
        }

        //====================================== QUERY ==========================================

        public static Query GetTermsQuery(string fieldName, string terms)
        {
            var parser = new QueryParser(Util.Version.LUCENE_20, fieldName, new StandardAnalyzer(Util.Version.LUCENE_30));
            var termQuery = parser.Parse(terms);
            return termQuery;
        }

        public static Query GetMultiFieldTermsQuery(string[] fieldNames, string terms)
        {
            var parser = new MultiFieldQueryParser(Util.Version.LUCENE_30, fieldNames, new StandardAnalyzer(Util.Version.LUCENE_30));

            var multiQuery = parser.Parse(terms);
            return multiQuery;
        }

        public static Query GetPhraseQuery(string fieldName, string phrase)
        {
            var phraseQuery = new PhraseQuery();

            var analyzer = new StandardAnalyzer(Util.Version.LUCENE_30);
            var tokens = analyzer.TokenStream(fieldName, new System.IO.StringReader(phrase));
            while (tokens.IncrementToken())
            {
                var termAttribute = tokens.GetAttribute<ITermAttribute>();
                phraseQuery.Add(new Term(fieldName, termAttribute.Term));
            }
            return phraseQuery;
        }

        static Query GetRangeQuery(string fieldName, int? min, int? max)
        {
            var rangeQuery = NumericRangeQuery.NewIntRange(fieldName, 10, min, max, true, true);
            return rangeQuery;
        }

        // ====================================== FIM QUERY  =======================================

        
    }
}
