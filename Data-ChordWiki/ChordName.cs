using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Data_ChordWiki
{
    public enum Note
    {
        Cbb = -14,
        Dbb = -12,
        Ebb = -10,
        Fbb = -9,
        Gbb = -7,
        Abb = -5,
        Bbb = -3,

        Cb = -13,
        Fb = -8,

        C = 0,
        Db = 1,
        D = 2,
        Eb = 3,
        E = 4,
        F = 5,
        Gb = 6,
        G = 7,
        Ab = 8,
        A = 9,
        Bb = 10,
        B = 11,

        Cs = 13,
        Ds = 15,
        Es = 17,
        Fs = 18,
        Gs = 20,
        As = 22,
        Bs = 24,

        Cx = 26,
        Dx = 28,
        Ex = 30,
        Fx = 31,
        Gx = 33,
        Ax = 35,
        Bx = 37,
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
        Major = 0,            // M3, M7
        Minor = 1,            // m3, m7
        Dominant = 2,         // M3, m7
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
        public IntervalType fifthInterval;
        public bool isOpenSlashChord;
        public bool isNoChord;
        public bool isOtherChord;
        public bool isAltered;
        public List<ChordTone> tensions;
        public List<ChordTone> suspends;
        public List<int> omits;

        public IEnumerable<Note> ToComponents()
        {
            throw new NotImplementedException();
        }

        public string ToStandardSymbol()
        {
            throw new NotImplementedException();
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
            @"([Nn]\.?[Cc]\.?)|\/([#♯xb♭♮]?[#♯b♭]?|\b)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|(\b|[#♯xb♭♮][#♯b♭]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)([ØøΦ∅]|(?:[m-]|[Mm]in)(?:[MΔ△]|[Mm]aj)|[Mm]aj(?:or)?|MAJ(?:OR)?|[Mm]in(?:or)?|MIN(?:OR)?|[Ii]on(?:ian)?|ION(?:IAN)?|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Mm]ix(?:o|olydian)?|MIX(?:O|OLYDIAN)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?|[Ll]oc(?:rian)?|LOC(?:RIAN)?|[m-]|[MΔ△]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(sus|)(69|11|13|[796513]|)((?:sus[#♯b♭]?[24]?){1,2}|)(\s?\(?omi?t\s?[0-9,]+\)?|)(?:(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅])\s?(7)?|)(\s?|\s?\(?[Aa]lt(?:ered)?\.?\)?)?(\(?(?:[Aa][Dd][Dd])?(?:(?:[#♯b♭♮+-]?)(?:11|13|69|[79651234]))?(?:(?:[\b\/,.]|\)?\(|[#♯b♭+-]?)[#♯b♭+-]?(?:11|13|[79651234]))*\)?)(?:(?:\/|[Oo][Nn])([#♯b♭]?[#♯xb♭♮]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|)(\s?\(?omi?t\s?[0-9,]+\)?|)(?!\w)"
        );
        static readonly Regex reg_sharp = new(@"[#♯]");
        static readonly Regex reg_double_sharp = new(@"x");
        static readonly Regex reg_flat = new(@"[b♭]");

        static readonly Regex reg_augmented = new(@"\+|aug", RegexOptions.IgnoreCase);
        static readonly Regex reg_diminished = new(@"°|dim", RegexOptions.IgnoreCase);
        static readonly Regex reg_half_diminished = new(@"[øΦ∅]|loc(?:rian)?", RegexOptions.IgnoreCase);
        static readonly Regex reg_minorMajor = new(@"(?:[m-]|[Mm]in)(?:[MΔ△]|[Mm]aj)");
        static readonly Regex reg_minor = new(@"[m-]|[Mm]in|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Mm]in(?:or)?|MIN(?:OR)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?");
        static readonly Regex reg_major = new(@"[MΔ△]|[Mm]aj|[Mm]aj(?:or)?|MAJ(?:OR)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Ii]on(?:ian)?|ION(?:IAN)?");
        
        static readonly Regex reg_suspended = new(@"sus([#♯b♭])?([24])?", RegexOptions.IgnoreCase);
        static readonly Regex reg_number = new(@"\d+");
        static readonly Regex reg_tension = new(@"([#♯b♭♮+-])?(\d+)");

        static readonly Dictionary<int, string> notationFromSemitone = new() {
            { -2, "bb" }, { -1, "b" }, { 0, "" }, { 1, "s" }, { 2, "x" }
        };

        static readonly int[] semitonesFromInterval = new int[] { 0, 0, 2, 4, 5, 7, 9, 11, 12 };



        private static bool ParseNote(string notationLeft, string name, string notationRight, out Note note)
        {
            if (name.Length == 0) {
                note = Note.C;
                return false;
            }
            name = name.ToUpper();

            string notation = notationLeft + notationRight;

            int sharpCount = reg_sharp.GetCount(notation);
            int doubleSharpCount = reg_double_sharp.GetCount(notation);
            int flatCount = reg_flat.GetCount(notation);

            int semitones = sharpCount + 2 * doubleSharpCount - flatCount;

            note = $"{name}{notationFromSemitone[semitones]}".ToEnum<Note>();

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
            foreach (Match match in reg_number.Matches(text).Cast<Match>()) {
                yield return int.Parse(match.Value);
            }

        }

        private static IEnumerable<ChordTone> ParseTensions(string text)
        {
            foreach (Match match in reg_tension.Matches(text).Cast<Match>()) {
                var groups = match.Groups;

                string notation = groups[0].Value;
                int interval = int.Parse(groups[1].Value);
                if (interval % 2 == 0) interval += 7;

                int tune = 0;
                if (reg_flat.IsMatch(notation)) tune = -1;
                if (reg_sharp.IsMatch(notation)) tune = 1;

                int semitones = semitonesFromInterval[interval % 7];
                ChordTone result = interval < 8 ? ChordTone.Root : ChordTone.Octave;

                yield return result + semitones + tune;
            }
        }


        public static IEnumerable<ChordName> ParseChordText(string chordText)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            chordText = Strings.StrConv(chordText, VbStrConv.Narrow, 0) ?? chordText;
#pragma warning restore CA1416 // 验证平台兼容性

            var matches = reg_chord_name.Matches(chordText);
            List<ChordName> result = new();

            foreach (Match match in matches.Cast<Match>()) {
                var groups = match.Groups;

                string noChord = groups[1].Value;

                if (noChord.Length != 0) {
                    result.Add(new ChordName() { isNoChord = true });
                    continue;
                }

                string rootBass_notation_before = groups[2].Value;
                string rootBass_name = groups[3].Value;
                string rootBass_notation_after = groups[4].Value;

                if (ParseNote(rootBass_notation_before, rootBass_name, rootBass_notation_after, out Note bass)) {
                    result.Add(new ChordName() { bass = bass, isOpenSlashChord = true });
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
                    string notation = mm.Groups[0].Value;
                    string interval = mm.Groups[1].Value;

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

                string tensions_text = groups[18].Value;
                List<ChordTone> tensions = new();
                tensions.AddRange(ParseTensions(tensions_text));

                string slashBass_notation_before = groups[19].Value;
                string slashBass_name = groups[20].Value;
                string slashBass_notation_after = groups[21].Value;

                Note slashBass;
                ParseNote(slashBass_notation_before, slashBass_name, slashBass_notation_after, out slashBass);

                string omit_after = groups[14].Value;
                omits.AddRange(ParseOmits(omit_after));

                IntervalType fifthInterval = IntervalType.Perfect;
                if ((fifthType & FifthType.Diminished) != 0) {
                    quality = ChordQuality_37.Diminished;
                    fifthInterval = IntervalType.Diminished;
                }
                if ((fifthType & FifthType.HalfDiminished) != 0)
                    fifthInterval = IntervalType.Diminished;
                if ((fifthType & FifthType.Augmented) != 0)
                    fifthInterval = IntervalType.Augumented;

                List<ChordTone> suspends = new();
                if (sus4 != ChordTone.Suspended_None) suspends.Add(sus4);
                if (sus2 != ChordTone.Suspended_None) suspends.Add(sus2);

                result.Add(new ChordName() { 
                    bass = bass,
                    chordRoot = chordRoot,
                    quality = quality,
                    fifthInterval = fifthInterval,
                    isAltered = isAltered,
                    tensions = tensions,
                    suspends = suspends,
                    omits = omits,
                });
                continue;
            }


            throw new NotImplementedException();

        }


    }
}
