
using System.Xml;
using System.Web;
using System.Net;
using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace Data_ChordWiki
{
    internal class Program
    {


        static readonly string dataPath = new DirectoryInfo(@".\..\..\..\..\data\").FullName;
        static readonly string retrievedFileDir = dataPath + @".\pages\";
        static readonly string editPageUrl = @"https://ja.chordwiki.org/wiki.cgi?c=edit&t=";
        static readonly string rankingPageUrl = @"https://ja.chordwiki.org/wiki.cgi?c=ranking&m=";


        static bool FetchPage(string url)
        {
            Uri uri = new(url);
            string encodedMusicTitle = url.Split(@"/").Last();
            string musicTitle = HttpUtility.UrlDecode(encodedMusicTitle);

            Console.Write($"\"{musicTitle}\" ...");


            string html = editPageUrl + encodedMusicTitle;
            HtmlWeb web = new();
            HtmlDocument doc = web.Load(html);

            if (web.StatusCode != HttpStatusCode.OK) {
                Console.Write(web.StatusCode);
                return false;
            }

            var node = doc.DocumentNode.SelectSingleNode("//textarea");
            string chordProText = node.InnerText;

            string fileName = Utils.ParseStringToFileName(musicTitle) + ".txt";
            File.WriteAllText(retrievedFileDir + fileName, chordProText);

            Console.Write("OK\n");
            return true;

        }

        static void FetchPages()
        {

            Console.WriteLine("datapath = " + dataPath);
            Console.WriteLine();

            XmlDocument document = new XmlDocument();
            document?.Load(dataPath + @".\sitemap.xml");

            var root = document?.DocumentElement;
            XmlNodeList? nodes = root?.ChildNodes;
            int nodeCount = nodes?.Count ?? 0;

            Console.WriteLine("Pages to fetch: " + nodeCount);
            Console.WriteLine();

            Console.Write("Fetch pages from: #0");
            int startIndex = 0;
            int.TryParse(Console.ReadLine(), out startIndex);
            Console.WriteLine();

            if (startIndex < 0 || startIndex > nodeCount || nodeCount == 0) return;

            for (int i = startIndex; i < nodeCount; i++) {
                string url = nodes?[i]?.FirstChild?.InnerText ?? "";

                Console.Write($"[#{i,6} / {nodeCount,6}] Fetching ");
                //Console.WriteLine(url);
                if (!FetchPage(url)) {
                    i--;
                    Console.WriteLine($"Page #{i} fetching failed.");
                    Console.WriteLine();
                    Console.WriteLine("-- Press Enter to retry. --");
                    Console.ReadLine();
                }
                //Console.ReadLine();

            }
        }



        static void FetchRankings()
        {
            Console.Write("Month until to fetch: ");

            MonthTime until = new(Console.ReadLine() ?? "");


            using var writer = new StreamWriter(dataPath + "ranking.csv", false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            for (MonthTime m = new(1, 2012); m < until; m = m.Next()) {
                string str = $"{m.year:D4}{m.month:D2}";

                csv.WriteField(str);

                Console.Write($"[{str}] Fetching ranking ...");

                HtmlWeb web = new();
                HtmlDocument doc = web.Load(rankingPageUrl + str);

                if (web.StatusCode != HttpStatusCode.OK) {
                    Console.Write(web.StatusCode);
                }

                var nodes = doc.DocumentNode.SelectNodes("//table[@class=\"ranking\"]/tr/td[3]/a");

                foreach (var node in nodes) {
                    string url = node.GetAttributeValue("href", "");
                    Uri uri = new(url);
                    string encodedMusicTitle = url.Split(@"/").Last();
                    string musicTitle = HttpUtility.UrlDecode(encodedMusicTitle);
                    string fileName = Utils.ParseStringToFileName(musicTitle);
                    csv.WriteField(fileName);
                }

                csv.NextRecord();

                Console.Write("OK\n");
            }


        }




        static void Main(string[] args)
        {
            Directory.CreateDirectory(dataPath);
            Directory.CreateDirectory(retrievedFileDir);

            if (args[0] == "page") FetchPages();
            else if (args[0] == "ranking") FetchRankings();
            else if (args[0] == "preprocess_ranking") Preprocess.CalculateStatistics(retrievedFileDir); // Preprocess.RearrangeRanking(dataPath);
        }
    }
}