using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_ChordWiki
{



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


    public struct Note
    {
        static readonly Dictionary<int, string> notationFromSemitone = new() {
            { -3, "bbb" },{ -2, "bb" }, { -1, "b" }, { 0, "" }, { 1, "#" }, { 2, "x" }, { 3, "x#" }
        };

        static readonly float[] weightsOfMajorKey = new float[] {
           0,-1, 0,-1, 0, 0, -1,0,-1,0,-1,0
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


        public float CalculateKeyScore(int[] semitoneMap)
        {
            int tune = GetSemitones() + 12;
            float score = 0;
            for (int j = 0; j < 12; j++) {
                score += weightsOfMajorKey[(tune + j) % 12] * semitoneMap[j];
            }

            return score;
        }


        public static Note FitKey(int[] semitoneMap, out float maxScore)
        {
            float[] scores = new float[12];

            maxScore = float.MinValue;
            int tune = 0;

            for (int i = 0; i < 12; i++) {
                float score = 0f;
                for (int j = 0; j < 12; j++) {
                    score += weightsOfMajorKey[(i + j) % 12] * semitoneMap[j];
                }
                scores[i] = score;
                if (score > maxScore) {
                    maxScore = score;
                    tune = 12 - i;
                }
            }

            //Console.WriteLine(tune);
            //Console.WriteLine(string.Join(',', scores));

            return FromSemitones(tune);
        }


        public int GetSemitones()
        {
            int scaleNumber = ((int)name) % 7;
            int offset = scaleNumber >= 3 ? -1 : 0;
            return (scaleNumber * 2 + tune + offset + 120) % 12;
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

        public Note ToNearestKey(bool isNumber = false)
        {
            Note result = this;
            int resultSemitones = result.GetSemitones();

            while (result.tune <= -2) {
                int actualScaleNumber = ((int)result.name + 6) % 7;
                int actualScaleSemitones = (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

                int tuneOffset = resultSemitones - actualScaleSemitones;
                tuneOffset = (tuneOffset + 126) % 12 - 6;

                result = new() {
                    name = (NoteName)(actualScaleNumber + (isNumber ? 14 : 7)),
                    tune = tuneOffset,
                };
            }

            while (result.tune >= 2) {
                int actualScaleNumber = ((int)result.name + 1) % 7;
                int actualScaleSemitones = (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

                int tuneOffset = resultSemitones - actualScaleSemitones;
                tuneOffset = (tuneOffset + 126) % 12 - 6;

                result = new() {
                    name = (NoteName)(actualScaleNumber + (isNumber ? 14 : 7)),
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
            int actualScaleSemitones = (actualScaleNumber >= 3 ? -1 : 0) + actualScaleNumber * 2;

            int capo = 12 - root.GetSemitones();
            Note result = FromSemitones(GetSemitones() + capo);
            int resultScaleNumber = ((int)result.name) % 7;
            int resultScaleSemitones = (resultScaleNumber >= 3 ? -1 : 0) + resultScaleNumber * 2;

            int tuneOffset = resultScaleSemitones - actualScaleSemitones;
            tuneOffset = (tuneOffset + 6) % 12 - 6;


            return new Note() {
                name = (NoteName)(actualScaleNumber + (isNumber ? 14 : 7)),
                tune = result.tune + tuneOffset,
            }.ToNearestKey(isNumber);
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

        public static bool operator ==(Note left, Note right)
        {
            return (int)left.name % 7 == (int)right.name % 7 && left.tune == right.tune;
        }

        public static bool operator !=(Note left, Note right)
        {
            return (int)left.name % 7 != (int)right.name % 7 || left.tune != right.tune;
        }

    }




}
