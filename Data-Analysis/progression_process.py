import csv
from collections import defaultdict
from typing import Callable


def process_progressions(path: str, r: dict[str, list[int]]):

    prog = defaultdict[str, list[float]](lambda: [0.0] * 13)

    # data = dict[str, list[str]]()

    with open(path + '/music_db.csv', 'r', encoding='utf-8') as f:
        reader = csv.reader(f)

        for row in reader:
            title = row[1]
            progressions = row[8].split('|')
            # data[title] = progressions

            is_rank_in = title in r

            for progression in progressions:
                in_year = prog[progression]
                in_year[0] += 1

                if not is_rank_in:
                    continue

                weights = r[title]
                for i in range(12):
                    in_year[i+1] += weights[i]

    with open(path + '/progressions.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in {k: v for k, v in sorted(prog.items(), key=lambda item: item[1][0], reverse=True)}.items():
            writer.writerow([k] + v)
