using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data_ChordWiki
{
    public static class Chords
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
        static readonly Regex reg_sharp = new(@"[#♯+]");
        static readonly Regex reg_double_sharp = new(@"x");
        static readonly Regex reg_flat = new(@"[b♭-]");

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

                if (result < 0) continue;
                //if (result == ChordTone.Perfect_5th) {
                //    continue;
                //}

                yield return result;
            }
        }


        public static IEnumerable<Chord> ParseChordText(string chordText)
        {
            chordText = chordText.ToDBC();

            var matches = reg_chord_name.Matches(chordText);

            foreach (Match match in matches.Cast<Match>()) {
                var groups = match.Groups;

                string noChord = groups[1].Value;

                if (noChord.Length != 0) {
                    yield return new Chord() { isNoChord = true };
                    continue;
                }

                string rootBass_notation_before = groups[2].Value;
                string rootBass_name = groups[3].Value;
                string rootBass_notation_after = groups[4].Value;

                if (ParseNote(rootBass_notation_before, rootBass_name, rootBass_notation_after, out Note bass)) {
                    yield return new Chord() { bass = bass, isOpenSlashChord = true };
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

                // auto convert C+5 / C7+5 to C+ / C+7
                if (((quality == ChordQuality_37.Major && degree == 0) || quality == ChordQuality_37.Dominant) 
                    && tensions.Remove(ChordTone.Augumented_5th)) {
                    fifthType |= FifthType.Augmented;
                }


                ChordTone fifthModification = ChordTone.None;

                if (tensions.Remove(ChordTone.Diminished_5th)) {
                    fifthModification = ChordTone.Diminished_5th;
                } else if (tensions.Remove(ChordTone.Augumented_5th)) {
                    fifthModification = ChordTone.Augumented_5th;
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


                yield return new Chord() {
                    bass = bass,
                    chordRoot = chordRoot,
                    quality = quality,
                    fifthType = fifthType,
                    degree = degree,
                    isAltered = isAltered,
                    fifthModification = fifthModification,
                    adds = adds,
                    tensions = tensions,
                    suspends = suspends,
                    omits = omits,
                };

            }


        }


    }

}
