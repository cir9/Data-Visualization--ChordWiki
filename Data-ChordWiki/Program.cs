
using System.Xml;
using System.Web;
using System.Net;
using HtmlAgilityPack;



namespace Data_ChordWiki
{
    internal class Program
    {

        static readonly string dataPath = new DirectoryInfo(@".\..\..\..\..\data\").FullName;
        static readonly string retrievedFileDir = dataPath + @".\pages\";
        static readonly string editPageUrl = @"https://ja.chordwiki.org/wiki.cgi?c=edit&t=";


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







        static void Main(string[] args)
        {
            Directory.CreateDirectory(dataPath);
            Directory.CreateDirectory(retrievedFileDir);

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
    }
}