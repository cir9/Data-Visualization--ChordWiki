import csv
from collections import defaultdict
from typing import Callable


def process_progressions(path: str, r: dict[str, list[int]]):

    prog = defaultdict[str, list[float]](lambda: [0.0] * 13)

    # data = dict[str, list[str]]()

    music_count = 0
    sums_in_year = [0.0] * 13

    with open(path + '/music_db.csv', 'r', encoding='utf-8') as f:
        reader = csv.reader(f)
        music_count += 1
        for row in reader:
            title = row[1]
            progressions = row[8].split('|')
            # data[title] = progressions

            progset = defaultdict[str, float](lambda: 0)


            for progression in progressions:
                progset[progression] = 1


            sums_in_year[0] += 1

            is_rank_in = title in r
            if is_rank_in:
                weights = r[title]
                for i in range(12):
                    sums_in_year[i+1] += weights[i]

            for [progression, count] in progset.items():
                in_year = prog[progression]
                in_year[0] += count

                if not is_rank_in:
                    continue

                for i in range(12):
                    in_year[i+1] += weights[i] * count

    # Normalization

    for i in range(13):

        mul = 100.0 / sums_in_year[i]

        for music in prog.values():
            music[i] *= mul


    with open(path + '/progressions.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(prog.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)
