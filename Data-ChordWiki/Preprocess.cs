
using System.Xml;
using System.Web;
using System.Net;
using CsvHelper;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using System.Collections.Generic;
using HtmlAgilityPack;
using CsvHelper.Configuration.Attributes;
using System.Text.RegularExpressions;

namespace Data_ChordWiki
{






    public static class Preprocess
    {
        static readonly Regex reg_chord_bracket = new Regex(@"\[(.*?)\]");
        static readonly Regex reg_key_title = new Regex(@"{title:(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_key_subtitle = new Regex(@"{subtitle:(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_key_c = new Regex(@"{c:(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_key_block = new Regex(@"{key:(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_chord_name = new Regex(
            @"([Nn]\.?[Cc]\.?)|(\b|\/|[#♯b♭]?[#♯xb♭♮ｷ])([a-gA-G])([#♯b♭]?[#♯xb♭♮ｷ]?)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|(?:[m-]|[Mm]in)(?:[MΔ]|[Mm]aj)|[Mm]aj(?:or)?|MAJ(?:OR)?|[Mm]in(?:or)?|MIN(?:OR)?|[Ii]on(?:ian)?|ION(?:IAN)?|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Mm]ix(?:o|olydian)?|MIX(?:O|OLYDIAN)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?|[Ll]oc(?:rian)?|LOC(?:RIAN)?|[m-]|[MΔ]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(sus|)(69|11|13|[796513]|)(sus[#♯b♭+-]?[24]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(69|11|13|[79651234]|)(\(?(?:[Aa][Dd][Dd])?(?:(?:[#♯b♭+-]?)(?:11|13|[79651234]))?(?:[\b\/](?:[#♯b♭+-]?)(?:11|13|[79651234]))*\)?)(\/([#♯b♭]?[#♯xb♭♮ｷ]?)([a-gA-G])([#♯b♭]?[#♯xb♭♮ｷ]?)|)(?!\w)"
        );



        public static void CalculateStatistics(string dataPath)
        {
            Dictionary<string, int> chordCounts = new();

            string[] files = Directory.GetFiles(dataPath);
            int fileCount = files.Length;

            int i = 0;
            foreach (string file in files) {

                Console.Write($"[#{i,6} / {fileCount,6}] Reading ...");
                string text = File.ReadAllText(file);

                foreach (Match match in reg_chord_bracket.Matches(text)) {
                    string chordName = match.Captures[1].Value;
                    int count = 0;
                    chordCounts.TryGetValue(chordName, out count);
                    chordCounts[chordName] = count + 1;

                }


                i++;

                Console.Write("OK\n");
            }


            using var writer = new StreamWriter(dataPath + "chord_counts.csv", false, Encoding.UTF8);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {

                foreach (var kv in chordCounts) {

                    csv.WriteField(kv.Key);
                    csv.WriteField(kv.Value);

                    csv.NextRecord();
                }

            }


        }




        public static void RearrangeRanking(string dataPath)
        {

            Console.Write("Preprocessing ranking data ...");


            Dictionary<string, List<string>> data = new();

            using var reader = new StreamReader(dataPath + "ranking.csv");

            using (var csv = new CsvParser(reader, CultureInfo.InvariantCulture)) {
                while (csv.Read()) {
                    var row = csv.Record;
                    if (row is null) break;

                    string currentMonth = row[0];

                    for (int i = 1; i < row.Length; i++) {
                        string record = row[i];
                        List<string>? list;

                        if (!data.TryGetValue(record, out list) || list is null) {
                            list = new List<string>();
                            data[record] = list;
                        }

                        list.Add($"{currentMonth}:{i}");
                    }
                }
            }

            using var writer = new StreamWriter(dataPath + "ranking_rearrange.csv", false, Encoding.UTF8);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {

                foreach (var kv in data) {

                    csv.WriteField(kv.Key);

                    foreach (string rank in kv.Value) {
                        csv.WriteField(rank);
                    }

                    csv.NextRecord();
                }

            }

            Console.Write("OK\n");



        }







    }
}
