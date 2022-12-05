using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data_ChordWiki
{
    public enum Note
    {
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

        Suspended_Flat_2nd = 25,
        Suspended_Natural_2nd = 26,
        Suspended_Sharp_2nd = 27,
        Suspended_Flat_4th = 28,
        Suspended_Natural_4th = 29,
        Suspended_Sharp_4th = 30,
    }

    public enum ChordType_37
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

    public struct ChordName
    {
        public Note bass;
        public Note chordRoot;
        public ChordType_37 chordType;
        public IntervalType fifthInterval;
        public bool isOpenSlashedChord;
        public List<ChordTone> tensions;
        public List<ChordTone> omits;

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
        /// Group 9:  major / minor / mM / dim / aug / half-diminish / Ionian ...
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
        static readonly Regex reg_chord_name = new Regex(
            @"([Nn]\.?[Cc]\.?)|\/([#♯xb♭♮]?[#♯b♭]?|\b)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|(\b|[#♯xb♭♮][#♯b♭]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|(?:[m-]|[Mm]in)(?:[MΔ△]|[Mm]aj)|[Mm]aj(?:or)?|MAJ(?:OR)?|[Mm]in(?:or)?|MIN(?:OR)?|[Ii]on(?:ian)?|ION(?:IAN)?|[Dd]or(?:ian)?|DOR(?:IAN)?|[Pp]hr(?:y|ygian)?|PHR(?:Y|YGIAN)?|[Ll]yd(?:ian)?|LYD(?:IAN)?|[Mm]ix(?:o|olydian)?|MIX(?:O|OLYDIAN)?|[Aa]eo(?:lian)?|AEO(?:LIAN)?|[Ll]oc(?:rian)?|LOC(?:RIAN)?|[m-]|[MΔ△]|)(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅]|)(sus|)(69|11|13|[796513]|)(sus[#♯b♭]?[24]?|)(\s?\(?omi?t\s?[0-9,]+\)?|)(?:(\+|[Aa][Uu][Gg]|°|[Dd][Ii][Mm]|[ØøΦ∅])\s?(7)?|)(\s?|\s?\(?[Aa]lt(?:ered)?\.?\)?)?(\(?(?:[Aa][Dd][Dd])?(?:(?:[#♯b♭+-]?)(?:11|13|69|[79651234]))?(?:(?:[\b\/,.]|\)?\(|[#♯b♭+-]?)[#♯b♭+-]?(?:11|13|[79651234]))*\)?)(?:(?:\/|[Oo][Nn])([#♯b♭]?[#♯xb♭♮]?)([a-gA-G])([#♯xb♭♮]?[#♯b♭]?)|)(\s?\(?omi?t\s?[0-9,]+\)?|)(?!\w)"
        );
        


    }
}
