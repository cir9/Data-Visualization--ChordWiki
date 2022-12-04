
using System.Xml;





namespace Data_ChordWiki
{
    internal class Program
    {

        static readonly string dataPath = new DirectoryInfo(@".\..\..\..\..\data\").FullName;


        static void Main(string[] args)
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

                Console.WriteLine(url);
                Console.ReadLine();


            }
        }
    }
}