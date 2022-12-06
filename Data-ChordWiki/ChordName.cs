using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data_ChordWiki
{

    public struct Note
    {
        static readonly Dictionary<int, string> notationFromSemitone = new() {
            { -3, "bbb" },{ -2, "bb" }, { -1, "b" }, { 0, "" }, { 1, "#" }, { 2, "x" }, { 3, "x#" }
        };
        public NoteName name;
        public int tune;

        public bool IsUnknown { get => name == NoteName.Unknown; }

        public override string ToString()
        {
            if (name >= NoteName.I) return $"{notationFromSemitone.GetOrDefault(tune, "")}{name.ToString().Replace("_", "")}";
            return $"{name.ToString().Replace("_", "")}{notationFromSemitone.GetOrDefault(tune, "")}";
        }

        //public string ToNumberString()
        //{
        //    int scaleNumber = ((int)name) % 7;
        //    return $"{notationFromSemitone.GetOrDefault(tune, "")}{scaleNumber + 1}";
        //}


        public int GetSemitones()
        {
            int scaleNumber = ((int)name) % 7;
            int offset = scaleNumber >= 3 ? -1 : 0;
            return scaleNumber * 2 + tune + offset;
        }

        public static Note FromSemitones(int count)
        {
            int scaleNumber = (count % 12 + 1) / 2;
            Note result = new() {
                name = (NoteName)scaleNumber
            };
            result.tune = (count - result.GetSemitones() + 6) % 12 - 6;
            return result;
        }

        public Note ToNearestKey()
        {
            Note result = this;
            int resultSemitones = result.GetSemitones();

            while (result.tune <= -2) {
                int actualScaleNumber = ((int)result.name + 6) % 7;
                int actualScaleSemitones = (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

                int tuneOffset = resultSemitones - actualScaleSemitones;
                tuneOffset = (tuneOffset + 126) % 12 - 6;

                result = new() {
                    name = (NoteName)(actualScaleNumber + 7),
                    tune = tuneOffset,
                };
            }

            while (result.tune >= 2) {
                int actualScaleNumber = ((int)result.name + 1) % 7;
                int actualScaleSemitones = (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

                int tuneOffset = resultSemitones - actualScaleSemitones;
                tuneOffset = (tuneOffset + 126) % 12 - 6;

                result = new() {
                    name = (NoteName)(actualScaleNumber + 7),
                    tune = tuneOffset,
                };
            }

            //if (result.tune <= -3) {

            //}

            return result;
        }

        public Note ToRelativeKey(Note root, bool isNumber = false)
        {
            int scaleOffset = 7 - ((int)root.name) % 7;
            int actualScaleNumber = ((int)name + scaleOffset) % 7;
            int actualScaleSemitones =  (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

            int capo = 12 - root.GetSemitones();
            Note result = FromSemitones(GetSemitones() + capo);
            int resultScaleNumber = ((int)result.name) % 7;
            int resultScaleSemitones = (resultScaleNumber >= 3 ? -1 : 0) + resultScaleNumber * 2;

            int tuneOffset = resultScaleSemitones - actualScaleSemitones;
            tuneOffset = (tuneOffset + 6) % 12 - 6;


            return new Note() {
                name = (NoteName)(actualScaleNumber + (isNumber ? 14 : 7) ),
                tune = result.tune + tuneOffset,
            }.ToNearestKey();
        }

        public static readonly Note Empty = new() { name = NoteName.C, tune = 0 };
        public static readonly Note Note_A = new() { name = NoteName.A, tune = 0 };
        public static readonly Note Unknown = new() { name = NoteName.Unknown, tune = 0 };
        public Note MinorToMajor()
        {
            Note result = ToRelativeKey(Note_A);
            result.name -= 7;
            return result;
        }

        public override bool Equals(object? obj)
        {
            return obj is Note note &&
                   name == note.name &&
                   tune == note.tune;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(name, tune);
        }

        public static bool operator == (Note left, Note right)
        {
            return (int)left.name % 7 == (int)right.name % 7 && left.tune == right.tune;
        }

        public static bool operator !=(Note left, Note right)
        {
            return (int)left.name % 7 != (int)right.name % 7 || left.tune != right.tune;
        }

    }



    public enum NoteName
    {
        Unknown = 65536,

        C = 0,
        D = 1,
        E = 2,
        F = 3,
        G = 4,
        A = 5,
        B = 6,

        I = 7,
        II = 8,
        III = 9,
        IV = 10,
        V = 11,
        VI = 12,
        VII = 13,

        _1 = 14,
        _2 = 15,
        _3 = 16,
        _4 = 17,
        _5 = 18,
        _6 = 19,
        _7 = 20,
    }

    public enum ChordTone
    {
        Root = 0,

        Minor_3rd = 3,
        Major_3rd = 4,
        Diminished_5th = 6,
        Perfect_5th = 7,
        Augumented_5th = 8,
        Diminished_7th = 9,
        Minor_7th = 10,
        Major_7th = 11,

        Octave = 12,

        Flat_9th = 13,
        Natural_9th = 14,
        Sharp_9th = 15,
        Flat_11th = 16,
        Natural_11th = 17,
        Sharp_11th = 18,
        Flat_13th = 20,
        Natural_13th = 21,

        Suspended_None = 24,
        Suspended_Flat_2nd = 25,
        Suspended_Natural_2nd = 26,
        Suspended_Sharp_2nd = 27,
        Suspended_Flat_4th = 28,
        Suspended_Natural_4th = 29,
        Suspended_Sharp_4th = 30,
    }

    public enum ChordQuality_37
    {
        Dominant = 0,         // M3, m7
        Major = 1,            // M3, M7
        Minor = 2,            // m3, m7
        MinorMajor = 3,       // m3, M7
        Diminished = 4,       // m3, d7
    }
    public enum IntervalType
    {
        Diminished = -2,
        Minor = -1,
        Perfect = 0,
        Major = 1,
        Augumented = 2,
    }

    public enum FifthType
    {
        None = 0,
        Diminished = 1,
        Augmented = 2,
        HalfDiminished = 4,
    }

    public struct ChordName
    {
        public Note bass;
        public Note chordRoot;
        public ChordQuality_37 quality;
        public FifthType fifthType;
        public int degree;
        public bool isOpenSlashChord;
        public bool isNoChord;
        public bool isOtherChord;
        public bool isAltered;
        public bool isNewParagraph;
        public bool isNewLine;
        public List<ChordTone> adds;
        public List<ChordTone> tensions;
        public List<ChordTone> suspends;
        public List<int> omits;

        static readonly Dictionary<ChordTone, string> textFromChordTone = new() {
            { ChordTone.Root, "" }, 
            { ChordTone.Minor_3rd, "m3" },  
            { ChordTone.Major_3rd, "M3" },
            { ChordTone.Diminished_5th, "-5" },
            { ChordTone.Perfect_5th, "5" },
            { ChordTone.Augumented_5th, "+5" },
            { ChordTone.Diminished_7th, "°7" },
            { ChordTone.Minor_7th, "m7" },
            { ChordTone.Major_7th, "M7" },
            { ChordTone.Octave, "8" },
            { ChordTone.Flat_9th, "b9" },
            { ChordTone.Natural_9th, "9" },
            { ChordTone.Sharp_9th, "#9" },
            { ChordTone.Flat_11th, "b11" },
            { ChordTone.Natural_11th, "11" },
            { ChordTone.Sharp_11th, "#11" },
            { ChordTone.Flat_13th, "b13" },
            { ChordTone.Natural_13th, "13" },
            { ChordTone.Suspended_None, "" },
            { ChordTone.Suspended_Flat_2nd, "susb2" },
            { ChordTone.Suspended_Natural_2nd, "sus2" },
            { ChordTone.Suspended_Sharp_2nd, "sus#2" },
            { ChordTone.Suspended_Flat_4th, "susb4" },
            { ChordTone.Suspended_Natural_4th, "sus4" },
            { ChordTone.Suspended_Sharp_4th, "sus#4" },
        };

        static readonly Dictionary<FifthType, string> textFromFifthType = new() {
            { FifthType.None, "" },
            { FifthType.Diminished, "°" },
            { FifthType.Augmented, "+" },
            { FifthType.HalfDiminished, "ø" },
            {(FifthType)6, "+°" },
        };


        static readonly Dictionary<ChordQuality_37, string> textFromChordQuality = new() {
            { ChordQuality_37.Dominant, "" },
            { ChordQuality_37.Minor, "m" },
            { ChordQuality_37.Major, "M" },
            { ChordQuality_37.MinorMajor, "mM" },
            { ChordQuality_37.Diminished, "" }
        };



        public IEnumerable<Note> ToComponents()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return ToStandardSymbol();
        }

        public readonly static ChordName NewParagraph = new() { isNewParagraph = true };
        public readonly static ChordName NewLine = new() { isNewLine = true };

        public ChordName ToRelativeKey(Note root)
        {
            ChordName result = this;

            result.bass = bass.ToRelativeKey(root, true);
            result.chordRoot = chordRoot.ToRelativeKey(root);

            return result;
        }

        public string ToStandardSymbol()
        {
            if (isNewParagraph) return "||";
            if (isNewLine) return "|";
            if (isNoChord) return "N.C.";
            if (isOtherChord) return "Other";
            if (isOpenSlashChord) return $"/{bass}";

            string root = chordRoot.ToString();
            string mod = textFromFifthType.GetOrDefault(fifthType, "");

            string quality = 
                ((this.degree < 7 || this.degree == 69) && this.quality == ChordQuality_37.Major)
                ? "" : textFromChordQuality.GetOrDefault(this.quality, "");

            string degree = (this.degree == 0 || (fifthType == FifthType.HalfDiminished && this.degree == 7))
                ? "" : this.degree.ToString();


            string sus = string.Join(string.Empty, suspends.Select(e => textFromChordTone.GetOrDefault(e, "")));

            string alt = isAltered ? "alt." : "";



            string[] tensions = this.tensions.Select(e => textFromChordTone.GetOrDefault(e, "")).ToArray();
            string[] adds = this.adds.Select(e => textFromChordTone.GetOrDefault(e, "")).ToArray();


            bool needBracket = false;
            bool isAlt = false;
            foreach (var t in tensions) {
                if (t.Length == 0) continue;
                char first = t.First();
                if (first >= '0' && first <= '9') {
                    needBracket = true;
                } else {
                    isAlt = true;
                }
            }

            string tension;
            if (!isAlt && sus.Length == 0 && adds.Length == 0 && tensions.Length == 1 && this.tensions[0] <= ChordTone.Natural_9th)
                if (this.quality == ChordQuality_37.Minor || degree.Length == 0 || !isAltered)
                    tension = $"(add{tensions[0]})";
                else
                    tension = $"add{tensions[0]}";
            else if (!needBracket)
                tension = string.Join(string.Empty, tensions);
            else
                tension = $"({string.Join(',', tensions)})";

            string add = adds.Length == 0 ? "" : $"(add{string.Join(',', adds)})";



            string omit = omits.Count == 0 ? "" : $"(omit{string.Join(',', omits.Select(e => e.ToString()))})";

            string slash = "";
            if (bass != chordRoot) 
                slash = $"/{bass}";

            return root + mod + quality + degree + sus + alt + tension + add + omit + slash;
        }

    }


    public static class ChordNameParse
    {
        /// <summary>
        /// Group 1:  N.C.
        /// ======== Root Bass =========
        /// Group 2:  #b - before
        /// Group 3:  Bass note
        /// Group 4:  #b - after
        /// ========== Chord ===========
        /// Group 5:  #b - before
        /// Group 6:  Chord Name
        /// Group 7:  #b - after
        /// Group 8:  dim / aug / half-diminish
        /// Group 9:  major / minor / mM / half-diminish / Ionian ...
        /// Group 10: dim / aug / half-diminish
        /// Group 11: sus(4)
        /// Group 12: 7 / 9 / 11 / 13 / 5 / 69 ...
        /// Group 13: sus4 / sus2 / sus#4 / susb2 ...
        /// Group 14: omit5 / (omit 1,3,5)
        /// Group 15: dim / aug / half-diminish
        /// Group 16: last 7 chance
        /// Group 17: altered
        /// Group 18: tension / add
        /// ======== Slash Chord ========
        /// Group 19: #b - before
        /// Group 20: Bass note
        /// Group 21: #b - after
        /// Group 22: omit5 / (omit 1,3,5)
        /// </summary>
        static readonly Regex reg_chord_name = new(
            @"([Nn]\.?[Cc]\.?)|\/([#♯xb♭♮]?[#♯b♭]?|\b)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|(\b|[#♯xb♭♮][#♯b♭]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)([ØøΦ∅]|(?:[m-]|[Mm]in)(?:[MΔ△]|[Mm]aj)|[Mm]aj(?:or)?|MAJ(?:OR)?|[Mm]in(?:or)?|MIN(?:OR)?|[Ii]on(?:ian)?|ION(?:IAN)?|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Mm]ix(?:o|olydian)?|MIX(?:O|OLYDIAN)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?|[Ll]oc(?:rian)?|LOC(?:RIAN)?|[m-]|[MΔ△]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(sus(?!\d)|)(69|11|13|[796513]|)((?:sus[#♯b♭]?[24]?){1,2}|)(\s?\(?omi?t\s?[0-9,\s]+\)?|)(?:(\+(?!\d)|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅])\s?(7)?|)(\s?|\s?\(?[Aa]lt(?:ered)?\.?\)?)?(\(?(?:[Aa][Dd][Dd])?(?:(?:[#♯b♭♮+-]?)(?:11|13|69|[79651234]))?(?:(?:[\b\/,.]|\)?\(|[#♯b♭+-]?)[#♯b♭+-]?(?:11|13|[79651234]))*\)?)(?:(?:\/|[Oo][Nn])([#♯b♭]?[#♯xb♭♮]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|)(\s?\(?omi?t\s?[0-9,\s]+\)?|)(?!\w)"
        );
        static readonly Regex reg_sharp = new(@"[#♯-]");
        static readonly Regex reg_double_sharp = new(@"x");
        static readonly Regex reg_flat = new(@"[b♭+]");

        static readonly Regex reg_augmented = new(@"\+|aug", RegexOptions.IgnoreCase);
        static readonly Regex reg_diminished = new(@"°|dim", RegexOptions.IgnoreCase);
        static readonly Regex reg_half_diminished = new(@"[øΦ∅]|loc(?:rian)?", RegexOptions.IgnoreCase);
        static readonly Regex reg_minorMajor = new(@"(?:[m-]|[Mm]in)(?:[MΔ△]|[Mm]aj)");
        static readonly Regex reg_minor = new(@"[m-]|[Mm]in|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Mm]in(?:or)?|MIN(?:OR)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?");
        static readonly Regex reg_major = new(@"[MΔ△]|[Mm]aj|[Mm]aj(?:or)?|MAJ(?:OR)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Ii]on(?:ian)?|ION(?:IAN)?");
        
        static readonly Regex reg_suspended = new(@"sus([#♯b♭])?([24])?", RegexOptions.IgnoreCase);
        static readonly Regex reg_add = new(@"add((?:[\s,.]?[#♯b♭]?(?:11|13|\d))+)", RegexOptions.IgnoreCase);
        static readonly Regex reg_number = new(@"\d+");
        static readonly Regex reg_tension = new(@"([#♯b♭♮+-])?(\d+)");


        static readonly Dictionary<int, string> notationFromSemitone = new() {
            { -2, "bb" }, { -1, "b" }, { 0, "" }, { 1, "s" }, { 2, "x" }
        };

        static readonly int[] semitonesFromInterval = new int[] { 0, 0, 2, 4, 5, 7, 9, 11, 12 };



        private static bool ParseNote(string notationLeft, string name, string notationRight, out Note note)
        {
            if (name.Length == 0) {
                note = Note.Empty;
                return false;
            }
            name = name.ToUpper();

            string notation = notationLeft + notationRight;

            int sharpCount = reg_sharp.GetCount(notation);
            int doubleSharpCount = reg_double_sharp.GetCount(notation);
            int flatCount = reg_flat.GetCount(notation);

            int semitones = sharpCount + 2 * doubleSharpCount - flatCount;

            note = new Note() {
                name = name.ToEnum<NoteName>(),
                tune = semitones,
            };
            
            return true;
        }

        private static FifthType ParseAugDim(string text)
        {
            if (text.Length == 0) return FifthType.None;

            if (reg_diminished.IsMatch(text))
                return FifthType.Diminished;
            if (reg_augmented.IsMatch(text))
                return FifthType.Augmented;

            return FifthType.HalfDiminished;
        }

        private static ChordQuality_37 ParseQuality(string text)
        {
            if (text.Length == 0) 
                return ChordQuality_37.Dominant;

            if (reg_minorMajor.IsMatch(text)) 
                return ChordQuality_37.MinorMajor;
            if (reg_major.IsMatch(text))
                return ChordQuality_37.Major;
            if (reg_minor.IsMatch(text))
                return ChordQuality_37.Minor;

            return ChordQuality_37.Dominant;
        }

        private static IEnumerable<int> ParseOmits(string text)
        {
            if (text.Length == 0) yield break;
            foreach (Match match in reg_number.Matches(text).Cast<Match>()) {
                yield return int.Parse(match.Value);
            }

        }

        private static IEnumerable<ChordTone> ParseChordTones(string text)
        {
            foreach (Match match in reg_tension.Matches(text).Cast<Match>()) {
                var groups = match.Groups;

                string notation = groups[1].Value;
                int interval = int.Parse(groups[2].Value);
                if (interval == 0) continue;

                if (interval % 2 == 0) interval += 7;

                int tune = 0;
                if (reg_flat.IsMatch(notation)) tune = -1;
                if (reg_sharp.IsMatch(notation)) tune = 1;

                int semitones = semitonesFromInterval[interval % 7];
                ChordTone result = interval < 8 ? ChordTone.Root : ChordTone.Octave;

                result += semitones + tune;

                //if (result == ChordTone.Perfect_5th) {
                //    continue;
                //}

                yield return result;
            }
        }


        public static IEnumerable<ChordName> ParseChordText(string chordText)
        {
            chordText = chordText.ToDBC();

            var matches = reg_chord_name.Matches(chordText);

            foreach (Match match in matches.Cast<Match>()) {
                var groups = match.Groups;

                string noChord = groups[1].Value;

                if (noChord.Length != 0) {
                    yield return new ChordName() { isNoChord = true };
                    continue;
                }

                string rootBass_notation_before = groups[2].Value;
                string rootBass_name = groups[3].Value;
                string rootBass_notation_after = groups[4].Value;

                if (ParseNote(rootBass_notation_before, rootBass_name, rootBass_notation_after, out Note bass)) {
                    yield return new ChordName() { bass = bass, isOpenSlashChord = true };
                    continue;
                }

                string chordRoot_notation_before = groups[5].Value;
                string chordRoot_name = groups[6].Value;
                string chordRoot_notation_after = groups[7].Value;


                Note chordRoot;
                ParseNote(chordRoot_notation_before, chordRoot_name, chordRoot_notation_after, out chordRoot);

                FifthType fifthType = FifthType.None;

                string fifth_modification_before = groups[8].Value;
                fifthType |= ParseAugDim(fifth_modification_before);

                string chord_quality = groups[9].Value;
                ChordQuality_37 quality = ParseQuality(chord_quality);
                if (reg_half_diminished.IsMatch(chord_quality))
                    fifthType |= FifthType.HalfDiminished;

                string fifth_modification_after = groups[10].Value;
                fifthType |= ParseAugDim(fifth_modification_after);

                ChordTone sus4 = ChordTone.Suspended_None, sus2 = ChordTone.Suspended_None;
                string suspended_before = groups[11].Value;
                if (suspended_before.Length > 0)
                    sus4 = ChordTone.Suspended_Natural_4th;

                string chord_degree = groups[12].Value;
                int degree;
                if (chord_degree.Length == 0) degree = 0;
                else degree = int.Parse(chord_degree);

                string suspended_after = groups[13].Value;
                foreach (Match mm in reg_suspended.Matches(suspended_after).Cast<Match>()) {
                    string notation = mm.Groups[1].Value;
                    string interval = mm.Groups[2].Value;

                    int tune = 0;
                    if (reg_flat.IsMatch(notation)) tune = -1;
                    if (reg_sharp.IsMatch(notation)) tune = 1;

                    if (interval == "2") 
                        sus2 = ChordTone.Suspended_Natural_2nd + tune;
                    else 
                        sus4 = ChordTone.Suspended_Natural_4th + tune;
                }

                List<int> omits = new(); 
                string omit_before = groups[14].Value;
                omits.AddRange(ParseOmits(omit_before));

                string fifth_modification_final = groups[15].Value;
                fifthType |= ParseAugDim(fifth_modification_final);

                // ignore this
                string chord_quality_7th = groups[16].Value;
                string altered = groups[17].Value;
                bool isAltered = altered.Length > 0;

                string mod_text = groups[18].Value;

                List<ChordTone> adds = new();
                foreach (Match mod_match in reg_add.Matches(mod_text).Cast<Match>()) {
                    string add_text = mod_match.Groups[1].Value;
                    adds.AddRange(ParseChordTones(add_text));
                }

                string tensions_text = reg_add.Replace(mod_text, string.Empty);

                List<ChordTone> tensions = new();
                tensions.AddRange(ParseChordTones(tensions_text));

                string slashBass_notation_before = groups[19].Value;
                string slashBass_name = groups[20].Value;
                string slashBass_notation_after = groups[21].Value;

                if (!ParseNote(slashBass_notation_before, slashBass_name, slashBass_notation_after, out Note slashBass))
                    bass = chordRoot;
                else
                    bass = slashBass;

                string omit_after = groups[22].Value;
                omits.AddRange(ParseOmits(omit_after));


                // auto convert Cm7b5 to Cø
                if (quality == ChordQuality_37.Minor && degree >= 7 && tensions.Remove(ChordTone.Diminished_5th)) {
                    quality = ChordQuality_37.Dominant;
                    fifthType |= FifthType.HalfDiminished;
                }

                // auto convert Cmb5 to C°
                if (quality == ChordQuality_37.Minor && degree == 0 && tensions.Remove(ChordTone.Diminished_5th)) {
                    fifthType |= FifthType.Diminished;
                }

                if ((fifthType & FifthType.Diminished) != 0)
                    quality = ChordQuality_37.Diminished;


                if (degree == 0 && (tensions.Remove(ChordTone.Natural_13th) || adds.Remove(ChordTone.Natural_13th))) 
                    degree = 6;
                if (degree == 6 && (tensions.Remove(ChordTone.Natural_9th) || adds.Remove(ChordTone.Natural_9th))) 
                    degree = 69;

                //if (tensions.Remove(ChordTone.Natural_9th))
                //    adds.Add(ChordTone.Natural_9th);
                //if (tensions.Remove(ChordTone.Natural_11th))
                //    adds.Add(ChordTone.Natural_11th);
                //if (tensions.Remove(ChordTone.Natural_13th))
                //    adds.Add(ChordTone.Natural_13th);


                if ((degree < 7 || degree == 69) && quality == ChordQuality_37.MinorMajor)
                    quality = ChordQuality_37.Minor;
                if ((degree < 7 || degree == 69) && quality == ChordQuality_37.Dominant)
                    quality = ChordQuality_37.Major;



                List<ChordTone> suspends = new();
                if (sus4 != ChordTone.Suspended_None) suspends.Add(sus4);
                if (sus2 != ChordTone.Suspended_None) suspends.Add(sus2);

                //if (sus4 == ChordTone.Suspended_Natural_4th && tensions.Contains(ChordTone.Natural_9th)) {

                //}

                //if (omits.Count == 2 && omits[1] == 5) {

                //}


                yield return new ChordName() { 
                    bass = bass,
                    chordRoot = chordRoot,
                    quality = quality,
                    fifthType = fifthType,
                    degree = degree,
                    isAltered = isAltered,
                    adds = adds,
                    tensions = tensions,
                    suspends = suspends,
                    omits = omits,
                };

            }


        }


    }
}
