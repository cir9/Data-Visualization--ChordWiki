import csv
from collections import defaultdict
from typing import Callable


def process_progressions(path: str, r: dict[str, list[int]]):

    prog = defaultdict[str, list[float]](lambda: [0.0] * 13)

    first_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)
    two_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)
    three_chords = defaultdict[str, list[float]](lambda: [0.0] * 13)

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

            for progression in progressions:
                progset[progression] = 1


            chord_count = len(simple_chords)
            i = 0
            for chord in simple_chords:
                set1[chord] += 1
                if i + 1 < chord_count:
                    set2[chord + ' ' + simple_chords[i+1]] += 1
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

    # Normalization

    for i in range(13):

        mul = 100.0 / sums_in_year[i]
        smul = 100.0 / notes_in_year[i]

        for music in prog.values():
            music[i] *= mul

        for music in first_chords.values():
            music[i] *= smul

        for music in two_chords.values():
            music[i] *= smul

        for music in three_chords.values():
            music[i] *= smul

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
        for k, v in {k: v for k, v in sorted(two_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)

    with open(path + '/three_chords.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(three_chords.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)