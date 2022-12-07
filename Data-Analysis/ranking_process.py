import csv
from typing import Callable, Iterable


def default_func(list: list[int]) -> float:
    # return 1 if len(list) > 0 else 0
    r = 0.0
    list.sort()
    pts = [20 / (e + 19) for e in list]
    for i, pt in enumerate(pts):
        r += (0.9 ** i) * pt
    return r + 1 if r > 0.001 else 0


def process_ranking(path: str, weight_func: Callable[[list[int]], float] = default_func) -> dict[str, float]:
    """
    Returns dict of [Title, Weight]
    :param path: The path of /data/
    :param weight_func: Function(e: list[int]) -> float to calculate the weight of music by ranks in history.
    """

    ps = [
        lambda x: 201200 <= x < 201300,
        lambda x: 201300 <= x < 201400,
        lambda x: 201400 <= x < 201500,
        lambda x: 201500 <= x < 201600,
        lambda x: 201600 <= x < 201700,
        lambda x: 201700 <= x < 201800,
        lambda x: 201800 <= x < 201900,
        lambda x: 201900 <= x < 202000,
        lambda x: 202000 <= x < 202100,
        lambda x: 202100 <= x < 202200,
        lambda x: 202200 <= x < 202300,
    ]

    r = dict[str, list[float]]()
    with open(path + '/ranking_rearrange.csv', 'r', encoding='utf-8') as f:
        reader = csv.reader(f)
        # [Title] , [Month]:[Rank] , ...
        for row in reader:
            title = row[0].replace('+', ' ')
            data = row[1:]
            ranks = [list(map(lambda x: int(x), e.split(':'))) for e in data]
            r[title] = list[float]()
            r[title].append(weight_func(list(map(lambda x: x[1], ranks))))
            for p in ps:
                year_ranks = list(map(lambda x: x[1], filter(lambda x: p(x[0]), ranks)))
                r[title].append(weight_func(year_ranks))


    r = {k: v for k, v in sorted(r.items(), key=lambda item: item[1], reverse=True)}
    for k, v in r.items():
        print(k, v)

    with open(path + '/ranking_weighted_sum.csv', 'w', newline='', encoding='utf-8-sig') as f:
        writer = csv.writer(f)
        for k, v in r.items():
            writer.writerow([k] + v)

    return r
