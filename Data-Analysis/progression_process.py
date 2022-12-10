import csv
from collections import defaultdict
from typing import Callable
import re


chord_name_map = {
    'bI'   : 11, 'I'   : 0,  '#I'   : 1,
    'bII'  : 1,  'II'  : 2,  '#II'  : 3,
    'bIII' : 3,  'III' : 4,  '#III' : 5,
    'bIV'  : 4,  'IV'  : 5,  '#IV'  : 6,
    'bV'   : 6,  'V'   : 7,  '#V'   : 8,
    'bVI'  : 8,  'VI'  : 9,  '#VI'  : 10,
    'bVII' : 10, 'VII' : 11, '#VII' : 0,
}
note_name_map = {
    'b1' : 11, '1' : 0,  '#1' : 1,
    'b2' : 1,  '2' : 2,  '#2' : 3,
    'b3' : 3,  '3' : 4,  '#3' : 5,
    'b4' : 4,  '4' : 5,  '#4' : 6,
    'b5' : 6,  '5' : 7,  '#5' : 8,
    'b6' : 8,  '6' : 9,  '#6' : 10,
    'b7' : 10, '7' : 11, '#7' : 0,
}
reg_chordname = re.compile(r'[#b]?(?:VI{1,2}|I?V|I{1,3})')
reg_bassname = re.compile(r'\/([#b]?[1-7])')
def get_chord_bass(chord: str):
    match_bassname = reg_bassname.search(chord)
    if match_bassname is not None:
        return note_name_map[match_bassname.group(1)]

    match_chordname = reg_chordname.search(chord)
    if match_chordname is not None:
        return chord_name_map[match_chordname.group(0)]

    return 0





def process_progressions(path: str, r: dict[str, list[int]]):

    prog = defaultdict[str, list[float]](lambda: [0.0] * 13)

    first_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)
    two_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)
    three_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)


    conn_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)

    # data = dict[str, list[str]]()

    music_count = 0
    sums_in_year = [0.0] * 13
    notes_in_year = [0] * 13

    with open(path + '/music_db.csv', 'r', encoding='utf-8') as f:
        reader = csv.reader(f)
        music_count += 1
        for row in reader:
            title = row[1]
            simple_chords = row[8].split(' ')
            progressions = row[9].split('|')
            # data[title] = progressions


            if len(progressions) == 1 and len(progressions[0]) == 0:
                continue

            progset = defaultdict[str, float](lambda: 0)
            set1 = defaultdict[str, float](lambda: 0)
            set2 = defaultdict[str, float](lambda: 0)
            set3 = defaultdict[str, float](lambda: 0)
            conn = defaultdict[str, float](lambda: 0)

            for progression in progressions:
                progset[progression] = 1


            chord_count = len(simple_chords)
            i = 0
            for chord in simple_chords:
                set1[chord] += 1
                if i + 1 < chord_count:
                    set2[chord + ' ' + simple_chords[i+1]] += 1
                    conn[str(get_chord_bass(chord)) + ' ' + str(get_chord_bass(simple_chords[i+1]))] += 1
                if i + 2 < chord_count:
                    set3[chord + ' ' + simple_chords[i+1] + ' ' + simple_chords[i+2]] += 1

                i += 1

            sums_in_year[0] += 1
            notes_in_year[0] += chord_count



            is_rank_in = title in r
            if is_rank_in:
                weights = r[title]
                for i in range(12):
                    sums_in_year[i+1] += weights[i]
                    notes_in_year[i+1] += chord_count * weights[i]

            for [progression, count] in progset.items():
                in_year = prog[progression]
                in_year[0] += count

                if is_rank_in:
                    for i in range(12):
                        in_year[i+1] += weights[i] * count

            for [seg, count] in set1.items():
                in_year = first_chords[seg]
                in_year[0] += count

                if is_rank_in:
                    for i in range(12):
                        in_year[i + 1] += weights[i] * count

            for [seg, count] in set2.items():
                in_year = two_chords[seg]
                in_year[0] += count

                if is_rank_in:
                    for i in range(12):
                        in_year[i + 1] += weights[i] * count

            for [seg, count] in set3.items():
                in_year = three_chords[seg]
                in_year[0] += count

                if is_rank_in:
                    for i in range(12):
                        in_year[i + 1] += weights[i] * count


            for [seg, count] in conn.items():
                in_year = conn_chords[seg]
                in_year[0] += count

                if is_rank_in:
                    for i in range(12):
                        in_year[i + 1] += weights[i] * count

    # Normalization

    for i in range(13):

        mul = 100.0 / sums_in_year[i]
        smul1 = 100.0 / notes_in_year[i]
        smul2 = 100.0 / (notes_in_year[i]-1)
        smul3 = 100.0 / (notes_in_year[i]-2)

        for music in prog.values():
            music[i] *= mul

        for music in first_chords.values():
            music[i] *= smul1

        for music in two_chords.values():
            music[i] *= smul2

        for music in conn_chords.values():
            music[i] *= smul2

        for music in three_chords.values():
            music[i] *= smul3

    header = ['key', 'total', 'weighted',
              '2012', '2013', '2014', '2015', '2016', '2017', '2018', '2019', '2020', '2021', '2022']

    with open(path + '/progressions.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(prog.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)

    with open(path + '/first_chords.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(first_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)

    with open(path + '/two_chords.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        writer.writerow(['semi'] + header)
        i = 0
        for k, v in {k: v for k, v in sorted(two_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow(
                [
                    ' '.join(map(lambda x: str(get_chord_bass(x)), k.split(' '))),
                    k
                ] + v
            )
            i += 1
            if i > 2000: 
                break


    with open(path + '/conn_chords.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        writer.writerow(header)
        for k, v in {k: v for k, v in sorted(conn_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)

    with open(path + '/three_chords.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(three_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)