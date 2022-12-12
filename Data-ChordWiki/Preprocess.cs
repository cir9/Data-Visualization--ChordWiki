
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
using System.Collections;
using System.Linq;

namespace Data_ChordWiki
{




    public static class Preprocess
    {

        private readonly static int progressionLength = 4;
        private readonly static int minSpanLength = 4;


        public static void CalculateStatistics(string dataPath)
        {
            Dictionary<string, int> chordCounts = new();

            string[] files = Directory.GetFiles(dataPath);
            int fileCount = files.Length;

            int i = 0;
            using var writer_data = new StreamWriter(dataPath + "/../music_db.csv", false, Encoding.UTF8);
            using (var csv = new CsvWriter(writer_data, CultureInfo.InvariantCulture)) {

                //csv.WriteField("FILENAME");
                //csv.WriteField("TITLE");
                //csv.WriteField("SUBTITLE");
                //csv.WriteField("BPM");
                //csv.WriteField("MEASURE");
                //csv.WriteField("KEY");
                //csv.WriteField("POSSIBLE KEYS");
                //csv.WriteField("CHORDS");
                //csv.WriteField("PROGRESSIONS");
                //csv.NextRecord();

                foreach (string file in files) {

                    Console.Write($"[#{i,6} / {fileCount,6}] Reading ...");

                    ChordFile chordFile = new(file);
                    if (chordFile.IsKeyUnknown) {
                        Note mostLikelyKey = chordFile.MostLikelyKey;

                        if (chordFile.ContainsTranspose || mostLikelyKey.IsUnknown) {
                            Console.Write("Key Unknown ...Skip\n");
                            continue;
                        }

                        Console.Write($"Key Predicted: {mostLikelyKey, -2}, score = {chordFile.averageScore,6:F3} ...");

                        //continue;
                    };

                    if (chordFile.containsBannedChord)
                    {
                        Console.Write($"Contains banned Chord ...Skip\n");
                        continue;
                    }

                    var chords = chordFile.chords;


                    List<List<string>> progressionChords = new();
                    List<List<string>> newParaChords = new();
                    List<string> allSimpleChords = new();
                    int spanCount = minSpanLength;
                    //string lastChordName = "";
                    foreach (var chord in chords) {
                        if (chord.IsMark) {
                            if (chord.isNewParagraph) {

                                newParaChords.Clear();
                                spanCount = minSpanLength;
                            }

                            if (spanCount >= minSpanLength) {
                                newParaChords.Add(new());
                                spanCount = 0;
                            }
                            continue;
                        }
                        //if (chord.IsMark) continue;

                        if (chord.IsProgressionChord) {
                            string simpleChordName = chord.ToStandardSymbol();

                            if (allSimpleChords.Count == 0 || allSimpleChords[^1] != simpleChordName) {
                                allSimpleChords.Add(simpleChordName);
                            }

                            foreach (var list in newParaChords) {
                                if (list.Count == 0 || list[^1] != simpleChordName) {

                                    list.Add(simpleChordName);
                                }
                            }
                            progressionChords.AddRange(newParaChords.Where(e => e.Count >= progressionLength));
                            newParaChords.RemoveAll(e => e.Count >= progressionLength);
                            spanCount++;
                        }


                        string chordName = chord.ToStandardSymbol();
                        int count = 0;
                        chordCounts.TryGetValue(chordName, out count);
                        chordCounts[chordName] = count + 1;
                    }

                    // remove repeat head-tail
                    // progressionChords.RemoveAll(e => e[0] == e[^1]);

                    //foreach (var list in progressionChords) {
                    //    if (list[0] == list[progressionLength - 1])
                    //        list[progressionLength - 1] = list[progressionLength - 2];
                    //}



                    csv.WriteField(file.Split('\\').Last().Split('.').First());
                    csv.WriteField(chordFile.title);
                    csv.WriteField(chordFile.subtitle);
                    csv.WriteField(chordFile.bpm);
                    csv.WriteField(chordFile.measure);
                    csv.WriteField(chordFile.key);
                    csv.WriteField(string.Join(';', chordFile.PossibleKeys.Select(e => $"{e.key}:{e.possibility}")));
                    //csv.WriteField(chordFile.averageScore);
                    csv.WriteField(string.Join(' ', chordFile.chords.Select(e => e.ToString())));
                    csv.WriteField(string.Join(' ', allSimpleChords));

                    csv.WriteField(
                        string.Join('|', progressionChords.Select(
                            e => string.Join(' ', e)
                        ))
                    );


                    csv.NextRecord();



                    Console.Write("OK\n");




                    i++;
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
