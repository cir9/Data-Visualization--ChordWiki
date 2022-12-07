
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


    public class KeyCalculator
    {

        public int minimumBatchSize = 16;
        public List<Chord> batch = new();
        public int[] semitoneMap = new int[12];

        public float totalScore = 0f;
        public int totalNotes = 0;

        private int keyNotes = 0;

        private Note _currentKey = Note.Unknown;
        public readonly Dictionary<Note, int> keyDistribution = new();

        public float AverageScore { get => totalScore / totalNotes; }

        public Note CurrentKey {
            get => _currentKey; 
            set { 
                totalScore += Note.CalculateKeyScore(_currentKey, semitoneMap);

                CountKeyNotes();

                semitoneMap = new int[12];

                _currentKey = value;
            }
        }

        public void ComputeScoreFor(IEnumerable<Chord> chords)
        {
            foreach (var chord in chords) {
                var comps = chord.GetComponents();
                foreach (int tune in comps) {
                    semitoneMap[tune]++;
                    totalNotes++;
                    keyNotes++;
                }
            }
        }

        public void EndCalculate()
        {
            totalScore += Note.CalculateKeyScore(_currentKey, semitoneMap);
            CountKeyNotes();
        }



        public void Store(IEnumerable<Chord> chords)
        {
            ComputeScoreFor(chords);
            batch.AddRange(chords);
        }
        private void CountKeyNotes()
        {
            if (keyNotes == 0) return;
            if (keyDistribution.TryGetValue(_currentKey, out int count)) {
                keyDistribution[_currentKey] = count + keyNotes;
            } else
                keyDistribution[_currentKey] = keyNotes;

            keyNotes = 0;
        }

        public IEnumerable<Chord> ForceFlush()
        {
            if (batch.Count == 0) return Enumerable.Empty<Chord>();

            _currentKey = Note.FitKey(semitoneMap, out float maxScore);
            totalScore += maxScore;
            CountKeyNotes();
            return Flush();
        }
        public IEnumerable<Chord> TryFlush()
        {
            if (batch.Count < minimumBatchSize) {
                return Enumerable.Empty<Chord>();
            }

            return ForceFlush();
        }


        private IEnumerable<Chord> Flush()
        {
            foreach (var chord in batch) {
                yield return chord.ToRelativeKey(_currentKey);
            }


            batch.Clear();
            semitoneMap = new int[12];
        }

    }



    public class ChordFile
    {
        static readonly Regex reg_chord_bracket = new(@"\[(.*?)\]");
        static readonly Regex reg_info_bracket = new(@"{.*?}", RegexOptions.IgnoreCase);
        static readonly Regex reg_title = new(@"{(?:title|t):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_subtitle = new(@"{(?:subtitle|st):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_comment = new(@"{(?:ci?|comment(?:_italic)):(.*?)}", RegexOptions.IgnoreCase);
        static readonly Regex reg_key = new(@"key:\s?([#♯b♭]?)([a-g])([#♯b♭]?)\s?(m(?:in|inor)?|)", RegexOptions.IgnoreCase); 
        static readonly Regex reg_sharp = new(@"[#♯-]");
        static readonly Regex reg_flat = new(@"[b♭+]");

        static readonly Regex reg_measure = new(@"(\d+\/\d+)");
        static readonly Regex reg_bpm = new(@"bpm.??[=≈≒:].??約?([\d.]+)(?!\/)", RegexOptions.IgnoreCase);


        public string title = "";
        public string subtitle = "";
        public float bpm = 0f;
        public string measure = "";
        public Note key = Note.Unknown;
        public Dictionary<Note, int> keyDistribution;
        public List<Chord> chords = new();
        public float averageScore = 0f;
        public int totalNotes = 0;

        public bool ContainsTranspose { get => keyDistribution.Count > 1; }
        public bool IsKeyUnknown { get => key.IsUnknown; }

        static readonly KeyValuePair<Note, int> defaultKV = new (Note.Unknown, 0);
        public Note MostLikelyKey { get => keyDistribution.OrderBy(e => e.Value).FirstOrDefault(defaultKV).Key; }
        public IEnumerable<KeyValuePair<Note, float>> PossibleKeys {
            get {
                if (totalNotes == 0) return Enumerable.Empty<KeyValuePair<Note, float>>();
                return keyDistribution.OrderBy(e => -e.Value)
                    .Select(e => new KeyValuePair<Note, float>(e.Key, e.Value * 1.0f / totalNotes));
            }
        }

        public ChordFile(string file)
        {
            string text = File.ReadAllText(file);

            //Note currentKey = Note.Unknown;

            string[] lines = text.Split('\n');
            bool isPreviousLineEmpty = true;


            KeyCalculator keyCalculator = new ();

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
                        int flatCount = reg_flat.GetCount(notation.ToLower());
                        bool isMinor = minor.Length > 0;

                        Note newKey = new() {
                            name = keyName.ToUpper().ToEnum<NoteName>(),
                            tune = sharpCount - flatCount,
                        };

                        if (isMinor) newKey = newKey.MinorToMajor();

                        if (key.IsUnknown) key = newKey;
                        keyCalculator.CurrentKey = newKey;
                    }

                    isPreviousLineEmpty = true;
                    continue;
                }

                var matches = reg_chord_bracket.Matches(line);
                if (matches.Count != 0) {
                    //if (key.IsUnknown) break;

                    if (isPreviousLineEmpty) {

                        //var rawChords = keyCalculator.TryFlush(out Note _);

                        chords.AddRange(keyCalculator.TryFlush());

                        if (key.IsUnknown)
                            keyCalculator.Store(new Chord[]{ Chord.NewParagraph});
                        else
                            chords.Add(Chord.NewParagraph);
                    } else { 

                        if (key.IsUnknown)
                            keyCalculator.Store(new Chord[]{ Chord.NewLine });
                        else                    
                            chords.Add(Chord.NewLine);
                    }
                }

                //if (key.IsUnknown && matches.Count > 0) break;

                foreach (Match match in matches.Cast<Match>()) {
                    string chordName = match.Groups[1].Value;

                    var parsedChords = ChordNameParse.ParseChordText(chordName);

                    if (key.IsUnknown) {

                        keyCalculator.Store(parsedChords);
                        //return;

                    } else {

                        parsedChords = parsedChords.Select(e => e.ToRelativeKey(keyCalculator.CurrentKey));
                        keyCalculator.ComputeScoreFor(parsedChords);

                        chords.AddRange(parsedChords);
                    }
                }


                isPreviousLineEmpty = matches.Count == 0;
            }


            chords.AddRange(keyCalculator.ForceFlush());
            keyCalculator.EndCalculate();

            totalNotes = keyCalculator.totalNotes;
            averageScore = keyCalculator.AverageScore;
            keyDistribution = keyCalculator.keyDistribution;
        }

    }



    public static class Preprocess
    {

        private readonly static int progressionLength = 4;

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
                            Console.Write("Key Unknown\n");
                            continue;
                        }

                        Console.Write($"Key Predicted: {mostLikelyKey, -2}, score = {chordFile.averageScore,6:F3} ...");

                        //continue;
                    };

                    var chords = chordFile.chords;


                    List<List<string>> progressionChords = new();
                    List<List<string>> newParaChords = new();
                    int spanCount = 4;
                    //string lastChordName = "";
                    foreach (var chord in chords) {
                        if (chord.IsMark) {
                            if (chord.isNewParagraph) {

                                newParaChords.Clear();
                                spanCount = 4;
                            }

                            if (spanCount >= 4) {
                                newParaChords.Add(new());
                                spanCount = 0;
                            }
                            continue;
                        }
                        //if (chord.IsMark) continue;

                        if (chord.IsProgressionChord) {
                            string simpleChordName = chord.ToSimpleChord().ToStandardSymbol();
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
                    progressionChords.RemoveAll(e => e[0] == e[^1]);

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
                    csv.WriteField(string.Join(';', chordFile.PossibleKeys.Select(e => $"{e.Key}:{e.Value}")));
                    //csv.WriteField(chordFile.averageScore);
                    csv.WriteField(string.Join(' ', chordFile.chords.Select(e => e.ToString())));
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
