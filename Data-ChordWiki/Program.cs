
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
            document.Load(dataPath + @".\sitemap.xml");

            var root = document.DocumentElement;
            Console.WriteLine("Pages to fetch: " + (root?.ChildNodes.Count ?? 0));
            Console.WriteLine();

            Console.Write("Fetch pages from: #");
            int count = int.Parse(Console.ReadLine() ?? "-1");




        }
    }
}