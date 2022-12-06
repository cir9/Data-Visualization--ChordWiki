
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



    public class ChordFile
    {
        static readonly Regex reg_chord_bracket = new(@"\[(.*?)\]");
        static readonly Regex reg_info_bracket = new(@"{.*?}", RegexOptions.IgnoreCase);
        static readonly Regex reg_title = new(@"{(?:title|t):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_subtitle = new(@"{(?:subtitle|st):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_comment = new(@"{(?:ci?|comment(?:_italic)):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_key = new(@"key:\s?([#♯b♭]?)([a-g])([#♯b♭]?)\s?(m(?:in|inor)?|)", RegexOptions.IgnoreCase); static readonly Regex reg_sharp = new(@"[#♯-]");
        static readonly Regex reg_flat = new(@"[b♭+]");

        static readonly Regex reg_measure = new(@"(\d+\/\d+)");
        static readonly Regex reg_bpm = new(@"bpm.??[=≈≒:].??約?([\d.]+)(?!\/)", RegexOptions.IgnoreCase);


        public string title = "";
        public string subtitle = "";
        public float bpm = 0f;
        public string measure = "";
        public Note key = Note.Unknown;
        public List<ChordName> chords = new();

        public bool IsKeyUnknown { get => key.IsUnknown; }

        public ChordFile(string file)
        {
            string text = File.ReadAllText(file);

            Note currentKey = Note.Unknown;

            string[] lines = text.Split('\n');
            bool isPreviousLineEmpty = true;

            foreach (var line in lines) {

                string info = reg_info_bracket.GetMatchAt(line, 0);
                if (info.Length > 0) {

                    string g_title = reg_title.GetMatchAt(info, 1);
                    if (g_title.Length > 0) title = g_title;

                    string g_subtitle = reg_subtitle.GetMatchAt(info, 1);
                    if (g_subtitle.Length > 0) subtitle = g_subtitle;

                    string comment = reg_comment.GetMatchAt(info, 1);
                    if (comment.Length > 0) {
                        comment = comment.ToDBC();

                        string g_bpm = reg_bpm.GetMatchAt(comment, 1);
                        if (g_bpm.Length > 0) bpm = float.Parse(g_bpm);

                        string g_measure = reg_measure.GetMatchAt(comment, 1);
                        if (g_measure.Length > 0) measure = g_measure;
                    }

                    var key_match = reg_key.Match(info);
                    if (key_match.Success) {
                        var groups = key_match.Groups;

                        string notation = groups[1].Value + groups[3].Value;
                        string keyName = groups[2].Value;
                        string minor = groups[4].Value;

                        int sharpCount = reg_sharp.GetCount(notation);
                        int flatCount = reg_flat.GetCount(notation);
                        bool isMinor = minor.Length > 0;

                        Note newKey = new() {
                            name = keyName.ToUpper().ToEnum<NoteName>(),
                            tune = sharpCount - flatCount,
                        };

                        if (isMinor) newKey = newKey.MinorToMajor();

                        currentKey = newKey;
                        if (key.IsUnknown) key = currentKey;
                    }


                    continue;
                }

                var matches = reg_chord_bracket.Matches(line);
                if (matches.Count != 0) {
                    if (isPreviousLineEmpty)
                        chords.Add(ChordName.NewParagraph);
                    else
                        chords.Add(ChordName.NewLine);
                }


                foreach (Match match in matches.Cast<Match>()) {
                    string chordName = match.Groups[1].Value;

                    if (currentKey.IsUnknown) return;

                    var parsedChords = ChordNameParse.ParseChordText(chordName);
                    parsedChords = parsedChords.Select(e => e.ToRelativeKey(currentKey));

                    //foreach (var chord in parsedChords) {
                    //    if (chord.isOpenSlashChord && chords.Count > 0) {
                    //        ChordName lastChord = chords.Last();
                    //        if (lastChord.IsValidChord) {
                    //            lastChord.bass = chord.bass ;
                    //            chords.Add(chord);
                    //            continue;
                    //        } 
                    //    }

                    //    chords.Add(chord);
                    //}

                    chords.AddRange(parsedChords);
                }

                isPreviousLineEmpty = matches.Count == 0;
            }

        }

    }



    public static class Preprocess
    {


        public static void CalculateStatistics(string dataPath)
        {
            Dictionary<string, int> chordCounts = new();

            string[] files = Directory.GetFiles(dataPath);
            int fileCount = files.Length;

            int i = 0;
            using var writer_data = new StreamWriter(dataPath + "/../music_db.csv", false, Encoding.UTF8);
            using (var csv = new CsvWriter(writer_data, CultureInfo.InvariantCulture)) {

                csv.WriteField("TITLE");
                csv.WriteField("SUBTITLE");
                csv.WriteField("BPM");
                csv.WriteField("MEASURE");
                csv.WriteField("KEY");
                csv.WriteField("CHORDS");
                csv.NextRecord();

                foreach (string file in files) {

                    Console.Write($"[#{i,6} / {fileCount,6}] Reading ...");

                    ChordFile chordFile = new(file);

                    if (chordFile.IsKeyUnknown) {
                        Console.Write("Key Unknown\n");
                        continue;
                    }

                    var chords = chordFile.chords;

                    foreach (var chord in chords) {
                        string chordName = chord.ToStandardSymbol();
                        int count = 0;
                        chordCounts.TryGetValue(chordName, out count);
                        chordCounts[chordName] = count + 1;
                    }

                    csv.WriteField(chordFile.title);
                    csv.WriteField(chordFile.subtitle);
                    csv.WriteField(chordFile.bpm);
                    csv.WriteField(chordFile.measure);
                    csv.WriteField(chordFile.key);
                    csv.WriteField(string.Join(' ', chordFile.chords.Select(e=>e.ToString())));

                    csv.NextRecord();


                    i++;

                    Console.Write("OK\n");
                }

            }

            using var writer = new StreamWriter(dataPath + "/../chord_counts.csv", false, Encoding.UTF8);
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {

                csv.WriteField("CHORD");
                csv.WriteField("COUNT");
                csv.NextRecord();

                foreach (var kv in chordCounts.OrderBy(e => -e.Value)) {

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
