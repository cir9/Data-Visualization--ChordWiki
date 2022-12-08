using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Data_ChordWiki
{

    public enum ChordTone
    {
        None = 65536,

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

    public partial struct Chord
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
        public ChordTone fifthModification;
        public List<ChordTone> adds;
        public List<ChordTone> tensions;
        public List<ChordTone> suspends;
        public List<int> omits;


        static readonly Dictionary<ChordTone, string> textFromChordTone = new() {
            { ChordTone.None, "" },
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

        static readonly Dictionary<ChordQuality_37, (ChordTone third, ChordTone Seventh)> thirdAndSeventhToneFromChordQuality = new() {
            { ChordQuality_37.Major, (ChordTone.Major_3rd, ChordTone.Major_7th) },
            { ChordQuality_37.Minor, (ChordTone.Minor_3rd, ChordTone.Minor_7th) },
            { ChordQuality_37.MinorMajor, (ChordTone.Minor_3rd, ChordTone.Major_7th) },
            { ChordQuality_37.Dominant, (ChordTone.Major_3rd, ChordTone.Minor_7th) },
            { ChordQuality_37.Diminished, (ChordTone.Minor_3rd, ChordTone.Diminished_7th) },
        };

        static readonly Dictionary<FifthType, ChordTone> fifthToneFromFifthType = new() {
            { FifthType.None, ChordTone.Perfect_5th },
            { FifthType.Diminished, ChordTone.Diminished_5th },
            { FifthType.Augmented, ChordTone.Augumented_5th },
            { FifthType.HalfDiminished, ChordTone.Diminished_5th },
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



        public bool IsValidChord { get => !(isNoChord || isNewLine || isNewParagraph); }
        public bool IsMark { get => isNewLine || isNewParagraph; }
        public bool IsProgressionChord { get => !(isNoChord || isNewLine || isNewParagraph || isOpenSlashChord); }




        public IEnumerable<int> GetComponents()
        {
            if (!IsValidChord) yield break;


            if (isOpenSlashChord) {
                yield return bass.GetSemitones();
                yield break;
            }

            ChordTone root = ChordTone.Root;

            (ChordTone third, ChordTone seventh) = thirdAndSeventhToneFromChordQuality.GetOrDefault(
                quality, (ChordTone.Major_3rd, ChordTone.Minor_7th));

            ChordTone fifth = fifthToneFromFifthType.GetOrDefault(fifthType, ChordTone.Perfect_5th);

            bool hideRoot = false, hideThird = false, hideFifth = false, hideSeventh = false;

            List<ChordTone> notes = new();

            switch (degree) {
                case 0:
                    hideSeventh = true;
                    break;
                case 1:
                    hideThird = true;
                    hideFifth = true;
                    hideSeventh = true;
                    break;
                case 2:
                    notes.Add(ChordTone.Natural_9th);
                    hideThird = true;
                    hideFifth = true;
                    hideSeventh = true;
                    break;
                case 3:
                    hideFifth = true;
                    hideSeventh = true;
                    break;
                case 4:
                    notes.Add(ChordTone.Natural_11th);
                    hideThird = true;
                    hideFifth = true;
                    hideSeventh = true;
                    break;
                case 5:
                    hideSeventh = true;
                    hideThird = true;
                    break;
                case 6:
                    notes.Add(ChordTone.Natural_13th);
                    hideSeventh = true;
                    break;
                case 7:
                    break;
                case 9:
                    notes.Add(ChordTone.Natural_9th);
                    break;
                case 11:
                    notes.Add(ChordTone.Natural_9th);
                    notes.Add(ChordTone.Natural_11th);
                    break;
                case 13:
                    notes.Add(ChordTone.Natural_9th);
                    notes.Add(ChordTone.Natural_11th);
                    notes.Add(ChordTone.Natural_13th);
                    break;
                case 69:
                    notes.Add(ChordTone.Natural_9th);
                    notes.Add(ChordTone.Natural_13th);
                    break;
                default:
                    hideSeventh = true;
                    break;
            }


            if (suspends.Count > 0) {
                hideThird = true;
                notes.AddRange(suspends);
            }

            if (tensions.Contains(ChordTone.Diminished_5th))
                fifth = ChordTone.Diminished_5th;
            if (tensions.Contains(ChordTone.Augumented_5th))
                fifth = ChordTone.Augumented_5th;

            if (omits.Contains(1))
                hideRoot = true;
            if (omits.Contains(3))
                hideThird = true;
            if (omits.Contains(5))
                hideFifth = true;
            if (omits.Contains(7))
                hideSeventh = true;

            if (!hideThird) notes.Add(third);
            if (!hideFifth) notes.Add(fifth);
            if (!hideSeventh) notes.Add(seventh);

            notes.AddRange(tensions);
            notes.Sort();

            int rootSemitomes = chordRoot.GetSemitones();

            if (bass != chordRoot)
                yield return bass.GetSemitones();

            if (!hideRoot)
                yield return (rootSemitomes + (int)root) % 12;

            foreach (var note in notes) {
                yield return (rootSemitomes + (int)note) % 12;
            }

        }
 


        public override string ToString()
        {
            return ToStandardSymbol();
        }

        public readonly static Chord NewParagraph = new() { isNewParagraph = true };
        public readonly static Chord NewLine = new() { isNewLine = true };

        public Chord ToRelativeKey(Note root)
        {
            Chord result = this;

            result.bass = bass.ToRelativeKey(root, true);
            result.chordRoot = chordRoot.ToRelativeKey(root);

            return result;
        }

        public Chord ToSimpleChord()
        {
            if (!IsProgressionChord) return this;

            Chord result = this;
            if (result.degree > 7) result.degree = 7;
            else if (result.degree < 7) result.degree = 0;

            List<ChordTone> tensions = new();
            tensions.AddRange(result.tensions.Where(e => e == ChordTone.Diminished_5th || e == ChordTone.Augumented_5th));
            result.tensions = tensions;
            if (result.degree == 7 && tensions.Count == 0 && result.fifthType != FifthType.HalfDiminished)
                result.degree = 0;

            result.omits = new();
            result.adds = new();
            result.isAltered = false;

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

            string alt = isAltered ? "alt" : "";



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

            string fifth = textFromChordTone.GetOrDefault(fifthModification, "");


            string tension = "";
            if (!isAlt && sus.Length == 0 && adds.Length == 0 && tensions.Length == 1 && this.tensions[0] <= ChordTone.Natural_9th)
                tension = $"(add{tensions[0]})";
            //else if (!needBracket)
            //    tension = string.Join(string.Empty, tensions);
            else if(tensions.Length > 0)
                tension = $"({string.Join(',', tensions)})";

            string add = adds.Length == 0 ? "" : $"(add{string.Join(',', adds)})";



            string omit = omits.Count == 0 ? "" : $"(omit{string.Join(',', omits.Select(e => e.ToString()))})";

            string slash = "";
            if (bass != chordRoot) 
                slash = $"/{bass}";

            return root + mod + quality + degree + fifth + sus + alt + tension + add + omit + slash;
        }

    }


}
